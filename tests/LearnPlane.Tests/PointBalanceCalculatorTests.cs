using LearnPlane.Web.Services;
using Xunit;

namespace LearnPlane.Tests;

public sealed class PointBalanceCalculatorTests
{
    private readonly PointBalanceCalculator _calculator = new();

    [Theory]
    [InlineData(100, 30, 70)]
    [InlineData(0, 0, 0)]
    [InlineData(20, 50, 0)]
    public void BalanceIsEarnedMinusSpentWithoutGoingNegative(int earned, int spent, int expected)
        => Assert.Equal(expected, _calculator.Calculate(earned, spent));

    [Theory]
    [InlineData(50, 50, true)]
    [InlineData(49, 50, false)]
    [InlineData(100, 0, false)]
    public void AffordabilityRequiresPositiveTotalWithinBalance(int balance, int total, bool expected)
        => Assert.Equal(expected, _calculator.CanAfford(balance, total));
}
