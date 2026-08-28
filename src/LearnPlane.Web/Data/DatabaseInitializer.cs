using LearnPlane.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LearnPlane.Web.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services, IConfiguration configuration)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<LearnPlaneDbContext>();
        await db.Database.EnsureCreatedAsync();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var role in new[] { Roles.Admin, Roles.Student })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var admin = await userManager.FindByNameAsync("admin");
        if (admin is null)
        {
            admin = new ApplicationUser { UserName = "admin", DisplayName = "Administrator" };
            var password = configuration["ADMIN_INITIAL_PASSWORD"] ?? "3d9XehYf";
            var result = await userManager.CreateAsync(admin, password);
            if (!result.Succeeded)
                throw new InvalidOperationException($"Administrator kunne ikke opprettes: {string.Join(", ", result.Errors.Select(x => x.Description))}");
            await userManager.AddToRoleAsync(admin, Roles.Admin);
        }

        if (!await db.Courses.AnyAsync())
        {
            db.Courses.AddRange(CurriculumCatalog.CreateCourses());
            await db.SaveChangesAsync();
        }
    }
}
