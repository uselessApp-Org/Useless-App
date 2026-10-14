using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace UselessApp.IntegrationTests;
public class AppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder) => builder.UseEnvironment("Development");
}
public class RouteTests : IClassFixture<AppFactory>
{
    private readonly HttpClient client;
    public RouteTests(AppFactory factory) => client = factory.CreateClient(new WebApplicationFactoryClientOptions
    {
        BaseAddress = new Uri("https://localhost"), AllowAutoRedirect = false
    });
    [Theory]
    [InlineData("/", "Your toolkit")]
    [InlineData("/calculator", "Calculator")]
    [InlineData("/counter", "Current count: 0")]
    [InlineData("/api-search-term", "Dictionary")]
    [InlineData("/lab/timer", "Eventually")]
    [InlineData("/lab/trivia", "Definitely Incorrect")]
    [InlineData("/lab/converter", "Unit Confuser")]
    public async Task LocalPages_RenderWithoutExternalServices(string route, string heading)
    {
        using var response = await client.GetAsync(route);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(heading, await response.Content.ReadAsStringAsync());
    }
    [Fact]
    public async Task HealthEndpoint_ReturnsOk()
    {
        using var response = await client.GetAsync("/health/live");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("\"status\":\"ok\"", await response.Content.ReadAsStringAsync());
    }
}
