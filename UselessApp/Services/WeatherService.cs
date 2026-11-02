using System.Text.Json;
using System.Text.Json.Serialization;

#region Weather Point Data
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

    [JsonPropertyName("civilTwilightBegin")]
    public DateTimeOffset? Dawn { get; set; }

    [JsonPropertyName("civilTwilightEnd")]
    public DateTimeOffset? Dusk { get; set; }
}

#endregion

#region Hourly / Daily Weather Data

// these should work for both hourly forecast and "daily forecast" same exact structure
public class Forecast
{
    [JsonPropertyName("properties")]
    public ForecastData? Properties { get; set; }
}

public class ForecastData
{
    [JsonPropertyName("units")]
    public string Units { get; set; } = string.Empty;

    [JsonPropertyName("periods")]
    public List<HourlyPeriod> hourlyPeriods = new List<HourlyPeriod>();
}

public class HourlyPeriod
{
    [JsonPropertyName("number")]   
    public int hourNumber;

    [JsonPropertyName("name")]
    public string name = string.Empty;

    [JsonPropertyName("startTime")]
    public DateTimeOffset startTime;

    [JsonPropertyName("endTime")]
    public DateTimeOffset endTime;

    [JsonPropertyName("temperature")]
    public int temperature;

    [JsonPropertyName("temperatureUnit")]
    public string temperatureUnit = string.Empty;

    [JsonPropertyName("windSpeed")]
    public string windSpeed = string.Empty;

    [JsonPropertyName("windDirection")]
    public string windDirection = string.Empty;

    [JsonPropertyName("icon")]
    public string iconURL = string.Empty;

    [JsonPropertyName("shortForecast")]
    public string shortForecast = string.Empty;
}

#endregion

public class WeatherService
{
    private readonly HttpClient _httpClient;
    private readonly string basePointURL = "https://api.weather.gov/points/"; //{latitude, longitude}

    public WeatherService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "UselessApp_UWSTOUT_WI");
    }

    /// <summary>
    /// Gets Required weather data into classes for use
    /// </summary>
    /// <param name="Latitude"></param>
    /// <param name="Longitude"></param>
    /// <returns>the class that makes the weater data</returns>
    public async Task<(WeatherProperties? Properties, string? Error)> GetWeatherPointData(float Latitude, float Longitude)
    {
        string finalURL = $"{basePointURL}{Latitude},{Longitude}";

        WeatherPointsResponse? PointResponse;
        string PointResponseString = string.Empty;
        HttpResponseMessage responseMessage = await _httpClient.GetAsync(finalURL);

        if(!responseMessage.IsSuccessStatusCode)
        {
            return (null, $"Error: {(int)responseMessage.StatusCode} {responseMessage.ReasonPhrase}");
        }

        PointResponseString = await responseMessage.Content.ReadAsStringAsync();
        PointResponse = JsonSerializer.Deserialize<WeatherPointsResponse>(PointResponseString);

        return (PointResponse?.Properties, null);
    }

    /// <summary>
    /// this can be used to get either hourly or data forecast data
    /// </summary>
    /// <param name="ForecastURL">URL from WeatherProperties</param>
    /// <returns>forecast data</returns>
    public async Task<(ForecastData? ForecastData, string? Error)> GetWeatherData(string ForecastURL)
    {
        Forecast? DailyCast;
        string ForecastResponse = string.Empty;
        HttpResponseMessage responseMessage = await _httpClient.GetAsync(ForecastURL);

        if(!responseMessage.IsSuccessStatusCode)
            return (null, $"Error: {(int)responseMessage.StatusCode} {responseMessage.ReasonPhrase}");
        
        ForecastResponse = await responseMessage.Content.ReadAsStringAsync();
        DailyCast = JsonSerializer.Deserialize<Forecast>(ForecastResponse);

        return (DailyCast?.Properties, null);
    }


}