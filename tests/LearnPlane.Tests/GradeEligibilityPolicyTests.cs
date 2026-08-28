using LearnPlane.Web.Services;
using Xunit;

namespace LearnPlane.Tests;

public sealed class GradeEligibilityPolicyTests
{
    private readonly GradeEligibilityPolicy _policy = new();

    [Theory]
    [InlineData(6, 1)]
    [InlineData(10, 5)]
    [InlineData(15, 10)]
    [InlineData(18, 10)]
    public void MapsNorwegianSchoolAgeToGrade(int age, int expectedGrade)
        => Assert.Equal(expectedGrade, _policy.GetCurrentGrade(age));

    [Fact]
    public void CourseBelowCurrentGradeDoesNotEarnPoints()
        => Assert.False(_policy.CanEarnPoints(age: 12, courseGrade: 6));

    [Fact]
    public void CurrentAndHigherGradesEarnPoints()
    {
        Assert.True(_policy.CanEarnPoints(age: 12, courseGrade: 7));
        Assert.True(_policy.CanEarnPoints(age: 12, courseGrade: 10));
    }

    [Fact]
    public void ExistingUserWithoutAgeRemainsEligible()
        => Assert.True(_policy.CanEarnPoints(age: null, courseGrade: 1));
}
