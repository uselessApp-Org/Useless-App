using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components;

public class MetaData
{
    [JsonPropertyName(name: "id")]
    public string ID { get; set; }
}

public class DictionaryEntry
{
    [JsonPropertyName("meta")]
    public MetaData Meta { get; set; }

    [JsonPropertyName("shortdef")]
    public List<string> ShortDef { get; set; }
}

public class MerriamAPI
{
    public readonly HttpClient _httpClient;
    public readonly string _apiKey = string.Empty;

    public MerriamAPI(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["DictionaryApi:ApiKey"]
            ?? throw new ArgumentNullException("Merriam API key is not configured.");
    }

    public async Task<(string finalUrl, string jsonString, string finalWord, string finalDescription, string apiResult)> FetchRandomWord()
    {
        try
        {
            string randomWordURL = "https://random-word-api.herokuapp.com/word";
            string randomWordJson = await _httpClient.GetStringAsync(randomWordURL);

            var wordArray = JsonSerializer.Deserialize<List<string>>(randomWordJson);
            string cleanWord = wordArray?.FirstOrDefault() ?? "hogmenay";

            string finalUrl = $"https://dictionaryapi.com/api/v3/references/collegiate/json/{randomWordJson}?key={_apiKey}";

            string jsonString = await FetchRawHttpDataAsync(finalUrl);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var entries = JsonSerializer.Deserialize<List<DictionaryEntry>>(jsonString, options);

            if (entries != null && entries.Count > 0)
            {
                string finalWord = entries[0].Meta?.ID;
                string finalDescription = entries[0].ShortDef != null ? string.Join("; ", entries[0].ShortDef) : "No definition found.";
                string apiResult = $"Found something else";

                return (finalUrl, jsonString, finalWord, finalDescription, apiResult);
            }
            else
            {
                return (finalUrl, jsonString, "", "", "No dictionary entries found for this word");
            }
        }
        catch (Exception ex)
        {
            return ("", "", "", "", $"Error: {ex.Message}");
        }
    }

    private async Task<string> FetchRawHttpDataAsync(string url)
    {
        try
        {
            using HttpClient client = new HttpClient();
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("User-Agent", "BlazorApp/1.0");

            using HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }
}