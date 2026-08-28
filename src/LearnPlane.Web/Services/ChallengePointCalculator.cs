namespace LearnPlane.Web.Services;

public sealed class ChallengePointCalculator
{
    public const decimal PassThreshold = 70m;

    public int CalculateNewAward(decimal percentage, int maxPoints, int previouslyAwarded)
    {
        if (percentage is < 0 or > 100) throw new ArgumentOutOfRangeException(nameof(percentage));
        if (maxPoints < 1) throw new ArgumentOutOfRangeException(nameof(maxPoints));
        if (previouslyAwarded < 0) throw new ArgumentOutOfRangeException(nameof(previouslyAwarded));

        var earnedTarget = percentage switch
        {
            100m => maxPoints,
            >= PassThreshold => (int)Math.Round(maxPoints * 0.75m, MidpointRounding.AwayFromZero),
            _ => 0
        };
        return Math.Max(0, earnedTarget - Math.Min(previouslyAwarded, maxPoints));
    }
}
