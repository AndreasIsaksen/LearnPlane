using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace LearnPlane.Web.Models;

public static class Roles
{
    public const string Admin = "Admin";
    public const string Student = "Elev";
}

public sealed class ApplicationUser : IdentityUser
{
    [MaxLength(100)]
    public string DisplayName { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public ICollection<QuizAttempt> QuizAttempts { get; set; } = [];
}

public enum CourseDifficulty
{
    Lett = 1,
    Middels = 2,
    Utfordrende = 3
}

public sealed class Course
{
    public int Id { get; set; }
    [Range(1, 10)] public int Grade { get; set; }
    [MaxLength(80)] public string Subject { get; set; } = string.Empty;
    [MaxLength(160)] public string Title { get; set; } = string.Empty;
    [MaxLength(350)] public string Summary { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public CourseDifficulty Difficulty { get; set; }
    public bool IsPublished { get; set; } = true;
    public int SortOrder { get; set; }
    public ICollection<QuizQuestion> Questions { get; set; } = [];
    public ICollection<QuizAttempt> Attempts { get; set; } = [];
}

public sealed class QuizQuestion
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;
    [MaxLength(500)] public string Text { get; set; } = string.Empty;
    [MaxLength(500)] public string Explanation { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public ICollection<AnswerOption> Options { get; set; } = [];
}

public sealed class AnswerOption
{
    public int Id { get; set; }
    public int QuizQuestionId { get; set; }
    public QuizQuestion Question { get; set; } = null!;
    [MaxLength(300)] public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int SortOrder { get; set; }
}

public sealed class QuizAttempt
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;
    public int CorrectAnswers { get; set; }
    public int TotalQuestions { get; set; }
    public decimal Percentage { get; set; }
    public bool Passed { get; set; }
    public int PointsAwarded { get; set; }
    public DateTime CompletedAtUtc { get; set; } = DateTime.UtcNow;
}
