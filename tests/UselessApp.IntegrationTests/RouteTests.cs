/*using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
namespace UselessApp.IntegrationTests;

public class AppFactory : WebApplicationFactory<object>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
        builder.UseEnvironment("Development");
}

public class RouteTests : IClassFixture<AppFactory>, IDisposable
{
    private readonly HttpClient client;
    public RouteTests(AppFactory factory) => client = factory.CreateClient(new()
    {
        BaseAddress = new Uri("https://localhost"),
        AllowAutoRedirect = false
    });

    [Theory]
    [InlineData("/", "Your toolkit")]
    [InlineData("/counter", "Current count: 0")]
    [InlineData("/lab/translator", "Lost in Translation")]
    [InlineData("/lab/converter", "Unit Confuser")]
    [InlineData("/weather", "Temp. (C)")]
    [InlineData("/Error", "An error occurred while processing your request.")]
    public async Task Pages_ReturnHtmlWithExpectedContent(string route, string expected)
    {
        using var response = await client.GetAsync(route);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
        Assert.Contains(expected, await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task UnknownRoute_ReturnsNotFound()
    {
        using var response = await client.GetAsync("/this-route-does-not-exist");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    public void Dispose() => client.Dispose();
}
*/