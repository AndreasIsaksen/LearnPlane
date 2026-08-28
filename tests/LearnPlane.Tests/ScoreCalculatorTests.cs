using LearnPlane.Web.Models;
using LearnPlane.Web.Services;
using Xunit;

namespace LearnPlane.Tests;

public sealed class ScoreCalculatorTests
{
    private readonly ScoreCalculator _calculator = new();

    [Theory]
    [InlineData(2, 3, false)]
    [InlineData(7, 10, true)]
    [InlineData(3, 4, true)]
    public void SeventyPercentIsRequired(int correct, int total, bool expectedPassed)
        => Assert.Equal(expectedPassed, _calculator.Calculate(correct, total, CourseDifficulty.Lett).Passed);

    [Theory]
    [InlineData(CourseDifficulty.Lett, 10)]
    [InlineData(CourseDifficulty.Middels, 20)]
    [InlineData(CourseDifficulty.Utfordrende, 30)]
    public void PassedQuizAwardsPointsByDifficulty(CourseDifficulty difficulty, int expectedPoints)
        => Assert.Equal(expectedPoints, _calculator.Calculate(4, 4, difficulty).AvailablePoints);

    [Fact]
    public void FailedQuizAwardsNoPoints()
        => Assert.Equal(0, _calculator.Calculate(1, 4, CourseDifficulty.Utfordrende).AvailablePoints);
}
