using LearnPlane.Web.Components;
using LearnPlane.Web.Data;
using LearnPlane.Web.Models;
using LearnPlane.Web.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' mangler.");

builder.Services.AddDbContextFactory<LearnPlaneDbContext>(options =>
    options.UseNpgsql(connectionString, npgsql => npgsql.EnableRetryOnFailure()), ServiceLifetime.Scoped);
builder.Services.AddDbContext<LearnPlaneDbContext>(options =>
    options.UseNpgsql(connectionString, npgsql => npgsql.EnableRetryOnFailure()));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.User.RequireUniqueEmail = false;
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<LearnPlaneDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/logg-inn";
    options.AccessDeniedPath = "/ingen-tilgang";
    options.ExpireTimeSpan = TimeSpan.FromDays(14);
    options.SlidingExpiration = true;
});

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<QuizService>();
builder.Services.AddScoped<CourseGameService>();
builder.Services.AddScoped<RewardStoreService>();
builder.Services.AddSingleton<PointBalanceCalculator>();
builder.Services.AddSingleton<ScoreCalculator>();
builder.Services.AddSingleton<ChallengePointCalculator>();
builder.Services.AddSingleton<GradeEligibilityPolicy>();

var dataProtectionKeysPath = builder.Configuration["DataProtection:KeysPath"];
if (!string.IsNullOrWhiteSpace(dataProtectionKeysPath))
{
    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeysPath))
        .SetApplicationName("LearnPlane");
}

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/feil", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapPost("/account/login", async (
    [FromForm] string username,
    [FromForm] string password,
    [FromForm] string? returnUrl,
    SignInManager<ApplicationUser> signInManager) =>
{
    var result = await signInManager.PasswordSignInAsync(username.Trim(), password, true, lockoutOnFailure: true);
    if (!result.Succeeded)
    {
        return Results.LocalRedirect($"/logg-inn?feil={Uri.EscapeDataString("Ugyldig brukernavn eller passord.")}");
    }

    return Results.LocalRedirect(IsLocalUrl(returnUrl) ? EncodeLocalUrl(returnUrl!) : "/");
});

app.MapPost("/account/register", async (
    [FromForm] string username,
    [FromForm] string displayName,
    [FromForm] int age,
    [FromForm] string password,
    [FromForm] string confirmPassword,
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager) =>
{
    username = username.Trim();
    displayName = displayName.Trim();
    if (username.Length < 3 || displayName.Length < 2)
        return Results.LocalRedirect("/registrer?feil=Brukernavn+og+navn+er+for+korte.");
    if (age is < GradeEligibilityPolicy.MinimumAge or > GradeEligibilityPolicy.MaximumAge)
        return Results.LocalRedirect("/registrer?feil=Alder+m%C3%A5+v%C3%A6re+mellom+6+og+18+%C3%A5r.");
    if (!string.Equals(password, confirmPassword, StringComparison.Ordinal))
        return Results.LocalRedirect("/registrer?feil=Passordene+er+ikke+like.");

    var user = new ApplicationUser { UserName = username, DisplayName = displayName, Age = age };
    var result = await userManager.CreateAsync(user, password);
    if (!result.Succeeded)
    {
        var error = string.Join(" ", result.Errors.Select(x => TranslateIdentityError(x.Code)));
        return Results.LocalRedirect($"/registrer?feil={Uri.EscapeDataString(error)}");
    }

    await userManager.AddToRoleAsync(user, Roles.Student);
    await signInManager.SignInAsync(user, isPersistent: true);
    return Results.LocalRedirect("/");
});

app.MapPost("/account/logout", async (SignInManager<ApplicationUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.LocalRedirect("/logg-inn");
});

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

await DatabaseInitializer.InitializeAsync(app.Services, app.Configuration);
app.Run();

static bool IsLocalUrl(string? url) =>
    !string.IsNullOrWhiteSpace(url) && url.StartsWith('/') && !url.StartsWith("//");

static string EncodeLocalUrl(string url)
{
    var absoluteUrl = new Uri(new Uri("http://localhost"), url);
    return absoluteUrl.PathAndQuery + absoluteUrl.Fragment;
}

static string TranslateIdentityError(string code) => code switch
{
    "DuplicateUserName" => "Brukernavnet er allerede i bruk.",
    "PasswordTooShort" => "Passordet må ha minst 8 tegn.",
    "PasswordRequiresDigit" => "Passordet må inneholde minst ett tall.",
    "PasswordRequiresLower" => "Passordet må inneholde minst én liten bokstav.",
    _ => "Kunne ikke opprette brukeren. Kontroller opplysningene."
};

public partial class Program;
