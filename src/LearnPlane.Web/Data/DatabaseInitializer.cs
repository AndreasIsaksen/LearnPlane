using LearnPlane.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LearnPlane.Web.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services, IConfiguration configuration)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<LearnPlaneDbContext>();
        await db.Database.EnsureCreatedAsync();
        await ApplyIncrementalUpgradesAsync(db);

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var role in new[] { Roles.Admin, Roles.Student })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var admin = await userManager.FindByNameAsync("admin");
        if (admin is null)
        {
            admin = new ApplicationUser { UserName = "admin", DisplayName = "Administrator" };
            var password = configuration["ADMIN_INITIAL_PASSWORD"] ?? "3d9XehYf";
            var result = await userManager.CreateAsync(admin, password);
            if (!result.Succeeded)
                throw new InvalidOperationException($"Administrator kunne ikke opprettes: {string.Join(", ", result.Errors.Select(x => x.Description))}");
            await userManager.AddToRoleAsync(admin, Roles.Admin);
        }

        if (!await db.Courses.AnyAsync())
        {
            db.Courses.AddRange(CurriculumCatalog.CreateCourses());
            await db.SaveChangesAsync();
        }
        else
        {
            await SynchronizeCurriculumAsync(db);
        }

        var coursesWithoutGames = await db.Courses
            .Where(course => !db.CourseGames.Any(game => game.CourseId == course.Id))
            .ToListAsync();
        if (coursesWithoutGames.Count > 0)
        {
            db.CourseGames.AddRange(coursesWithoutGames.Select(GameCatalog.CreateGame));
            await db.SaveChangesAsync();
        }

        if (!await db.RewardItems.AnyAsync())
        {
            db.RewardItems.Add(new RewardItem
            {
                Name = "Leksefri-kupong",
                Description = "En hyggelig forhåndsvisning: Vis kupongen til en voksen og avtal en liten, leksefri bonusstund.",
                ImageUrl = "/images/reward-homework.svg",
                PricePoints = 50,
                IsActive = true
            });
            await db.SaveChangesAsync();
        }
    }

    private static async Task SynchronizeCurriculumAsync(LearnPlaneDbContext db)
    {
        var catalog = CurriculumCatalog.CreateCourses().ToList();
        var existingCourses = await db.Courses
            .Include(x => x.Questions).ThenInclude(x => x.Options)
            .Include(x => x.Game).ThenInclude(x => x!.Levels).ThenInclude(x => x.Cards)
            .AsSplitQuery()
            .ToListAsync();
        var existingByKey = existingCourses
            .GroupBy(x => (x.Grade, x.Subject, x.SortOrder))
            .ToDictionary(x => x.Key, x => x.First());

        var changed = false;
        foreach (var template in catalog)
        {
            if (!existingByKey.TryGetValue((template.Grade, template.Subject, template.SortOrder), out var course))
            {
                db.Courses.Add(template);
                changed = true;
                continue;
            }

            if (course.CatalogVersion == CurriculumCatalog.ContentVersion)
                continue;

            course.Title = template.Title;
            course.Summary = template.Summary;
            course.Content = template.Content;
            course.CatalogVersion = template.CatalogVersion;
            course.Difficulty = template.Difficulty;
            course.IsPublished = template.IsPublished;

            db.QuizQuestions.RemoveRange(course.Questions);
            course.Questions = template.Questions;

            var gameTemplate = GameCatalog.CreateGame(template);
            if (course.Game is null)
            {
                gameTemplate.Course = course;
                course.Game = gameTemplate;
            }
            else
            {
                course.Game.Title = gameTemplate.Title;
                course.Game.Intro = gameTemplate.Intro;
                foreach (var levelTemplate in gameTemplate.Levels)
                {
                    var level = course.Game.Levels.SingleOrDefault(x => x.LevelNumber == levelTemplate.LevelNumber);
                    if (level is null)
                    {
                        course.Game.Levels.Add(levelTemplate);
                        continue;
                    }

                    level.Title = levelTemplate.Title;
                    level.Instructions = levelTemplate.Instructions;
                    level.MaxPoints = levelTemplate.MaxPoints;
                    level.Mode = levelTemplate.Mode;
                    db.GameCards.RemoveRange(level.Cards);
                    level.Cards = levelTemplate.Cards;
                }
            }
            changed = true;
        }

        if (changed)
            await db.SaveChangesAsync();
    }

    private static async Task ApplyIncrementalUpgradesAsync(LearnPlaneDbContext db)
    {
        await db.Database.ExecuteSqlRawAsync("""
            ALTER TABLE "AspNetUsers" ADD COLUMN IF NOT EXISTS "Age" integer NULL;
            ALTER TABLE "Courses" ADD COLUMN IF NOT EXISTS "CatalogVersion" character varying(40) NULL;
            CREATE TABLE IF NOT EXISTS "RewardItems" (
                "Id" integer GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
                "Name" character varying(140) NOT NULL,
                "Description" character varying(600) NOT NULL,
                "ImageUrl" text NOT NULL,
                "PricePoints" integer NOT NULL,
                "IsActive" boolean NOT NULL,
                "CreatedAtUtc" timestamp with time zone NOT NULL,
                "UpdatedAtUtc" timestamp with time zone NOT NULL
            );
            CREATE TABLE IF NOT EXISTS "CartItems" (
                "Id" integer GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
                "UserId" text NOT NULL,
                "RewardItemId" integer NOT NULL,
                "Quantity" integer NOT NULL,
                "CreatedAtUtc" timestamp with time zone NOT NULL,
                CONSTRAINT "FK_CartItems_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE,
                CONSTRAINT "FK_CartItems_RewardItems_RewardItemId" FOREIGN KEY ("RewardItemId") REFERENCES "RewardItems" ("Id") ON DELETE CASCADE
            );
            CREATE TABLE IF NOT EXISTS "RewardPurchases" (
                "Id" integer GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
                "UserId" text NOT NULL,
                "TotalPoints" integer NOT NULL,
                "CreatedAtUtc" timestamp with time zone NOT NULL,
                CONSTRAINT "FK_RewardPurchases_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
            );
            CREATE TABLE IF NOT EXISTS "RewardPurchaseLines" (
                "Id" integer GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
                "RewardPurchaseId" integer NOT NULL,
                "RewardItemId" integer NULL,
                "ItemName" character varying(140) NOT NULL,
                "ItemDescription" character varying(600) NOT NULL,
                "ImageUrl" text NOT NULL,
                "UnitPricePoints" integer NOT NULL,
                "Quantity" integer NOT NULL,
                CONSTRAINT "FK_RewardPurchaseLines_RewardPurchases_RewardPurchaseId" FOREIGN KEY ("RewardPurchaseId") REFERENCES "RewardPurchases" ("Id") ON DELETE CASCADE,
                CONSTRAINT "FK_RewardPurchaseLines_RewardItems_RewardItemId" FOREIGN KEY ("RewardItemId") REFERENCES "RewardItems" ("Id") ON DELETE SET NULL
            );
            CREATE UNIQUE INDEX IF NOT EXISTS "IX_CartItems_UserId_RewardItemId" ON "CartItems" ("UserId", "RewardItemId");
            CREATE INDEX IF NOT EXISTS "IX_CartItems_RewardItemId" ON "CartItems" ("RewardItemId");
            CREATE INDEX IF NOT EXISTS "IX_RewardPurchases_UserId_CreatedAtUtc" ON "RewardPurchases" ("UserId", "CreatedAtUtc");
            CREATE INDEX IF NOT EXISTS "IX_RewardPurchaseLines_RewardPurchaseId" ON "RewardPurchaseLines" ("RewardPurchaseId");
            CREATE INDEX IF NOT EXISTS "IX_RewardPurchaseLines_RewardItemId" ON "RewardPurchaseLines" ("RewardItemId");
            CREATE TABLE IF NOT EXISTS "CourseGames" (
                "Id" integer GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
                "CourseId" integer NOT NULL,
                "Title" character varying(160) NOT NULL,
                "Intro" character varying(500) NOT NULL,
                CONSTRAINT "FK_CourseGames_Courses_CourseId" FOREIGN KEY ("CourseId") REFERENCES "Courses" ("Id") ON DELETE CASCADE
            );
            CREATE TABLE IF NOT EXISTS "GameLevels" (
                "Id" integer GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
                "CourseGameId" integer NOT NULL,
                "LevelNumber" integer NOT NULL,
                "Title" character varying(100) NOT NULL,
                "Instructions" character varying(500) NOT NULL,
                "MaxPoints" integer NOT NULL,
                CONSTRAINT "FK_GameLevels_CourseGames_CourseGameId" FOREIGN KEY ("CourseGameId") REFERENCES "CourseGames" ("Id") ON DELETE CASCADE
            );
            CREATE TABLE IF NOT EXISTS "GameCards" (
                "Id" integer GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
                "GameLevelId" integer NOT NULL,
                "Text" character varying(100) NOT NULL,
                "IsTarget" boolean NOT NULL,
                "SortOrder" integer NOT NULL,
                CONSTRAINT "FK_GameCards_GameLevels_GameLevelId" FOREIGN KEY ("GameLevelId") REFERENCES "GameLevels" ("Id") ON DELETE CASCADE
            );
            CREATE TABLE IF NOT EXISTS "GameLevelAttempts" (
                "Id" integer GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
                "UserId" text NOT NULL,
                "GameLevelId" integer NOT NULL,
                "CorrectSelections" integer NOT NULL,
                "IncorrectSelections" integer NOT NULL,
                "TotalTargets" integer NOT NULL,
                "Percentage" numeric(5,2) NOT NULL,
                "Passed" boolean NOT NULL,
                "PointsAwarded" integer NOT NULL,
                "CompletedAtUtc" timestamp with time zone NOT NULL,
                CONSTRAINT "FK_GameLevelAttempts_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE,
                CONSTRAINT "FK_GameLevelAttempts_GameLevels_GameLevelId" FOREIGN KEY ("GameLevelId") REFERENCES "GameLevels" ("Id") ON DELETE CASCADE
            );
            CREATE UNIQUE INDEX IF NOT EXISTS "IX_CourseGames_CourseId" ON "CourseGames" ("CourseId");
            CREATE UNIQUE INDEX IF NOT EXISTS "IX_GameLevels_CourseGameId_LevelNumber" ON "GameLevels" ("CourseGameId", "LevelNumber");
            CREATE INDEX IF NOT EXISTS "IX_GameCards_GameLevelId_SortOrder" ON "GameCards" ("GameLevelId", "SortOrder");
            CREATE INDEX IF NOT EXISTS "IX_GameLevelAttempts_UserId_GameLevelId" ON "GameLevelAttempts" ("UserId", "GameLevelId");
            CREATE INDEX IF NOT EXISTS "IX_GameLevelAttempts_GameLevelId" ON "GameLevelAttempts" ("GameLevelId");
            ALTER TABLE "GameLevels" ADD COLUMN IF NOT EXISTS "Mode" integer NOT NULL DEFAULT 1;
            ALTER TABLE "GameCards" ALTER COLUMN "Text" TYPE character varying(300);
            ALTER TABLE "GameCards" ADD COLUMN IF NOT EXISTS "PairKey" character varying(40) NULL;
            ALTER TABLE "GameCards" ADD COLUMN IF NOT EXISTS "CorrectPosition" integer NULL;
            ALTER TABLE "GameCards" ADD COLUMN IF NOT EXISTS "VisualCue" character varying(16) NOT NULL DEFAULT '';
            """);
    }
}
