namespace LearnPlane.Web.Services;

public sealed class PointBalanceCalculator
{
    public int Calculate(int earnedPoints, int spentPoints)
    {
        if (earnedPoints < 0) throw new ArgumentOutOfRangeException(nameof(earnedPoints));
        if (spentPoints < 0) throw new ArgumentOutOfRangeException(nameof(spentPoints));
        return Math.Max(0, earnedPoints - spentPoints);
    }

    public bool CanAfford(int balance, int total) => balance >= 0 && total > 0 && balance >= total;
}
