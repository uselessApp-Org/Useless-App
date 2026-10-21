using System.Text.Json;
using System.Text.Json.Serialization;

public class DictionaryResult
{
    [JsonPropertyName("meta")]
    public MetaData? Meta { get; set; }

    [JsonPropertyName("shortdef")]
    public List<string> ShortDef { get; set; } = new List<string>();
}

public class MetaData
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
}

public class MerriamWebsterService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey = string.Empty;
    private string ResultString = string.Empty;
    public string ResultDef = string.Empty;

    public MerriamWebsterService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["DictionaryApi:ApiKey"]
            ?? throw new ArgumentNullException("Gemini API key is not configured.");
    }

    public async Task<string> FetchRandomWord()
    {
        string randomWordURL = "https://random-word-api.herokuapp.com/word";
        string randomWordJson = await _httpClient.GetStringAsync(randomWordURL);

        var wordArray = JsonSerializer.Deserialize<List<string>>(randomWordJson);
        string cleanWord = wordArray?.FirstOrDefault() ?? "hogmenay";

        return cleanWord;
    }

    public async Task ExecuteSearch()
    {
        ResultString = $"Looking up something entirely different...";

        ResultString = await FetchRandomWord();

        string BaseApiUrl = $"https://dictionaryapi.com/api/v3/references/collegiate/json/{ResultString}?key={_apiKey}";
        ResultString = await FetchRawHttpDataAsync(BaseApiUrl);

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var results = JsonSerializer.Deserialize<List<DictionaryResult>>(ResultString, options);

        DictionaryResult? firstMatch = results?.FirstOrDefault();
        if (firstMatch != null)
        {
            ResultString = firstMatch.Meta?.Id ?? "Unknown word";
            ResultDef = string.Join("; ", firstMatch.ShortDef);
        }
    }

    public async Task<string> FetchRawHttpDataAsync(string url)
    {
        try
        {
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("User-Agent", "BlazorApp/1.0");

            using HttpResponseMessage response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }
}