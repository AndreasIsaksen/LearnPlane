using LearnPlane.Web.Data;
using LearnPlane.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace LearnPlane.Web.Services;

public sealed class QuizService(IDbContextFactory<LearnPlaneDbContext> dbFactory, ScoreCalculator calculator)
{
    public async Task<QuizSubmission> SubmitAsync(int courseId, string userId, IReadOnlyDictionary<int, int> answers)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var course = await db.Courses.AsNoTracking()
            .Include(x => x.Questions).ThenInclude(x => x.Options)
            .SingleOrDefaultAsync(x => x.Id == courseId && x.IsPublished)
            ?? throw new InvalidOperationException("Kurset finnes ikke.");

        var correct = course.Questions.Count(question =>
            answers.TryGetValue(question.Id, out var optionId) &&
            question.Options.Any(option => option.Id == optionId && option.IsCorrect));
        var score = calculator.Calculate(correct, course.Questions.Count, course.Difficulty);

        var previousBest = await db.QuizAttempts
            .Where(x => x.UserId == userId && x.CourseId == courseId)
            .MaxAsync(x => (int?)x.PointsAwarded) ?? 0;
        var awarded = Math.Max(0, score.AvailablePoints - previousBest);

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
        return new QuizSubmission(score, awarded);
    }
}

public sealed record QuizSubmission(QuizScore Score, int NewlyAwardedPoints);
