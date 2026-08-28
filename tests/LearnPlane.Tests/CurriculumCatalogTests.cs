using LearnPlane.Web.Data;
using LearnPlane.Web.Models;
using Xunit;

namespace LearnPlane.Tests;

public sealed class CurriculumCatalogTests
{
    private readonly List<Course> _courses = CurriculumCatalog.CreateCourses().ToList();

    [Fact]
    public void BuildsCompleteCatalogForGradesAndSubjects()
    {
        Assert.Equal(224, _courses.Count);
        Assert.Equal(Enumerable.Range(1, 10), _courses.Select(x => x.Grade).Distinct().Order());
        Assert.All(_courses, course => Assert.Equal(2,
            _courses.Count(x => x.Grade == course.Grade && x.Subject == course.Subject)));
    }

    [Fact]
    public void EveryCourseContainsStructuredSubstantialAcademicContent()
    {
        Assert.All(_courses, course =>
        {
            Assert.Contains($"data-content-version=\"{CurriculumCatalog.ContentVersion}\"", course.Content);
            Assert.Equal(CurriculumCatalog.ContentVersion, course.CatalogVersion);
            Assert.True(course.Content.Length >= 1_500, $"{course.Grade}. {course.Subject}/{course.Title} er for kort.");
            Assert.Equal(6, Count(course.Content, "<h2>"));
            Assert.Contains("Gjennomarbeidet eksempel", course.Content);
            Assert.Contains("Vanlig misforståelse", course.Content);
            Assert.Contains("LK20", course.Content);
            Assert.True(course.Summary.Length <= 350);
        });
    }

    [Fact]
    public void QuizzesUseValidVariedSubjectQuestions()
    {
        var allQuestionTexts = new List<string>();
        Assert.All(_courses, course =>
        {
            Assert.Equal(4, course.Questions.Count);
            Assert.DoesNotContain(course.Questions, x => x.Text.Contains("Hva er hovedtemaet i dette kurset?"));
            Assert.DoesNotContain(course.Questions, x => x.Text.Contains("Hva bør du gjøre hvis du svarer feil"));
            Assert.All(course.Questions, question =>
            {
                Assert.InRange(question.Text.Length, 10, 500);
                Assert.InRange(question.Explanation.Length, 10, 500);
                Assert.Equal(4, question.Options.Count);
                Assert.Single(question.Options, x => x.IsCorrect);
                Assert.Equal(4, question.Options.Select(x => x.Text).Distinct().Count());
                Assert.All(question.Options, option => Assert.InRange(option.Text.Length, 1, 300));
                allQuestionTexts.Add(question.Text);
            });
        });
        Assert.Equal(allQuestionTexts.Count, allQuestionTexts.Distinct().Count());
        Assert.Equal(new[] { 1, 2, 3, 4 }, _courses.SelectMany(x => x.Questions)
            .SelectMany(x => x.Options.Where(y => y.IsCorrect)).Select(x => x.SortOrder).Distinct().Order());
    }

    [Fact]
    public void MathematicsAndEnglishContainAuthenticTaskTypes()
    {
        var mathematics = _courses.Where(x => x.Subject == "Matematikk").SelectMany(x => x.Questions).ToList();
        Assert.Contains(mathematics, x => x.Text.Contains("Hva er") && x.Text.Any(char.IsDigit));
        Assert.Contains(mathematics, x => x.Explanation.Contains("Omkretsen") || x.Explanation.Contains("Arealet"));
        Assert.Contains(mathematics, x => x.Text.Contains("Løs") && x.Text.Contains("x"));
        Assert.Contains(mathematics, x => x.Text.Contains("sannsynligheten"));

        var english = _courses.Where(x => x.Subject == "Engelsk").SelectMany(x => x.Questions).ToList();
        Assert.Contains(english, x => x.Text.Contains("Yesterday"));
        Assert.Contains(english, x => x.Text.Contains("Which sentence"));
        Assert.Contains(english, x => x.Text.Contains("Read:"));
    }

    [Fact]
    public void EveryCourseGetsAValidTopicSpecificProgressiveGame()
    {
        Assert.All(_courses, course =>
        {
            var game = GameCatalog.CreateGame(course);
            Assert.StartsWith("Fagoppdrag:", game.Title);
            Assert.Contains(course.Title, game.Title);
            Assert.Equal(3, game.Levels.Count);
            Assert.Equal(new[] { 6, 10, 14 }, game.Levels.OrderBy(x => x.LevelNumber).Select(x => x.Cards.Count));
            Assert.All(game.Levels, level =>
            {
                Assert.Equal(level.Cards.Count / 2, level.Cards.Count(x => x.IsTarget));
                Assert.Equal(level.Cards.Count, level.Cards.Select(x => x.Text).Distinct(StringComparer.OrdinalIgnoreCase).Count());
                Assert.All(level.Cards, card => Assert.InRange(card.Text.Length, 1, 100));
                Assert.InRange(level.Instructions.Length, 10, 500);
            });
        });
    }

    private static int Count(string value, string fragment) =>
        (value.Length - value.Replace(fragment, string.Empty).Length) / fragment.Length;
}
