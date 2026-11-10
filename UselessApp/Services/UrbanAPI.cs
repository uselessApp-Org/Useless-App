using System.Text.Json;
using System.Text.Json.Serialization;

public class UrbanDictionaryResponse
{
    [JsonPropertyName("list")] 
    public List<UrbanDictionaryEntry> List { get; set; }
}

public class UrbanDictionaryEntry
{
    [JsonPropertyName("word")]
    public string Word { get; set; }

    [JsonPropertyName("definition")]
    public string Definition { get; set; }
}

public class UrbanAPI
{
    public readonly HttpClient _httpClient;

    public UrbanAPI(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
    }

    public async Task<(string finalUrl, string jsonString, string finalWord, string finalDescription, string apiResult)> FetchRandomWord()
    {
        try
        {
            string finalUrl = "https://api.urbandictionary.com/v0/random";

            string jsonString = await _httpClient.GetStringAsync(finalUrl);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var responseObj = JsonSerializer.Deserialize<UrbanDictionaryResponse>(jsonString, options);

            if (responseObj?.List != null && responseObj.List.Count > 0)
            {
                var firstEntry = responseObj.List[0];

                string finalWord = firstEntry.Word;
                string finalDescription = !string.IsNullOrWhiteSpace(firstEntry.Definition) 
                    ? firstEntry.Definition 
                    : "No definition found.";

                string apiResult = $"Found something else";

                return (finalUrl, jsonString, finalWord, finalDescription, apiResult);
            }
            else
            {
                string apiResult = $"No dictionary entries found for this word";
                return (finalUrl, jsonString, "", "", apiResult);
            }
        } 
        catch (Exception ex) 
        {
            return ("", "", "", "", $"Error: {ex.Message}");
        }
    }
}