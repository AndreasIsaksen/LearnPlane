using System.Data;
using LearnPlane.Web.Data;
using LearnPlane.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace LearnPlane.Web.Services;

public sealed class CourseGameService(
    IDbContextFactory<LearnPlaneDbContext> dbFactory,
    ChallengePointCalculator pointCalculator,
    GradeEligibilityPolicy gradePolicy,
    AttemptLockoutPolicy lockoutPolicy)
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
        var lockout = lockoutPolicy.Evaluate(attempts.Where(x => !x.Passed).Select(x => x.CompletedAtUtc), DateTime.UtcNow);
        var userAge = await db.Users.Where(x => x.Id == userId).Select(x => x.Age).SingleAsync();
        var currentGrade = userAge is null ? (int?)null : gradePolicy.GetCurrentGrade(userAge.Value);
        var canEarn = gradePolicy.CanEarnPoints(userAge, game.Course.Grade);
        var levels = game.Levels.OrderBy(x => x.LevelNumber).Select(level =>
        {
            var unlocked = level.LevelNumber == 1 || attempts.Any(x =>
                x.GameLevelId == game.Levels.Single(y => y.LevelNumber == level.LevelNumber - 1).Id && x.Passed);
            var levelAttempts = attempts.Where(x => x.GameLevelId == level.Id).ToList();
            var cards = ShuffleForPresentation(level);
            var requiredCount = level.Mode switch
            {
                GameLevelMode.CardSort => level.Cards.Count(x => x.IsTarget),
                GameLevelMode.Matching => level.Cards.Count(x => x.IsTarget),
                _ => level.Cards.Count
            };
            return new GameLevelView(level.Id, level.LevelNumber, level.Title, level.Instructions, level.MaxPoints,
                level.Mode, requiredCount, unlocked, levelAttempts.Any(x => x.Passed),
                levelAttempts.Sum(x => x.PointsAwarded), cards);
        }).ToList();
        return new GameView(game.Title, game.Intro, game.Course.Title, game.Course.Subject, game.Course.Grade,
            currentGrade, canEarn, lockout, levels);
    }

    public async Task<GameSubmission> SubmitAsync(int levelId, string userId, IReadOnlyCollection<GameMove> moves)
    {
        await using var strategyContext = await dbFactory.CreateDbContextAsync();
        var strategy = strategyContext.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(() => SubmitCoreAsync(levelId, userId, moves));
    }

    private async Task<GameSubmission> SubmitCoreAsync(int levelId, string userId, IReadOnlyCollection<GameMove> moves)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var level = await db.GameLevels
            .Include(x => x.Cards)
            .Include(x => x.CourseGame).ThenInclude(x => x.Course)
            .SingleOrDefaultAsync(x => x.Id == levelId && x.CourseGame.Course.IsPublished)
            ?? throw new InvalidOperationException("Spillnivået finnes ikke.");

        var nowUtc = DateTime.UtcNow;
        var recentFailures = await db.GameLevelAttempts
            .Where(x => x.UserId == userId && x.Level.CourseGameId == level.CourseGameId && !x.Passed)
            .OrderByDescending(x => x.CompletedAtUtc)
            .Select(x => x.CompletedAtUtc)
            .Take(AttemptLockoutPolicy.FailureThreshold)
            .ToListAsync();
        var currentLockout = lockoutPolicy.Evaluate(recentFailures, nowUtc);
        if (currentLockout.IsLocked)
            throw new ChallengeLockedException(currentLockout);

        if (level.LevelNumber > 1)
        {
            var previousLevelId = await db.GameLevels
                .Where(x => x.CourseGameId == level.CourseGameId && x.LevelNumber == level.LevelNumber - 1)
                .Select(x => x.Id).SingleAsync();
            var unlocked = await db.GameLevelAttempts
                .AnyAsync(x => x.UserId == userId && x.GameLevelId == previousLevelId && x.Passed);
            if (!unlocked) throw new InvalidOperationException("Fullfør forrige nivå før du fortsetter.");
        }

        var (correct, total) = EvaluateMoves(level, moves);
        var percentage = Math.Round(correct * 100m / total, 2);
        var passed = percentage >= ChallengePointCalculator.PassThreshold;
        var previouslyAwarded = await db.GameLevelAttempts
            .Where(x => x.UserId == userId && x.GameLevelId == levelId)
            .SumAsync(x => (int?)x.PointsAwarded) ?? 0;
        var userAge = await db.Users.Where(x => x.Id == userId).Select(x => x.Age).SingleAsync();
        var canEarn = gradePolicy.CanEarnPoints(userAge, level.CourseGame.Course.Grade);
        var awarded = canEarn ? pointCalculator.CalculateNewAward(percentage, level.MaxPoints, previouslyAwarded) : 0;

        db.GameLevelAttempts.Add(new GameLevelAttempt
        {
            UserId = userId, GameLevelId = levelId, CorrectSelections = correct,
            IncorrectSelections = total - correct, TotalTargets = total, Percentage = percentage,
            Passed = passed, PointsAwarded = awarded, CompletedAtUtc = nowUtc
        });
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        var lockout = passed
            ? ChallengeLockoutStatus.Unlocked
            : lockoutPolicy.Evaluate(recentFailures.Prepend(nowUtc), nowUtc);
        return new GameSubmission(level.LevelNumber, correct, total, percentage, passed, awarded,
            level.MaxPoints, canEarn, lockout);
    }

    private static (int Correct, int Total) EvaluateMoves(GameLevel level, IReadOnlyCollection<GameMove> moves)
    {
        var cardsById = level.Cards.ToDictionary(x => x.Id);
        if (moves.Any(x => !cardsById.ContainsKey(x.CardId)))
            throw new InvalidOperationException("Spillebrettet inneholder et ugyldig kort. Last siden på nytt.");

        return level.Mode switch
        {
            GameLevelMode.CardSort => EvaluateCardSort(level, moves, cardsById),
            GameLevelMode.Matching => EvaluateMatching(level, moves, cardsById),
            GameLevelMode.Jigsaw => EvaluateJigsaw(level, moves, cardsById),
            _ => throw new InvalidOperationException("Ukjent spilltype.")
        };
    }

    private static (int, int) EvaluateCardSort(GameLevel level, IReadOnlyCollection<GameMove> moves,
        IReadOnlyDictionary<int, GameCard> cardsById)
    {
        var targetCount = level.Cards.Count(x => x.IsTarget);
        var selected = moves.Select(x => x.CardId).Distinct().ToArray();
        if (selected.Length != targetCount || moves.Count != targetCount)
            throw new InvalidOperationException($"Velg nøyaktig {targetCount} kort før du leverer.");
        return (selected.Count(id => cardsById[id].IsTarget), targetCount);
    }

    private static (int, int) EvaluateMatching(GameLevel level, IReadOnlyCollection<GameMove> moves,
        IReadOnlyDictionary<int, GameCard> cardsById)
    {
        var prompts = level.Cards.Where(x => x.IsTarget).ToArray();
        var answers = level.Cards.Where(x => !x.IsTarget).ToDictionary(x => x.Id);
        var relatedIds = moves.Select(x => x.RelatedCardId).ToArray();
        if (moves.Count != prompts.Length || moves.Select(x => x.CardId).Distinct().Count() != prompts.Length ||
            relatedIds.Any(x => x is null || !answers.ContainsKey(x.Value)) || relatedIds.Distinct().Count() != prompts.Length ||
            moves.Any(x => !cardsById[x.CardId].IsTarget))
            throw new InvalidOperationException("Koble hvert kort til venstre med ett unikt svar til høyre.");
        var correct = moves.Count(move => cardsById[move.CardId].PairKey == answers[move.RelatedCardId!.Value].PairKey);
        return (correct, prompts.Length);
    }

    private static (int, int) EvaluateJigsaw(GameLevel level, IReadOnlyCollection<GameMove> moves,
        IReadOnlyDictionary<int, GameCard> cardsById)
    {
        if (moves.Count != level.Cards.Count || moves.Select(x => x.CardId).Distinct().Count() != level.Cards.Count ||
            moves.Any(x => x.Position is null or < 1 || x.Position > level.Cards.Count) ||
            moves.Select(x => x.Position).Distinct().Count() != level.Cards.Count)
            throw new InvalidOperationException("Plasser alle puslespillbrikkene én gang før du leverer.");
        var correct = moves.Count(move => cardsById[move.CardId].CorrectPosition == move.Position);
        return (correct, level.Cards.Count);
    }

    private static IReadOnlyList<GameCardView> ShuffleForPresentation(GameLevel level)
    {
        IEnumerable<GameCard> cards = level.Mode == GameLevelMode.Matching
            ? level.Cards.Where(x => x.IsTarget).OrderBy(_ => Random.Shared.Next())
                .Concat(level.Cards.Where(x => !x.IsTarget).OrderBy(_ => Random.Shared.Next()))
            : level.Cards.OrderBy(_ => Random.Shared.Next());
        return cards.Select(x => new GameCardView(x.Id, x.Text, x.IsTarget, x.VisualCue)).ToList();
    }
}

public sealed record GameMove(int CardId, int? RelatedCardId = null, int? Position = null);
public sealed record GameView(string Title, string Intro, string CourseTitle, string Subject, int Grade,
    int? CurrentGrade, bool CanEarnPoints, ChallengeLockoutStatus Lockout, IReadOnlyList<GameLevelView> Levels);
public sealed record GameLevelView(int Id, int Number, string Title, string Instructions, int MaxPoints,
    GameLevelMode Mode, int RequiredCount, bool Unlocked, bool Passed, int EarnedPoints,
    IReadOnlyList<GameCardView> Cards);
public sealed record GameCardView(int Id, string Text, bool IsPrompt, string VisualCue);
public sealed record GameSubmission(int LevelNumber, int Correct, int Total, decimal Percentage, bool Passed,
    int NewlyAwardedPoints, int MaxPoints, bool CanEarnPoints, ChallengeLockoutStatus Lockout);
