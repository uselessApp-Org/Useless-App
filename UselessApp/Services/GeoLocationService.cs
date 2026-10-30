using System.Text.Json.Serialization;
using System.Text.Json;
public class GeoLocationResponse
{
    [JsonPropertyName("results")]
    public List<GeoLocationResult>? Results { get; set; }
}

public class GeoLocationResult
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("latitude")]
    public float Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public float Longitude { get; set; }

    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;

    [JsonPropertyName("admin1")]
    public string? Admin1 { get; set; }
}

public class GeoLocationService
{
    private readonly HttpClient _httpClient;
    private readonly string baseURL = "https://geocoding-api.open-meteo.com/v1/search";

    public GeoLocationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<GeoLocationResult?> GetLocationData(string City)
    {
        if(string.IsNullOrWhiteSpace(City))
            return null;
        
        string url = $"{baseURL}?name={Uri.EscapeDataString(City.Trim())}&count=1";

        GeoLocationResponse? LocationData;
        string LocationString;
        HttpResponseMessage response = await _httpClient.GetAsync(url);

        if(response.IsSuccessStatusCode)
        {
            LocationString = await response.Content.ReadAsStringAsync();
            if(!string.IsNullOrEmpty(LocationString))
            {
                LocationData = JsonSerializer.Deserialize<GeoLocationResponse>(LocationString);
                return LocationData?.Results?.FirstOrDefault();
            }
        }

        return null;
    }
}   