using LearnPlane.Web.Services;
using Xunit;

namespace LearnPlane.Tests;

public sealed class ChallengePointCalculatorTests
{
    private readonly ChallengePointCalculator _calculator = new();

    [Theory]
    [InlineData(69, 20, 0, 0)]
    [InlineData(70, 20, 0, 15)]
    [InlineData(99, 20, 0, 15)]
    [InlineData(100, 20, 0, 20)]
    [InlineData(100, 20, 15, 5)]
    [InlineData(100, 20, 20, 0)]
    [InlineData(75, 20, 15, 0)]
    public void AwardsOnlyTheUnclaimedThreshold(decimal percentage, int maxPoints, int previous, int expected)
        => Assert.Equal(expected, _calculator.CalculateNewAward(percentage, maxPoints, previous));

    [Theory]
    [InlineData(10, 8)]
    [InlineData(14, 11)]
    [InlineData(30, 23)]
    public void ThreeQuartersRoundsToAWholePoint(int maxPoints, int expected)
        => Assert.Equal(expected, _calculator.CalculateNewAward(70, maxPoints, 0));
}
