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
    [Range(6, 18)] public int? Age { get; set; }
    public ICollection<QuizAttempt> QuizAttempts { get; set; } = [];
    public ICollection<GameLevelAttempt> GameLevelAttempts { get; set; } = [];
    public ICollection<CartItem> CartItems { get; set; } = [];
    public ICollection<RewardPurchase> RewardPurchases { get; set; } = [];
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
    public CourseGame? Game { get; set; }
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

public sealed class CourseGame
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;
    [MaxLength(160)] public string Title { get; set; } = string.Empty;
    [MaxLength(500)] public string Intro { get; set; } = string.Empty;
    public ICollection<GameLevel> Levels { get; set; } = [];
}

public sealed class GameLevel
{
    public int Id { get; set; }
    public int CourseGameId { get; set; }
    public CourseGame CourseGame { get; set; } = null!;
    [Range(1, 3)] public int LevelNumber { get; set; }
    [MaxLength(100)] public string Title { get; set; } = string.Empty;
    [MaxLength(500)] public string Instructions { get; set; } = string.Empty;
    [Range(1, 20)] public int MaxPoints { get; set; }
    public ICollection<GameCard> Cards { get; set; } = [];
    public ICollection<GameLevelAttempt> Attempts { get; set; } = [];
}

public sealed class GameCard
{
    public int Id { get; set; }
    public int GameLevelId { get; set; }
    public GameLevel Level { get; set; } = null!;
    [MaxLength(100)] public string Text { get; set; } = string.Empty;
    public bool IsTarget { get; set; }
    public int SortOrder { get; set; }
}

public sealed class GameLevelAttempt
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
    public int GameLevelId { get; set; }
    public GameLevel Level { get; set; } = null!;
    public int CorrectSelections { get; set; }
    public int IncorrectSelections { get; set; }
    public int TotalTargets { get; set; }
    public decimal Percentage { get; set; }
    public bool Passed { get; set; }
    public int PointsAwarded { get; set; }
    public DateTime CompletedAtUtc { get; set; } = DateTime.UtcNow;
}

public sealed class RewardItem
{
    public int Id { get; set; }
    [Required, MaxLength(140)] public string Name { get; set; } = string.Empty;
    [Required, MaxLength(600)] public string Description { get; set; } = string.Empty;
    [Required] public string ImageUrl { get; set; } = string.Empty;
    [Range(1, 100_000)] public int PricePoints { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    public ICollection<CartItem> CartItems { get; set; } = [];
}

public sealed class CartItem
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
    public int RewardItemId { get; set; }
    public RewardItem RewardItem { get; set; } = null!;
    [Range(1, 99)] public int Quantity { get; set; } = 1;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

public sealed class RewardPurchase
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
    public int TotalPoints { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public ICollection<RewardPurchaseLine> Lines { get; set; } = [];
}

public sealed class RewardPurchaseLine
{
    public int Id { get; set; }
    public int RewardPurchaseId { get; set; }
    public RewardPurchase Purchase { get; set; } = null!;
    public int? RewardItemId { get; set; }
    [MaxLength(140)] public string ItemName { get; set; } = string.Empty;
    [MaxLength(600)] public string ItemDescription { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int UnitPricePoints { get; set; }
    public int Quantity { get; set; }
}
