using UselessApp.Components;
using UselessApp.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;
using UselessApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Supplies content-based versions for the existing stylesheet and script links.
builder.Services.AddRazorPages();
builder.Services.AddCascadingAuthenticationState();

// ---------------------------
// GOOGLE AUTH
// ---------------------------
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    // Allow cookies over HTTP for local testing
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Google:ClientId"]!;
    options.ClientSecret = builder.Configuration["Google:ClientSecret"]!;

    // Match the callback endpoint path you defined
    options.CallbackPath = new PathString("/signin-google");
    options.CorrelationCookie.SameSite = SameSiteMode.Lax;
    options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

builder.Services.AddCascadingAuthenticationState();

// ---------------------------
// MONGODB + CUSTOM SERVICES
// ---------------------------
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDB"));

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var cfg = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(cfg.ConnectionString);
});

builder.Services.AddScoped<MongodbUserService>();
builder.Services.AddScoped<GoogleSignInService>(); // <--- Registered GoogleSignInService
builder.Services.AddScoped<CalculatorEvaluatorService>();
builder.Services.AddScoped<DisplayNameService>();
builder.Services.AddScoped<UserStatsService>();
builder.Services.AddScoped<IExperimentService, ExperimentService>();

builder.Services.AddScoped<ProtectedSessionStorage>();

builder.Services.AddHttpClient("experiments", client => client.Timeout = TimeSpan.FromSeconds(20));
builder.Services.AddHttpClient<GeoLocationService>();
builder.Services.AddHttpClient<WeatherService>();
builder.Services.AddHttpClient<RandomUSCoordsService>();
builder.Services.AddHttpClient<RainViewerService>();
builder.Services.AddHttpClient<GeminiService>();

builder.Services.AddHttpClient<MerriamAPI>();
builder.Services.AddHttpClient<UrbanAPI>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient<GeminiService>();
builder.Services.AddHttpClient<TMDBService>();


// ---------------------------
// BUILD APP
// ---------------------------
var app = builder.Build();

// ---------------------------
// PIPELINE
// ---------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// Mongo Indexes
using (var scope = app.Services.CreateScope())
{
    var userService = scope.ServiceProvider.GetRequiredService<MongodbUserService>();
    userService.EnsureIndexes();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapStaticAssets();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

// ---------------------------
// AUTH ENDPOINTS
// ---------------------------
app.MapGet("/auth/login", () =>
{
    var properties = new AuthenticationProperties
    {
        // This forces Google's middleware to hand off to your endpoint after login
        RedirectUri = "/auth/google-callback"
    };
    return Results.Challenge(properties, [GoogleDefaults.AuthenticationScheme]);
});
app.MapGet("/auth/logout", async (HttpContext context) =>
{
    // Clear the authentication cookie
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    // Prevent the browser from caching the logged-in state (BFCache fix)
    context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
    context.Response.Headers["Pragma"] = "no-cache";
    context.Response.Headers["Expires"] = "0";

    // Force a hard redirect back to home
    context.Response.Redirect("/login", permanent: false);
});

// Google OAuth Callback Flow using GoogleSignInService
app.MapGet("/auth/google-callback", async (HttpContext context, GoogleSignInService googleSignInService) =>
{
    var result = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    if (!result.Succeeded || result.Principal == null) 
        return Results.Redirect("/login?error=auth_failed");

    var (user, needsOnboarding) = await googleSignInService.ProcessGoogleUserAsync(result.Principal);

    if (user == null)
        return Results.Redirect("/login?error=user_creation_failed");

    // 1. Create the claims for the cookie so <AuthorizeView> and User.Identity recognize them
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, string.IsNullOrEmpty(user.DisplayName) ? user.Username : user.DisplayName),
        new Claim(ClaimTypes.NameIdentifier, user.Username)
    };
    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    
    // 2. Issue the authentication cookie
    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

    // 3. Redirect straight to onboarding or home (SessionStorage will populate once Blazor renders)
    if (needsOnboarding)
    {
        return Results.Redirect("/onboarding");
    }

    return Results.Redirect("/");
});

// ---------------------------
// RAZOR COMPONENTS
// ---------------------------
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

     

if (args.Contains("--update-users"))
{
    Console.WriteLine("Starting migration mode");

    using var scope = app.Services.CreateScope();

    var userService = scope.ServiceProvider
        .GetRequiredService<MongodbUserService>();

    await userService.UpdateUsersAsync();

    Console.WriteLine("Migration complete.");
    return;
}

app.Run();
