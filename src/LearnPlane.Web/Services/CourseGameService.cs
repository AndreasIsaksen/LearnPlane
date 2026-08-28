using System.Data;
using LearnPlane.Web.Data;
using LearnPlane.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace LearnPlane.Web.Services;

public sealed class CourseGameService(
    IDbContextFactory<LearnPlaneDbContext> dbFactory,
    ChallengePointCalculator pointCalculator,
    GradeEligibilityPolicy gradePolicy)
{
    public async Task<GameView?> GetAsync(int courseId, string userId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var game = await db.CourseGames.AsNoTracking()
            .Include(x => x.Course)
            .Include(x => x.Levels).ThenInclude(x => x.Cards)
            .SingleOrDefaultAsync(x => x.CourseId == courseId && x.Course.IsPublished);
        if (game is null) return null;

        var attempts = await db.GameLevelAttempts.AsNoTracking()
            .Where(x => x.UserId == userId && x.Level.CourseGameId == game.Id)
            .ToListAsync();
        var userAge = await db.Users.Where(x => x.Id == userId).Select(x => x.Age).SingleAsync();
        var currentGrade = userAge is null ? (int?)null : gradePolicy.GetCurrentGrade(userAge.Value);
        var canEarn = gradePolicy.CanEarnPoints(userAge, game.Course.Grade);
        var levels = game.Levels.OrderBy(x => x.LevelNumber).Select(level =>
        {
            var unlocked = level.LevelNumber == 1 || attempts.Any(x => x.GameLevelId == game.Levels.Single(y => y.LevelNumber == level.LevelNumber - 1).Id && x.Passed);
            var levelAttempts = attempts.Where(x => x.GameLevelId == level.Id).ToList();
            return new GameLevelView(level.Id, level.LevelNumber, level.Title, level.Instructions, level.MaxPoints,
                unlocked, levelAttempts.Any(x => x.Passed), levelAttempts.Sum(x => x.PointsAwarded),
                level.Cards.OrderBy(_ => Random.Shared.Next()).Select(x => new GameCardView(x.Id, x.Text)).ToList());
        }).ToList();
        return new GameView(game.Title, game.Intro, game.Course.Title, game.Course.Subject, game.Course.Grade, currentGrade, canEarn, levels);
    }

    public async Task<GameSubmission> SubmitAsync(int levelId, string userId, IReadOnlyCollection<int> selectedCardIds)
    {
        await using var strategyContext = await dbFactory.CreateDbContextAsync();
        var strategy = strategyContext.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(() => SubmitCoreAsync(levelId, userId, selectedCardIds));
    }

    private async Task<GameSubmission> SubmitCoreAsync(int levelId, string userId, IReadOnlyCollection<int> selectedCardIds)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var level = await db.GameLevels
            .Include(x => x.Cards)
            .Include(x => x.CourseGame).ThenInclude(x => x.Course)
            .SingleOrDefaultAsync(x => x.Id == levelId && x.CourseGame.Course.IsPublished)
            ?? throw new InvalidOperationException("Spillnivået finnes ikke.");
        var targetCount = level.Cards.Count(x => x.IsTarget);
        var selections = selectedCardIds.Distinct().ToHashSet();
        if (selections.Count != targetCount || selections.Any(id => level.Cards.All(x => x.Id != id)))
            throw new InvalidOperationException($"Velg nøyaktig {targetCount} kort før du leverer.");

        if (level.LevelNumber > 1)
        {
            var previousLevelId = await db.GameLevels.Where(x => x.CourseGameId == level.CourseGameId && x.LevelNumber == level.LevelNumber - 1)
                .Select(x => x.Id).SingleAsync();
            var unlocked = await db.GameLevelAttempts.AnyAsync(x => x.UserId == userId && x.GameLevelId == previousLevelId && x.Passed);
            if (!unlocked) throw new InvalidOperationException("Fullfør forrige nivå før du fortsetter.");
        }

        var correct = level.Cards.Count(x => x.IsTarget && selections.Contains(x.Id));
        var percentage = Math.Round(correct * 100m / targetCount, 2);
        var passed = percentage >= ChallengePointCalculator.PassThreshold;
        var previouslyAwarded = await db.GameLevelAttempts.Where(x => x.UserId == userId && x.GameLevelId == levelId)
            .SumAsync(x => (int?)x.PointsAwarded) ?? 0;
        var userAge = await db.Users.Where(x => x.Id == userId).Select(x => x.Age).SingleAsync();
        var canEarn = gradePolicy.CanEarnPoints(userAge, level.CourseGame.Course.Grade);
        var awarded = canEarn ? pointCalculator.CalculateNewAward(percentage, level.MaxPoints, previouslyAwarded) : 0;

        db.GameLevelAttempts.Add(new GameLevelAttempt
        {
            UserId = userId,
            GameLevelId = levelId,
            CorrectSelections = correct,
            IncorrectSelections = targetCount - correct,
            TotalTargets = targetCount,
            Percentage = percentage,
            Passed = passed,
            PointsAwarded = awarded
        });
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return new GameSubmission(level.LevelNumber, correct, targetCount, percentage, passed, awarded, level.MaxPoints, canEarn);
    }
}

public sealed record GameView(string Title, string Intro, string CourseTitle, string Subject, int Grade,
    int? CurrentGrade, bool CanEarnPoints, IReadOnlyList<GameLevelView> Levels);
public sealed record GameLevelView(int Id, int Number, string Title, string Instructions, int MaxPoints,
    bool Unlocked, bool Passed, int EarnedPoints, IReadOnlyList<GameCardView> Cards);
public sealed record GameCardView(int Id, string Text);
public sealed record GameSubmission(int LevelNumber, int Correct, int Total, decimal Percentage, bool Passed,
    int NewlyAwardedPoints, int MaxPoints, bool CanEarnPoints);
