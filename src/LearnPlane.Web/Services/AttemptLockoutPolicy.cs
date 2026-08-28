namespace LearnPlane.Web.Services;

public sealed class AttemptLockoutPolicy
{
    public const int FailureThreshold = 3;
    public static readonly TimeSpan FailureWindow = TimeSpan.FromMinutes(30);
    public static readonly TimeSpan LockDuration = TimeSpan.FromMinutes(30);

    public ChallengeLockoutStatus Evaluate(IEnumerable<DateTime> failedAttemptsUtc, DateTime nowUtc)
    {
        var failures = failedAttemptsUtc.OrderByDescending(x => x).Take(FailureThreshold).ToArray();
        if (failures.Length < FailureThreshold || failures[0] - failures[^1] > FailureWindow)
            return ChallengeLockoutStatus.Unlocked;

        var lockedUntilUtc = failures[0].Add(LockDuration);
        return lockedUntilUtc > nowUtc
            ? new ChallengeLockoutStatus(true, lockedUntilUtc)
            : ChallengeLockoutStatus.Unlocked;
    }
}

public sealed record ChallengeLockoutStatus(bool IsLocked, DateTime? LockedUntilUtc)
{
    public static ChallengeLockoutStatus Unlocked { get; } = new(false, null);
    public int RemainingMinutes => !IsLocked || LockedUntilUtc is null
        ? 0
        : Math.Max(1, (int)Math.Ceiling((LockedUntilUtc.Value - DateTime.UtcNow).TotalMinutes));
}

public sealed class ChallengeLockedException(ChallengeLockoutStatus lockout)
    : InvalidOperationException($"Utfordringen er låst i omtrent {lockout.RemainingMinutes} minutter etter tre mislykkede forsøk.")
{
    public ChallengeLockoutStatus Lockout { get; } = lockout;
}
