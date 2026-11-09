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
    options.CallbackPath = new PathString("/auth/google-callback");

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
builder.Services.AddScoped<CalculatorEvaluatorService>();
builder.Services.AddScoped<DisplayNameService>();
builder.Services.AddScoped<IExperimentService, ExperimentService>();

builder.Services.AddScoped<ProtectedSessionStorage>();

builder.Services.AddHttpClient("experiments", client => client.Timeout = TimeSpan.FromSeconds(20));
builder.Services.AddHttpClient<GeoLocationService>();
builder.Services.AddHttpClient<WeatherService>();
builder.Services.AddHttpClient<GeminiService>();


builder.Services.AddHttpClient<MerriamAPI>();
builder.Services.AddHttpClient<UrbanAPI>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient<GeminiService>();

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
//Mongo Indexs
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
app.MapGet("/auth/login", (string? returnUrl) =>
    Results.Challenge(
        new AuthenticationProperties { RedirectUri = returnUrl ?? "/" },
        [GoogleDefaults.AuthenticationScheme]));

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
// New app sign up/login flow with Google OAuth
app.MapGet("/auth/google-callback", async (HttpContext context, MongodbUserService userService) =>
{
    var result = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    if (!result.Succeeded) return Results.Redirect("/login?error=auth_failed");

    var email = result.Principal.FindFirstValue(ClaimTypes.Email);
    var googleId = result.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
    var name = result.Principal.FindFirstValue(ClaimTypes.Name);

    // Check if user exists in MongoDB
    var user = await userService.ValidateLoginAsync(email, null);

    if (user == null)
    {
        // --- THIS IS THE SIGN-UP STEP ---
        user = new User
        {
            Email = email,
            DisplayName = name,
            GoogleId = googleId,
            CreatedAt = DateTime.UtcNow
        };
        await userService.CreateUserAsync(user);
    }
    else if (string.IsNullOrEmpty(user.GoogleId))
    {
        // Account linking (e.g., if they previously signed up with password)
        user.GoogleId = googleId;
        await userService.UpdateUserAsync(user);
    }

    return Results.Redirect("/");
});
// ---------------------------
// RAZOR COMPONENTS
// ---------------------------
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
