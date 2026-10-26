using UselessApp.Components;
using UselessApp.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;


var builder = WebApplication.CreateBuilder(args);

// Bind MongoDB settings
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDB"));

// Register MongoClient
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var cfg = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(cfg.ConnectionString);
});

// Register your data service
builder.Services.AddScoped<MongodbUserService>();
builder.Services.AddScoped<CalculatorEvaluatorService>();
builder.Services.AddScoped<DisplayNameService>();
builder.Services.AddScoped<IExperimentService, ExperimentService>();


builder.Services.AddHttpClient("experiments", client => client.Timeout = TimeSpan.FromSeconds(20));
builder.Services.AddHttpClient<GeoLocationService>();
builder.Services.AddHttpClient<WeatherService>();
builder.Services.AddHttpClient<GeminiService>();


builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found");
app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
