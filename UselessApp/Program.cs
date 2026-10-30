using UselessApp.Components;
using UselessApp.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

var builder = WebApplication.CreateBuilder(args);

// A missing database is allowed only for local Development previews.
var localPreview = builder.Environment.IsDevelopment()
    && (builder.Configuration.GetValue<bool>("LocalPreview")
        || string.IsNullOrWhiteSpace(builder.Configuration["MongoDB:ConnectionString"])
        || builder.Configuration["MongoDB:ConnectionString"] == "YOUR_CONNECTION_STRING");

// ---------------------------
// GOOGLE AUTH
// ---------------------------
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var authentication = builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = localPreview
        ? CookieAuthenticationDefaults.AuthenticationScheme
        : GoogleDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    // Allow cookies over HTTP for local testing
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

if (!localPreview)
{
    authentication.AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Google:ClientSecret"]!;

        // Ensure correlation cookie works smoothly on localhost
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

if (localPreview)
{
    // Construct collection handles for existing component injection without
    // contacting a database. Account routes are unavailable in this mode.
    builder.Services.PostConfigure<MongoDbSettings>(settings =>
    {
        settings.ConnectionString = "mongodb://127.0.0.1:27017/?serverSelectionTimeoutMS=1000";
        settings.DatabaseName = "LocalPreview";
    });
}

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
if (!localPreview)
using (var scope = app.Services.CreateScope())
{
    var userService = scope.ServiceProvider.GetRequiredService<MongodbUserService>();
    userService.EnsureIndexes();
}

if (localPreview)
{
    app.Logger.LogWarning("Local preview: MongoDB is not configured. Account features are unavailable.");
    app.Use(async (context, next) =>
    {
        var requestPath = context.Request.Path;
        if (requestPath.Equals("/login", StringComparison.OrdinalIgnoreCase)
            || requestPath.Equals("/signup", StringComparison.OrdinalIgnoreCase)
            || requestPath.StartsWithSegments("/auth"))
        {
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            context.Response.ContentType = "text/html; charset=utf-8";
            await context.Response.WriteAsync("<!doctype html><html lang=\"en\"><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width,initial-scale=1\"><title>Local preview</title><body><h1>Local preview</h1><p>Account features require a MongoDB connection string. You can still test the local tools and themes.</p><a href=\"/\">Return to apps</a></body></html>");
            return;
        }
        await next(context);
    });
}
else
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();
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
// ---------------------------
// RAZOR COMPONENTS
// ---------------------------
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
