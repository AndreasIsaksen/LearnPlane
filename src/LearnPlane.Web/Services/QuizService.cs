using System.Data;
using LearnPlane.Web.Data;
using LearnPlane.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace LearnPlane.Web.Services;

public sealed class QuizService(
    IDbContextFactory<LearnPlaneDbContext> dbFactory,
    ScoreCalculator calculator,
    ChallengePointCalculator pointCalculator,
    GradeEligibilityPolicy gradePolicy)
{
    public async Task<QuizSubmission> SubmitAsync(int courseId, string userId, IReadOnlyDictionary<int, int> answers)
    {
        await using var strategyContext = await dbFactory.CreateDbContextAsync();
        var strategy = strategyContext.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(() => SubmitCoreAsync(courseId, userId, answers));
    }

    private async Task<QuizSubmission> SubmitCoreAsync(int courseId, string userId, IReadOnlyDictionary<int, int> answers)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var course = await db.Courses.AsNoTracking()
            .Include(x => x.Questions).ThenInclude(x => x.Options)
            .SingleOrDefaultAsync(x => x.Id == courseId && x.IsPublished)
            ?? throw new InvalidOperationException("Kurset finnes ikke.");

        var correct = course.Questions.Count(question =>
            answers.TryGetValue(question.Id, out var optionId) &&
            question.Options.Any(option => option.Id == optionId && option.IsCorrect));
        var score = calculator.Calculate(correct, course.Questions.Count, course.Difficulty);

        var previouslyAwarded = await db.QuizAttempts
            .Where(x => x.UserId == userId && x.CourseId == courseId)
            .SumAsync(x => (int?)x.PointsAwarded) ?? 0;
        var userAge = await db.Users.Where(x => x.Id == userId).Select(x => x.Age).SingleAsync();
        var canEarn = gradePolicy.CanEarnPoints(userAge, course.Grade);
        var maxPoints = (int)course.Difficulty * 10;
        var awarded = canEarn ? pointCalculator.CalculateNewAward(score.Percentage, maxPoints, previouslyAwarded) : 0;

        db.QuizAttempts.Add(new QuizAttempt
        {
            UserId = userId,
            CourseId = courseId,
            CorrectAnswers = score.CorrectAnswers,
            TotalQuestions = score.TotalQuestions,
            Percentage = score.Percentage,
            Passed = score.Passed,
            PointsAwarded = awarded
        });
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return new QuizSubmission(score, awarded, maxPoints, canEarn,
            userAge is null ? null : gradePolicy.GetCurrentGrade(userAge.Value));
    }
}

public sealed record QuizSubmission(QuizScore Score, int NewlyAwardedPoints, int MaxPoints, bool CanEarnPoints, int? CurrentGrade);
