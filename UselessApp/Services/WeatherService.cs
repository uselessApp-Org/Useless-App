// first use coords with
// /points/{latitude},{longitude}
// this will return a string url for both hourly and daily
// forecast hourly  forecast daily
// can also get information for current weather box

using System.Text.Json.Serialization;
using System.Text.Json;

public class WeatherPointsResponse
{
    [JsonPropertyName("properties")]
    public WeatherProperties? Properties { get; set; }
}

public class WeatherProperties
{
    [JsonPropertyName("@id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("gridX")]
    public int GridX { get; set; }

    [JsonPropertyName("gridY")]
    public int GridY { get; set; }

    [JsonPropertyName("forecast")]
    public string Forecast { get; set; } = string.Empty;

    [JsonPropertyName("forecastHourly")]
    public string ForecastHourly { get; set; } = string.Empty;

    [JsonPropertyName("county")]
    public string County { get; set; } = string.Empty;

    [JsonPropertyName("astronomicalData")]
    public AstronomicalData? AstronomicalData { get; set; }
}

public class AstronomicalData
{
    [JsonPropertyName("sunrise")]
    public DateTimeOffset? Sunrise { get; set; }

    [JsonPropertyName("sunset")]
    public DateTimeOffset? Sunset { get; set; }
}

public class WeatherService()
{
    private readonly HttpClient _httpClient;
}