using LearnPlane.Web.Data;
using LearnPlane.Web.Models;
using Xunit;

namespace LearnPlane.Tests;

public sealed class GameCatalogTests
{
    [Theory]
    [InlineData(2, CourseDifficulty.Lett, "Ordskogen", 8)]
    [InlineData(6, CourseDifficulty.Middels, "Begrepsjakten", 14)]
    [InlineData(9, CourseDifficulty.Utfordrende, "Fagduellen", 20)]
    public void CreatesThreeProgressiveAgeAdjustedLevels(int grade, CourseDifficulty difficulty, string firstTitle, int finalPoints)
    {
        var game = GameCatalog.CreateGame(new Course
        {
            Id = grade,
            Grade = grade,
            Subject = "Naturfag",
            Title = "Dyr, planter og økosystemer",
            Summary = "Test",
            Content = "Test",
            Difficulty = difficulty
        });

        var levels = game.Levels.OrderBy(x => x.LevelNumber).ToArray();
        Assert.Equal(3, levels.Length);
        Assert.Equal(firstTitle, levels[0].Title);
        Assert.Equal(finalPoints, levels[2].MaxPoints);
        Assert.Equal([6, 10, 14], levels.Select(x => x.Cards.Count));
        Assert.All(levels, level => Assert.Equal(level.Cards.Count / 2, level.Cards.Count(x => x.IsTarget)));
        Assert.All(levels, level => Assert.InRange(level.MaxPoints, 1, 20));
    }
}
