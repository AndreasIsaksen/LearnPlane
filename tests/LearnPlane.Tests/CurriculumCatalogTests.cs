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
            Assert.Contains("data-grade-band=", course.Content);
            Assert.Contains("data-visual-level=", course.Content);
            Assert.Contains("data-visual-aid=", course.Content);
            Assert.Contains("LK20", course.Content);
            Assert.True(course.Summary.Length <= 350);
        });
    }

    [Fact]
    public void VisualSupportAndLanguageProgressWithGradeLevel()
    {
        Assert.All(_courses, course =>
        {
            var expectedVisualLevel = course.Grade == 1 ? 4 : course.Grade == 2 ? 3 : course.Grade <= 4 ? 2 : 1;
            var expectedAidCount = course.Grade <= 2 ? 3 : course.Grade <= 4 ? 2 : 1;
            Assert.Contains($"data-visual-level=\"{expectedVisualLevel}\"", course.Content);
            Assert.Equal(expectedAidCount, Count(course.Content, "data-visual-aid="));
            Assert.Contains("role=\"img\"", course.Content);
            Assert.Contains("<title", course.Content);
        });

        var firstGrade = _courses.Where(x => x.Grade == 1).ToList();
        Assert.All(firstGrade, course =>
        {
            Assert.Contains("Se det for deg", course.Content);
            Assert.Contains("Tegn", course.Content, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("etterprøvbarhet", course.Content, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("premisser", course.Content, StringComparison.OrdinalIgnoreCase);
        });

        var secondary = _courses.Where(x => x.Grade >= 8).ToList();
        Assert.All(secondary, course =>
        {
            Assert.Contains("dokumentasjon", course.Content, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("premiss", course.Content, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("motargument", course.Content, StringComparison.OrdinalIgnoreCase);
        });
    }

    [Fact]
    public void EarlyGradeQuestionsUseConcretePromptsAndAccessibleTerms()
    {
        var earlyQuestions = _courses.Where(x => x.Grade <= 2).SelectMany(x => x.Questions).ToList();
        Assert.Contains(earlyQuestions, x => x.Text.Contains("Se og tenk:"));
        Assert.Contains(earlyQuestions, x => x.Text.Contains("Tenk på tegningen:"));
        Assert.DoesNotContain(earlyQuestions, x => x.Text.Contains("Analyser:"));
        Assert.DoesNotContain(earlyQuestions, x => x.Text.Contains("faglig holdbare", StringComparison.OrdinalIgnoreCase));

        var firstGradeMath = _courses.Single(x => x.Grade == 1 && x.Subject == "Matematikk" && x.Title == "Tall og regning");
        Assert.Contains("tall", firstGradeMath.Content);
        Assert.Contains("telle", firstGradeMath.Content);
        Assert.DoesNotContain("plassverdisystemet", firstGradeMath.Content, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void QuizzesUseValidVariedSubjectQuestions()
    {
        var allQuestionTexts = new List<string>();
        Assert.All(_courses, course =>
        {
            Assert.True(course.Questions.Count >= 10);
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
            Assert.StartsWith(course.Grade <= 2 ? "Læringslek:" : "Fagoppdrag:", game.Title);
            Assert.Contains(course.Title, game.Title);
            Assert.Equal(3, game.Levels.Count);
            Assert.Equal(new[] { 6, 10, 5 }, game.Levels.OrderBy(x => x.LevelNumber).Select(x => x.Cards.Count));
            Assert.Equal(new[] { GameLevelMode.CardSort, GameLevelMode.Matching, GameLevelMode.Jigsaw },
                game.Levels.OrderBy(x => x.LevelNumber).Select(x => x.Mode));
            Assert.All(game.Levels, level =>
            {
                Assert.Equal(level.Cards.Count, level.Cards.Select(x => x.Text).Distinct(StringComparer.OrdinalIgnoreCase).Count());
                Assert.All(level.Cards, card => Assert.InRange(card.Text.Length, 1, 300));
                Assert.InRange(level.Instructions.Length, 10, 500);
            });
        });
    }

    private static int Count(string value, string fragment) =>
        (value.Length - value.Replace(fragment, string.Empty).Length) / fragment.Length;
}
