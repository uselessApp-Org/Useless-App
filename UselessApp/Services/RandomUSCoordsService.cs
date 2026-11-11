using System.Text.Json;
using System.Text.Json.Serialization;

#region Random Coords Data
public class RandomCoordsResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public List<RandomCoord> Data { get; set; } = new List<RandomCoord>();
}

public class RandomCoord
{
    [JsonPropertyName("city")]
    public string City { get; set; } = string.Empty;

    [JsonPropertyName("state")]
    public string? State { get; set; }

    [JsonPropertyName("coordinates")]
    public double[] Coordinates { get; set; } = new double[2];

    // coordinates come back as [latitude, longitude]
    [JsonIgnore]
    public double Latitude => Coordinates.Length > 0 ? Coordinates[0] : 0;

    [JsonIgnore]
    public double Longitude => Coordinates.Length > 1 ? Coordinates[1] : 0;
}

#endregion

public class RandomUSCoordsService
{
    private readonly HttpClient _httpClient;
    private readonly string baseCountryURL = "https://api.randomcoords.com/v1/coordinates/countries/"; //{country}?limit={number}

    public RandomUSCoordsService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;

        // Azure App Service setting name: RandomCoords__ApiToken
        string apiToken = configuration["RandomCoords:ApiToken"]
            ?? throw new InvalidOperationException("RandomCoords API token is not configured.");

        _httpClient.DefaultRequestHeaders.Add("x-api-token", apiToken);
    }

    /// <summary>
    /// Gets random coordinates inside the US
    /// </summary>
    /// <param name="Limit">how many coordinates to get back</param>
    /// <returns>the list of random coordinates</returns>
    public async Task<(List<RandomCoord>? Coords, string? Error)> GetRandomUSCoords(int Limit = 1)
    {
        string finalURL = $"{baseCountryURL}united-states?limit={Limit}";

        RandomCoordsResponse? CoordsResponse;
        string CoordsResponseString = string.Empty;
        HttpResponseMessage responseMessage = await _httpClient.GetAsync(finalURL);

        if(!responseMessage.IsSuccessStatusCode)
        {
            return (null, $"Error: {(int)responseMessage.StatusCode} {responseMessage.ReasonPhrase}");
        }

        CoordsResponseString = await responseMessage.Content.ReadAsStringAsync();
        CoordsResponse = JsonSerializer.Deserialize<RandomCoordsResponse>(CoordsResponseString);

        return (CoordsResponse?.Data, null);
    }
}
