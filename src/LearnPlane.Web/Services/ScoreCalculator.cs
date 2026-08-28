using LearnPlane.Web.Models;

namespace LearnPlane.Web.Services;

public sealed class ScoreCalculator
{
    public const decimal PassThreshold = 70m;

    public QuizScore Calculate(int correctAnswers, int totalQuestions, CourseDifficulty difficulty)
    {
        if (totalQuestions <= 0) throw new ArgumentOutOfRangeException(nameof(totalQuestions));
        if (correctAnswers < 0 || correctAnswers > totalQuestions)
            throw new ArgumentOutOfRangeException(nameof(correctAnswers));

        var percentage = Math.Round(correctAnswers * 100m / totalQuestions, 2);
        var passed = percentage >= PassThreshold;
        var availablePoints = difficulty switch
        {
            CourseDifficulty.Lett => 10,
            CourseDifficulty.Middels => 20,
            CourseDifficulty.Utfordrende => 30,
            _ => 10
        };
        return new QuizScore(correctAnswers, totalQuestions, percentage, passed, passed ? availablePoints : 0);
    }
}

public sealed record QuizScore(int CorrectAnswers, int TotalQuestions, decimal Percentage, bool Passed, int AvailablePoints);
