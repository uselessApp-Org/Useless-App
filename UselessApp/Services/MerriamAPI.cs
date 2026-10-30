using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components;
using UselessApp.Components.DictionaryPage;

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

    }