using LearnPlane.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LearnPlane.Web.Data;

public sealed class LearnPlaneDbContext(DbContextOptions<LearnPlaneDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<QuizQuestion> QuizQuestions => Set<QuizQuestion>();
    public DbSet<AnswerOption> AnswerOptions => Set<AnswerOption>();
    public DbSet<QuizAttempt> QuizAttempts => Set<QuizAttempt>();
    public DbSet<CourseGame> CourseGames => Set<CourseGame>();
    public DbSet<GameLevel> GameLevels => Set<GameLevel>();
    public DbSet<GameCard> GameCards => Set<GameCard>();
    public DbSet<GameLevelAttempt> GameLevelAttempts => Set<GameLevelAttempt>();
    public DbSet<RewardItem> RewardItems => Set<RewardItem>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<RewardPurchase> RewardPurchases => Set<RewardPurchase>();
    public DbSet<RewardPurchaseLine> RewardPurchaseLines => Set<RewardPurchaseLine>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Course>().HasIndex(x => new { x.Grade, x.Subject, x.SortOrder });
        builder.Entity<QuizAttempt>().HasIndex(x => new { x.UserId, x.CourseId });
        builder.Entity<QuizAttempt>().Property(x => x.Percentage).HasPrecision(5, 2);
        builder.Entity<QuizAttempt>()
            .HasOne(x => x.User).WithMany(x => x.QuizAttempts)
            .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<QuizAttempt>()
            .HasOne(x => x.Course).WithMany(x => x.Attempts)
            .HasForeignKey(x => x.CourseId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<CourseGame>().HasIndex(x => x.CourseId).IsUnique();
        builder.Entity<CourseGame>()
            .HasOne(x => x.Course).WithOne(x => x.Game)
            .HasForeignKey<CourseGame>(x => x.CourseId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<GameLevel>().HasIndex(x => new { x.CourseGameId, x.LevelNumber }).IsUnique();
        builder.Entity<GameLevel>()
            .HasOne(x => x.CourseGame).WithMany(x => x.Levels)
            .HasForeignKey(x => x.CourseGameId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<GameCard>().HasIndex(x => new { x.GameLevelId, x.SortOrder });
        builder.Entity<GameCard>()
            .HasOne(x => x.Level).WithMany(x => x.Cards)
            .HasForeignKey(x => x.GameLevelId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<GameLevelAttempt>().HasIndex(x => new { x.UserId, x.GameLevelId });
        builder.Entity<GameLevelAttempt>().Property(x => x.Percentage).HasPrecision(5, 2);
        builder.Entity<GameLevelAttempt>()
            .HasOne(x => x.User).WithMany(x => x.GameLevelAttempts)
            .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<GameLevelAttempt>()
            .HasOne(x => x.Level).WithMany(x => x.Attempts)
            .HasForeignKey(x => x.GameLevelId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<CartItem>().HasIndex(x => new { x.UserId, x.RewardItemId }).IsUnique();
        builder.Entity<CartItem>()
            .HasOne(x => x.User).WithMany(x => x.CartItems)
            .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<CartItem>()
            .HasOne(x => x.RewardItem).WithMany(x => x.CartItems)
            .HasForeignKey(x => x.RewardItemId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<RewardPurchase>().HasIndex(x => new { x.UserId, x.CreatedAtUtc });
        builder.Entity<RewardPurchase>()
            .HasOne(x => x.User).WithMany(x => x.RewardPurchases)
            .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<RewardPurchaseLine>()
            .HasOne(x => x.Purchase).WithMany(x => x.Lines)
            .HasForeignKey(x => x.RewardPurchaseId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<RewardPurchaseLine>()
            .HasOne<RewardItem>().WithMany()
            .HasForeignKey(x => x.RewardItemId).OnDelete(DeleteBehavior.SetNull);
    }
}
