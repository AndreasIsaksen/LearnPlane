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
    }
}
