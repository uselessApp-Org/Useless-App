using System.Net;
using System.Text;
using Microsoft.Extensions.Configuration;
using UselessApp.Services;
namespace UselessApp.UnitTests;

public class ExperimentServiceTests
{
    private sealed class Handler(string body, HttpStatusCode status = HttpStatusCode.OK) : HttpMessageHandler
    {
        public int Calls { get; private set; }
        public string? RequestBody { get; private set; }
        public string? Authorization { get; private set; }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Calls++;
            RequestBody = await request.Content!.ReadAsStringAsync(cancellationToken);
            Authorization = request.Headers.Authorization?.ToString();
            return new(status) { Content = new StringContent(body, Encoding.UTF8, "application/json") };
        }
    }
    private sealed class Factory(HttpClient client) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => client;
    }
    private static ExperimentService Create(Handler handler, string endpoint = "https://provider.example/run")
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ToolProviders:translator:Endpoint"] = endpoint,
            ["ToolProviders:translator:ApiKey"] = "test-key"
        }).Build();
        return new(new Factory(new HttpClient(handler)), config);
    }
    [Fact]
    public async Task Demo_DoesNotCallConfiguredProvider()
    {
        var handler = new Handler("{}");
        var result = await Create(handler).RunAsync("translator", "hello", false);
        Assert.Equal("Local demo", result.Source);
        Assert.Contains("toaster", result.Text);
        Assert.Equal(0, handler.Calls);
    }
    [Fact]
    public async Task Connected_SendsInputAndReadsResult()
    {
        var handler = new Handler("{\"result\":\"Wrong on purpose\"}");
        var result = await Create(handler).RunAsync("translator", "hello", true);
        Assert.Equal("Connected provider", result.Source);
        Assert.Equal("Wrong on purpose", result.Text);
        Assert.Contains("hello", handler.RequestBody!);
        Assert.Equal("Bearer test-key", handler.Authorization);
    }
    [Theory]
    [InlineData("not json")]
    [InlineData("{\"other\":true}")]
    public async Task BadResponse_IsReported(string body)
    {
        var result = await Create(new Handler(body)).RunAsync("translator", "hello", true);
        Assert.Equal("Response error", result.Source);
    }
    [Fact]
    public async Task HttpFailure_IsReported()
    {
        var result = await Create(new Handler("{}", HttpStatusCode.Unauthorized)).RunAsync("translator", "hello", true);
        Assert.Equal("Connection error", result.Source);
    }
    [Theory]
    [InlineData("")]
    [InlineData("http://provider.example/run")]
    public async Task InvalidEndpoint_DoesNotSendRequest(string endpoint)
    {
        var handler = new Handler("{}");
        var service = Create(handler, endpoint);
        Assert.False(service.IsConfigured("translator"));
        Assert.Equal("Not configured", (await service.RunAsync("translator", "hello", true)).Source);
        Assert.Equal(0, handler.Calls);
    }

    [Fact]
    public async Task Converter_DemoUsesSelectedUnits()
    {
        var handler = new Handler("{}");
        var result = await Create(handler).RunAsync("converter", "10 Meters to Feet", false,
            new Dictionary<string, string> { ["category"] = "Length", ["amount"] = "10", ["fromUnit"] = "Meters", ["toUnit"] = "Feet" });
        Assert.Contains("10 Meters → 22 Feet", result.Text);
        Assert.Equal(0, handler.Calls);
    }
    [Fact]
    public async Task Converter_RejectsIncompatibleUnits()
    {
        var result = await Create(new Handler("{}")).RunAsync("converter", "10 Meters to Pounds", false,
            new Dictionary<string, string> { ["category"] = "Length", ["amount"] = "10", ["fromUnit"] = "Meters", ["toUnit"] = "Pounds" });
        Assert.Equal("Validation error", result.Source);
    }
    [Fact]
    public async Task Translator_DemoUsesTargetLanguage()
    {
        var result = await Create(new Handler("{}")).RunAsync("translator", "hello", false,
            new Dictionary<string, string> { ["sourceLanguage"] = "en", ["targetLanguage"] = "es" });
        Assert.Contains("English → Spanish", result.Text);
        Assert.Contains("La tostadora", result.Text);
    }
    [Fact]
    public async Task Translator_ForwardsLanguageOptionsToProvider()
    {
        var handler = new Handler("{\"result\":\"Connected answer\"}");
        await Create(handler).RunAsync("translator", "hello", true,
            new Dictionary<string, string> { ["sourceLanguage"] = "en", ["targetLanguage"] = "fr" });
        using var request = System.Text.Json.JsonDocument.Parse(handler.RequestBody!);
        Assert.Equal("fr", request.RootElement.GetProperty("options").GetProperty("targetLanguage").GetString());
    }
}
