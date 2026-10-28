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
    }