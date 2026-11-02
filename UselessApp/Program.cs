using UselessApp.Components;
using UselessApp.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

var builder = WebApplication.CreateBuilder(args);

// Supplies content-based asset versions for stable CSS and script URLs.
builder.Services.AddRazorPages();

// ---------------------------
// GOOGLE AUTH
// ---------------------------
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var googleConfigured = !string.IsNullOrWhiteSpace(builder.Configuration["Google:ClientId"])
    && !string.IsNullOrWhiteSpace(builder.Configuration["Google:ClientSecret"]);
if (!googleConfigured && !builder.Environment.IsDevelopment())
    throw new InvalidOperationException("Google OAuth client ID and secret must be configured.");

var authentication = builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = googleConfigured
        ? GoogleDefaults.AuthenticationScheme : CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(options => options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest);

if (googleConfigured)
{
    authentication.AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Google:ClientSecret"]!;
        options.CorrelationCookie.SameSite = SameSiteMode.Lax;
        options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });
}
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
// Keep the hosted database index initialization. A development credential failure
// must not prevent public pages or their CSS from being served.
try
{
    using var scope = app.Services.CreateScope();
    var userService = scope.ServiceProvider.GetRequiredService<MongodbUserService>();
    userService.EnsureIndexes();
}
catch (MongoException) when (app.Environment.IsDevelopment())
{
    app.Logger.LogWarning("The hosted account database is unavailable. Check MongoDB user secrets before using signup or login.");
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
{
    if (!googleConfigured)
        return (IResult)Results.Problem("Google sign-in is not configured. Add Google OAuth credentials to user secrets.", statusCode: 503);
    return Results.Challenge(new AuthenticationProperties { RedirectUri = returnUrl ?? "/" },
        [GoogleDefaults.AuthenticationScheme]);
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
// ---------------------------
// RAZOR COMPONENTS
// ---------------------------
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
