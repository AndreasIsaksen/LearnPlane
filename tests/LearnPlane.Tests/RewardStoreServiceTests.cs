using LearnPlane.Web.Data;
using LearnPlane.Web.Models;
using LearnPlane.Web.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LearnPlane.Tests;

public sealed class RewardStoreServiceTests
{
    [Fact]
    public async Task CheckoutDeductsPointsCreatesPurchaseAndClearsCart()
    {
        await using var fixture = await StoreFixture.CreateAsync(earnedPoints: 80, pricePoints: 50);

        var result = await fixture.Service.CheckoutAsync(StoreFixture.UserId);

        Assert.True(result.Success);
        Assert.Equal(30, result.RemainingBalance);
        await using var db = await fixture.Factory.CreateDbContextAsync();
        Assert.Empty(await db.CartItems.ToListAsync());
        var purchase = await db.RewardPurchases.Include(x => x.Lines).SingleAsync();
        Assert.Equal(50, purchase.TotalPoints);
        Assert.Equal("Testbelønning", purchase.Lines.Single().ItemName);
    }

    [Fact]
    public async Task CheckoutWithInsufficientPointsKeepsCartAndCreatesNoPurchase()
    {
        await using var fixture = await StoreFixture.CreateAsync(earnedPoints: 20, pricePoints: 50);

        var result = await fixture.Service.CheckoutAsync(StoreFixture.UserId);

        Assert.False(result.Success);
        Assert.Contains("30 poeng", result.Message);
        await using var db = await fixture.Factory.CreateDbContextAsync();
        Assert.Single(await db.CartItems.ToListAsync());
        Assert.Empty(await db.RewardPurchases.ToListAsync());
    }

    [Fact]
    public async Task BalanceIncludesPointsFromGameLevels()
    {
        await using var fixture = await StoreFixture.CreateAsync(earnedPoints: 20, pricePoints: 50);
        await using (var db = await fixture.Factory.CreateDbContextAsync())
        {
            var course = await db.Courses.SingleAsync();
            var game = new CourseGame { Course = course, Title = "Testspill", Intro = "Test" };
            var level = new GameLevel { CourseGame = game, LevelNumber = 1, Title = "Nivå 1", Instructions = "Test", MaxPoints = 10 };
            db.GameLevelAttempts.Add(new GameLevelAttempt
            {
                UserId = StoreFixture.UserId,
                Level = level,
                CorrectSelections = 3,
                TotalTargets = 3,
                Percentage = 100,
                Passed = true,
                PointsAwarded = 10
            });
            await db.SaveChangesAsync();
        }

        Assert.Equal(30, await fixture.Service.GetBalanceAsync(StoreFixture.UserId));
    }

    private sealed class StoreFixture(SqliteConnection connection, TestDbContextFactory factory, RewardStoreService service)
        : IAsyncDisposable
    {
        public const string UserId = "test-user";
        public TestDbContextFactory Factory { get; } = factory;
        public RewardStoreService Service { get; } = service;

        public static async Task<StoreFixture> CreateAsync(int earnedPoints, int pricePoints)
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            var options = new DbContextOptionsBuilder<LearnPlaneDbContext>().UseSqlite(connection).Options;
            var factory = new TestDbContextFactory(options);
            await using var db = await factory.CreateDbContextAsync();
            await db.Database.EnsureCreatedAsync();
            db.Users.Add(new ApplicationUser { Id = UserId, UserName = "test", DisplayName = "Test" });
            var course = new Course
            {
                Grade = 1,
                Subject = "Test",
                Title = "Testkurs",
                Summary = "Test",
                Content = "Test",
                Difficulty = CourseDifficulty.Lett
            };
            db.Courses.Add(course);
            db.RewardItems.Add(new RewardItem
            {
                Id = 1,
                Name = "Testbelønning",
                Description = "Test",
                ImageUrl = "/test.png",
                PricePoints = pricePoints
            });
            db.QuizAttempts.Add(new QuizAttempt
            {
                UserId = UserId,
                Course = course,
                CorrectAnswers = 4,
                TotalQuestions = 4,
                Percentage = 100,
                Passed = true,
                PointsAwarded = earnedPoints
            });
            db.CartItems.Add(new CartItem { UserId = UserId, RewardItemId = 1, Quantity = 1 });
            await db.SaveChangesAsync();

            var service = new RewardStoreService(factory, new PointBalanceCalculator());
            return new StoreFixture(connection, factory, service);
        }

        public async ValueTask DisposeAsync() => await connection.DisposeAsync();
    }

    public sealed class TestDbContextFactory(DbContextOptions<LearnPlaneDbContext> options)
        : IDbContextFactory<LearnPlaneDbContext>
    {
        public LearnPlaneDbContext CreateDbContext() => new(options);
        public Task<LearnPlaneDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(CreateDbContext());
    }
}
