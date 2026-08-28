namespace LearnPlane.Web.Services;

public sealed class GradeEligibilityPolicy
{
    public const int MinimumAge = 6;
    public const int MaximumAge = 18;

    public int GetCurrentGrade(int age)
    {
        if (age is < MinimumAge or > MaximumAge)
            throw new ArgumentOutOfRangeException(nameof(age));

        return Math.Clamp(age - 5, 1, 10);
    }

    public bool CanEarnPoints(int? age, int courseGrade) =>
        age is null || courseGrade >= GetCurrentGrade(age.Value);
}
