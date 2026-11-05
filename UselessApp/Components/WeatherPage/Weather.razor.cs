using UselessApp.Components.WeatherPage.Components;

namespace UselessApp.Components.WeatherPage;

public partial class Weather
{
    // Sample data until the weather API is wired up
    private string location = "Menomonie, WI";
    private string lastUpdated = DateTime.Now.ToString("h:mm tt");

    // Location prompt
    private bool showLocationPrompt = true;
    private bool hasLocation = false;
    private GeoLocationResult? selectedLocation;
    public WeatherProperties? weatherPoint;
    public WeatherProperties? badWeatherPoint;
    public ForecastData? RealHourlyForecast;

    public ForecastData? BadHourlyForecast;
    public ForecastData? BadDailyForecast;

    // Shown in place of the hourly/daily panels when loading their data fails
    private string? forecastError;

    private float BadLong;
    private float BadLat;
    private string? errorMessage;

    // LocationPrompt is removed from the tree when hidden, so it opens with a fresh input each time
    private void OpenLocationPrompt() => showLocationPrompt = true;

    private async Task SetLocation(GeoLocationResult result)
    {
        selectedLocation = result;
        location = string.IsNullOrEmpty(result.Admin1)
            ? $"{result.Name}, {result.Country}"
            : $"{result.Name}, {result.Admin1}";
        lastUpdated = DateTime.Now.ToString("h:mm tt");
        hasLocation = true;
        showLocationPrompt = false;

        await GetGoodLocation(result);
        await GetRandomLocation();
    }

    private async Task GetGoodLocation(GeoLocationResult result)
    {
        // Clear old results so a failed lookup doesn't leave the previous city's conditions on screen
        errorMessage = null;
        weatherPoint = null;
        RealHourlyForecast = null;

        try
        {
            string? ErrorString;
            (weatherPoint, ErrorString) = await WeatherService.GetWeatherPointData(result.Latitude, result.Longitude);
            if (!string.IsNullOrEmpty(ErrorString) || weatherPoint is null)
            {
                errorMessage = $"Couldn't find weather data for {location}. {ErrorString ?? "The weather service returned nothing."}";
                return;
            }

            ErrorString = await GetGoodHourlyForecast();
            if (!string.IsNullOrEmpty(ErrorString))
            {
                errorMessage = $"Couldn't load the current conditions. {ErrorString}";
                return;
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"Something went wrong loading the current conditions. {ex.Message}";
        }
    }

    private async Task GetRandomLocation()
    {
        // Clear old results so a failed lookup doesn't leave the previous city's forecast on screen
        forecastError = null;
        badWeatherPoint = null;
        BadDailyForecast = null;
        BadHourlyForecast = null;

        // try/catch: an exception escaping an event handler kills the Blazor Server circuit,
        // so the page would stop updating with no message
        try
        {
            var (RandCoords, ErrorString) = await RandomCoordsService.GetRandomUSCoords();
            RandomCoord? coord = RandCoords?.FirstOrDefault();
            if (!string.IsNullOrEmpty(ErrorString) || coord is null)
            {
                forecastError = $"Couldn't get a random location. {ErrorString ?? "No coordinates were returned."}";
                return;
            }

            (badWeatherPoint, ErrorString) = await WeatherService.GetWeatherPointData((float)coord.Latitude, (float)coord.Longitude);
            if (!string.IsNullOrEmpty(ErrorString) || badWeatherPoint is null)
            {
                forecastError = $"Couldn't find weather data for {coord.City}. {ErrorString ?? "The weather service returned nothing."}";
                return;
            }

            ErrorString = await GetBadDailyForecast();
            if (!string.IsNullOrEmpty(ErrorString))
            {
                forecastError = $"Couldn't load the daily forecast. {ErrorString}";
                return;
            }

            ErrorString = await GetBadHourlyForecast();
            if (!string.IsNullOrEmpty(ErrorString))
            {
                forecastError = $"Couldn't load the hourly forecast. {ErrorString}";
                return;
            }
        }
        catch (Exception ex)
        {
            forecastError = $"Something went wrong loading the forecast. {ex.Message}";
        }
    }

    // Each helper returns the error string (null when it worked) so the caller can report it
    private async Task<string?> GetBadDailyForecast()
    {
        string? ErrorString;
        (BadDailyForecast, ErrorString) = await WeatherService.GetWeatherData(badWeatherPoint!.Forecast);
        return ErrorString;
    }

    private async Task<string?> GetBadHourlyForecast()
    {
        string? ErrorString;
        (BadHourlyForecast, ErrorString) = await WeatherService.GetWeatherData(badWeatherPoint!.ForecastHourly);
        return ErrorString;
    }

    private async Task<string?> GetGoodHourlyForecast()
    {
        string? ErrorString;
        (RealHourlyForecast, ErrorString) = await WeatherService.GetWeatherData(weatherPoint!.ForecastHourly);
        return ErrorString;
    }
}
