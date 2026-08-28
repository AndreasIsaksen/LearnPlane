using LearnPlane.Web.Data;
using LearnPlane.Web.Models;
using Xunit;

namespace LearnPlane.Tests;

public sealed class GameCatalogTests
{
    [Theory]
    [InlineData(2, CourseDifficulty.Lett, "Kortskogen", 8)]
    [InlineData(6, CourseDifficulty.Middels, "Kortjakten", 14)]
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
        Assert.Equal([GameLevelMode.CardSort, GameLevelMode.Matching, GameLevelMode.Jigsaw], levels.Select(x => x.Mode));
        Assert.Equal([6, 10, 5], levels.Select(x => x.Cards.Count));
        Assert.Equal(3, levels[0].Cards.Count(x => x.IsTarget));
        Assert.Equal(5, levels[1].Cards.Count(x => x.IsTarget));
        Assert.Equal(5, levels[2].Cards.Count(x => x.CorrectPosition is not null));
        Assert.All(levels, level => Assert.InRange(level.MaxPoints, 1, 20));
        Assert.StartsWith("Fagoppdrag:", game.Title);
        Assert.Contains("Tre ulike oppdrag", game.Intro);
        Assert.Contains(levels[0].Cards, x => x.IsTarget && x.Text == "art");
        Assert.All(levels[1].Cards, x => Assert.NotNull(x.PairKey));
        Assert.Equal(Enumerable.Range(1, 5), levels[2].Cards.Select(x => x.CorrectPosition!.Value));
    }
}
