using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace UselessApp.IntegrationTests;

public class StaticAssetTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task VersionedStylesAndScripts_AreServed(bool fingerprinted)
    {
        var output = Path.GetDirectoryName(typeof(UselessApp.Services.CalculatorEvaluatorService).Assembly.Location)!;
        var runtimeManifest = Path.Combine(output, "UselessApp.staticwebassets.runtime.json");
        var endpointManifest = Path.Combine(output, "UselessApp.staticwebassets.endpoints.json");
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ApplicationName = "UselessApp",
            EnvironmentName = Environments.Development,
            ContentRootPath = output
        });
        builder.Logging.ClearProviders();
        builder.Services.AddRazorPages();
        builder.Configuration[WebHostDefaults.StaticWebAssetsKey] = runtimeManifest;
        builder.WebHost.UseStaticWebAssets();
        builder.WebHost.UseTestServer();
        await using var app = builder.Build();
        app.UseStaticFiles();
        app.MapStaticAssets(endpointManifest);
        await app.StartAsync();
        var client = app.GetTestClient();
        using var manifest = JsonDocument.Parse(await File.ReadAllTextAsync(endpointManifest));
        var required = new[] { "app.css", "themes.css", "polish.css", "UselessApp.styles.css", "preferences.js", "navigation.js" };
        foreach (var file in required)
        {
            var endpoint = manifest.RootElement.GetProperty("Endpoints").EnumerateArray()
                .First(e => e.GetProperty("AssetFile").GetString() == file && e.GetProperty("Route").GetString() != file);
            var url = fingerprinted ? "/" + endpoint.GetProperty("Route").GetString() : app.Services.GetRequiredService<IFileVersionProvider>().AddFileVersionToPath("", "/" + file);
            if (!fingerprinted) Assert.StartsWith("/" + file + "?v=", url);
            using var response = await client.GetAsync(url);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotEmpty(await response.Content.ReadAsByteArrayAsync());
            Assert.Contains(file.EndsWith(".css") ? "css" : "javascript", response.Content.Headers.ContentType!.MediaType!);
        }
    }
}
