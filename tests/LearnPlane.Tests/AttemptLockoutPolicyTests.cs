using LearnPlane.Web.Services;
using Xunit;

namespace LearnPlane.Tests;

public sealed class AttemptLockoutPolicyTests
{
    private readonly AttemptLockoutPolicy _policy = new();

    [Fact]
    public void LocksForThirtyMinutesAfterThirdFailureWithinWindow()
    {
        var now = new DateTime(2026, 8, 28, 12, 0, 0, DateTimeKind.Utc);

        var result = _policy.Evaluate([now.AddMinutes(-20), now.AddMinutes(-8), now], now);

        Assert.True(result.IsLocked);
        Assert.Equal(now.AddMinutes(30), result.LockedUntilUtc);
    }

    [Fact]
    public void DoesNotLockWhenThreeFailuresSpanMoreThanThirtyMinutes()
    {
        var now = new DateTime(2026, 8, 28, 12, 0, 0, DateTimeKind.Utc);

        var result = _policy.Evaluate([now.AddMinutes(-31), now.AddMinutes(-8), now], now);

        Assert.False(result.IsLocked);
    }

    [Fact]
    public void UnlocksThirtyMinutesAfterTheTriggeringFailure()
    {
        var trigger = new DateTime(2026, 8, 28, 12, 0, 0, DateTimeKind.Utc);

        var result = _policy.Evaluate([trigger.AddMinutes(-20), trigger.AddMinutes(-8), trigger], trigger.AddMinutes(30));

        Assert.False(result.IsLocked);
    }
}
