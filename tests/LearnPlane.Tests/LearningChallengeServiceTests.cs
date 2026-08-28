using LearnPlane.Web.Data;
using LearnPlane.Web.Models;
using LearnPlane.Web.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LearnPlane.Tests;

public sealed class LearningChallengeServiceTests
{
    [Fact]
    public async Task QuizAwardsThreeQuartersThenRemainderOnlyOnce()
    {
        await using var fixture = await ChallengeFixture.CreateAsync(userAge: 6, courseGrade: 1);
        var quiz = fixture.CreateQuizService();
        var answers = await fixture.GetQuizAnswersAsync(correctAnswers: 3);

        var first = await quiz.SubmitAsync(fixture.CourseId, ChallengeFixture.UserId, answers);
        var perfectAnswers = await fixture.GetQuizAnswersAsync(correctAnswers: 4);
        var perfect = await quiz.SubmitAsync(fixture.CourseId, ChallengeFixture.UserId, perfectAnswers);
        var repeated = await quiz.SubmitAsync(fixture.CourseId, ChallengeFixture.UserId, perfectAnswers);

        Assert.Equal(8, first.NewlyAwardedPoints);
        Assert.Equal(2, perfect.NewlyAwardedPoints);
        Assert.Equal(0, repeated.NewlyAwardedPoints);
    }

    [Fact]
    public async Task QuizBelowUsersCurrentGradeRecordsAttemptWithoutPoints()
    {
        await using var fixture = await ChallengeFixture.CreateAsync(userAge: 12, courseGrade: 6);
        var result = await fixture.CreateQuizService().SubmitAsync(
            fixture.CourseId, ChallengeFixture.UserId, await fixture.GetQuizAnswersAsync(4));

        Assert.True(result.Score.Passed);
        Assert.False(result.CanEarnPoints);
        Assert.Equal(7, result.CurrentGrade);
        Assert.Equal(0, result.NewlyAwardedPoints);
    }

    [Fact]
    public async Task PassedGameUnlocksNextLevelAndUsesStagedAwards()
    {
        await using var fixture = await ChallengeFixture.CreateAsync(userAge: 6, courseGrade: 1);
        var game = fixture.CreateGameService();
        var view = await game.GetAsync(fixture.CourseId, ChallengeFixture.UserId);
        var firstLevel = Assert.Single(view!.Levels, x => x.Number == 1);
        var targetIds = await fixture.GetGameTargetIdsAsync(firstLevel.Id);

        var first = await game.SubmitAsync(firstLevel.Id, ChallengeFixture.UserId,
            targetIds.Take(3).Append(firstLevel.Cards.First(x => !targetIds.Contains(x.Id)).Id)
                .Select(id => new GameMove(id)).ToArray());
        var perfect = await game.SubmitAsync(firstLevel.Id, ChallengeFixture.UserId,
            targetIds.Select(id => new GameMove(id)).ToArray());
        var after = await game.GetAsync(fixture.CourseId, ChallengeFixture.UserId);

        Assert.True(first.Passed);
        Assert.Equal(6, first.NewlyAwardedPoints);
        Assert.Equal(2, perfect.NewlyAwardedPoints);
        Assert.True(after!.Levels.Single(x => x.Number == 2).Unlocked);
    }

    [Fact]
    public async Task QuizRejectsFourthAttemptAfterThreeRecentFailures()
    {
        await using var fixture = await ChallengeFixture.CreateAsync(userAge: 6, courseGrade: 1);
        var quiz = fixture.CreateQuizService();
        var wrongAnswers = await fixture.GetQuizAnswersAsync(0);

        QuizSubmission? third = null;
        for (var attempt = 0; attempt < 3; attempt++)
            third = await quiz.SubmitAsync(fixture.CourseId, ChallengeFixture.UserId, wrongAnswers);

        Assert.True(third!.Lockout.IsLocked);
        await Assert.ThrowsAsync<ChallengeLockedException>(() =>
            quiz.SubmitAsync(fixture.CourseId, ChallengeFixture.UserId, wrongAnswers));
    }

    [Fact]
    public async Task GameRejectsFourthAttemptAfterThreeRecentFailures()
    {
        await using var fixture = await ChallengeFixture.CreateAsync(userAge: 6, courseGrade: 1);
        var game = fixture.CreateGameService();
        var level = Assert.Single((await game.GetAsync(fixture.CourseId, ChallengeFixture.UserId))!.Levels, x => x.Number == 1);
        var targetIds = await fixture.GetGameTargetIdsAsync(level.Id);
        var wrongMoves = level.Cards.Where(x => !targetIds.Contains(x.Id)).Select(x => new GameMove(x.Id)).ToArray();

        GameSubmission? third = null;
        for (var attempt = 0; attempt < 3; attempt++)
            third = await game.SubmitAsync(level.Id, ChallengeFixture.UserId, wrongMoves);

        Assert.True(third!.Lockout.IsLocked);
        await Assert.ThrowsAsync<ChallengeLockedException>(() =>
            game.SubmitAsync(level.Id, ChallengeFixture.UserId, wrongMoves));
    }

    [Fact]
    public async Task MatchingAndJigsawLevelsAreScoredByTheirOwnRules()
    {
        await using var fixture = await ChallengeFixture.CreateAsync(userAge: 6, courseGrade: 1);
        var game = fixture.CreateGameService();
        var initial = await game.GetAsync(fixture.CourseId, ChallengeFixture.UserId);
        var first = initial!.Levels.Single(x => x.Number == 1);
        var targets = await fixture.GetGameTargetIdsAsync(first.Id);
        await game.SubmitAsync(first.Id, ChallengeFixture.UserId, targets.Select(x => new GameMove(x)).ToArray());

        var matching = (await game.GetAsync(fixture.CourseId, ChallengeFixture.UserId))!.Levels.Single(x => x.Number == 2);
        var matchingMoves = matching.Cards.Where(x => x.IsPrompt).Select(prompt =>
        {
            var number = prompt.Text.Split(' ').Last();
            var answer = matching.Cards.Single(x => !x.IsPrompt && x.Text.EndsWith(number));
            return new GameMove(prompt.Id, answer.Id);
        }).ToArray();
        var matchingResult = await game.SubmitAsync(matching.Id, ChallengeFixture.UserId, matchingMoves);

        var jigsaw = (await game.GetAsync(fixture.CourseId, ChallengeFixture.UserId))!.Levels.Single(x => x.Number == 3);
        var jigsawMoves = jigsaw.Cards.Select(piece =>
            new GameMove(piece.Id, Position: int.Parse(piece.Text.Split(' ').Last()))).ToArray();
        var jigsawResult = await game.SubmitAsync(jigsaw.Id, ChallengeFixture.UserId, jigsawMoves);

        Assert.True(matchingResult.Passed);
        Assert.True(jigsawResult.Passed);
        Assert.Equal(100m, matchingResult.Percentage);
        Assert.Equal(100m, jigsawResult.Percentage);
    }

    private sealed class ChallengeFixture(SqliteConnection connection, TestFactory factory, int courseId) : IAsyncDisposable
    {
        public const string UserId = "challenge-user";
        public int CourseId { get; } = courseId;
        private TestFactory Factory { get; } = factory;

        public static async Task<ChallengeFixture> CreateAsync(int userAge, int courseGrade)
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            var factory = new TestFactory(new DbContextOptionsBuilder<LearnPlaneDbContext>().UseSqlite(connection).Options);
            await using var db = await factory.CreateDbContextAsync();
            await db.Database.EnsureCreatedAsync();
            db.Users.Add(new ApplicationUser { Id = UserId, UserName = "challenge", DisplayName = "Testelev", Age = userAge });
            var course = new Course
            {
                Grade = courseGrade,
                Subject = "Naturfag",
                Title = "Økosystemer",
                Summary = "Test",
                Content = "Test",
                Difficulty = CourseDifficulty.Lett,
                Questions = Enumerable.Range(1, 4).Select(number => new QuizQuestion
                {
                    SortOrder = number,
                    Text = $"Spørsmål {number}",
                    Options =
                    [
                        new AnswerOption { Text = "Riktig", IsCorrect = true, SortOrder = 1 },
                        new AnswerOption { Text = "Feil", SortOrder = 2 }
                    ]
                }).ToList()
            };
            var game = new CourseGame { Course = course, Title = "Begrepsjakt", Intro = "Test" };
            game.Levels =
            [
                new GameLevel
                {
                    LevelNumber = 1, Mode = GameLevelMode.CardSort, Title = "Nivå 1", Instructions = "Velg fire", MaxPoints = 8,
                    Cards = Enumerable.Range(1, 4).Select(x => new GameCard { Text = $"Riktig {x}", IsTarget = true, SortOrder = x })
                        .Concat(Enumerable.Range(1, 4).Select(x => new GameCard { Text = $"Feil {x}", SortOrder = x + 4 })).ToList()
                },
                new GameLevel
                {
                    LevelNumber = 2, Mode = GameLevelMode.Matching, Title = "Nivå 2", Instructions = "Koble fire", MaxPoints = 8,
                    Cards = Enumerable.Range(1, 4).SelectMany(x => new[]
                    {
                        new GameCard { Text = $"Spørsmål {x}", PairKey = $"par-{x}", IsTarget = true, SortOrder = x },
                        new GameCard { Text = $"Svar {x}", PairKey = $"par-{x}", SortOrder = x + 4 }
                    }).ToList()
                },
                new GameLevel
                {
                    LevelNumber = 3, Mode = GameLevelMode.Jigsaw, Title = "Nivå 3", Instructions = "Bygg fire", MaxPoints = 8,
                    Cards = Enumerable.Range(1, 4).Select(x => new GameCard
                        { Text = $"Brikke {x}", CorrectPosition = x, IsTarget = true, SortOrder = x }).ToList()
                }
            ];
            db.CourseGames.Add(game);
            await db.SaveChangesAsync();
            return new ChallengeFixture(connection, factory, course.Id);
        }

        public QuizService CreateQuizService() => new(Factory, new ScoreCalculator(), new ChallengePointCalculator(),
            new GradeEligibilityPolicy(), new AttemptLockoutPolicy());
        public CourseGameService CreateGameService() => new(Factory, new ChallengePointCalculator(),
            new GradeEligibilityPolicy(), new AttemptLockoutPolicy());

        public async Task<Dictionary<int, int>> GetQuizAnswersAsync(int correctAnswers)
        {
            await using var db = await Factory.CreateDbContextAsync();
            var questions = await db.QuizQuestions.Include(x => x.Options).OrderBy(x => x.SortOrder).ToListAsync();
            return questions.ToDictionary(x => x.Id, x => x.Options.Single(option => option.IsCorrect == (x.SortOrder <= correctAnswers)).Id);
        }

        public async Task<int[]> GetGameTargetIdsAsync(int levelId)
        {
            await using var db = await Factory.CreateDbContextAsync();
            return await db.GameCards.Where(x => x.GameLevelId == levelId && x.IsTarget).Select(x => x.Id).ToArrayAsync();
        }

        public async ValueTask DisposeAsync() => await connection.DisposeAsync();
    }

    private sealed class TestFactory(DbContextOptions<LearnPlaneDbContext> options) : IDbContextFactory<LearnPlaneDbContext>
    {
        public LearnPlaneDbContext CreateDbContext() => new(options);
        public Task<LearnPlaneDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default) => Task.FromResult(CreateDbContext());
    }
}
