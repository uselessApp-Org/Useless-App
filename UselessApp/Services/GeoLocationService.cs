using System.Text.Json.Serialization;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
public class GeoLocationResponse
{
    [JsonPropertyName("results")]
    public List<GeoLocationResult>? Results { get; set; }
}

public class GeoLocationResult
{
    [JsonPropertyName("id")]
    public int Id;

    [JsonPropertyName("name")]
    public string Name = string.Empty;

    [JsonPropertyName("latitude")]
    public float Latitude;

    [JsonPropertyName("longitude")]
    public float Longitude;

    [JsonPropertyName("country")]
    public string Country = string.Empty;
}

public class GeoLocationService
{
    private readonly HttpClient _httpClient;
    private readonly string baseURL = "https://geocoding-api.open-meteo.com/v1/search/";

    public GeoLocationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<GeoLocationResult?> GetLocationData(string City)
    {
        if(string.IsNullOrWhiteSpace(City))
            return null;
        
        string url = $"{baseURL}{City}";

        GeoLocationResult LocationData;
        HttpResponseMessage response = await _httpClient.GetAsync(url);

        if(response.IsSuccessStatusCode)
            LocationData = await response.Content.ReadAsAsync<GeoLocationResult>();
    }
}   