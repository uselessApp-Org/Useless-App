using UselessApp.Components.WeatherPage.Components;

namespace UselessApp.Components.WeatherPage;

public partial class Weather
{
    // Sample data until the weather API is wired up
    private string location = "Menomonie, WI";
    private string lastUpdated = DateTime.Now.ToString("h:mm tt");
    private int currentTemp = 58;
    private string condition = "Partly Indecisive";
    private string conditionIcon = "⛅";
    private int humidity = 64;
    private string windDirection = "NW";
    private int windDeg = 315;
    private int windSpeed = 12;
    private int feelsLike = 55;

    private readonly string[] icons = { "☀", "⛅", "☁", "🌧", "⛈", "🌨" };
    private List<ForecastItem> hourly = new();
    private List<ForecastItem> daily = new();

    // Location prompt
    private bool showLocationPrompt = true;
    private bool hasLocation = false;
    private bool isSearching = false;
    private string cityInput = string.Empty;
    private string? locationError;
    private GeoLocationResult? selectedLocation;
    private WeatherProperties? weatherPoint;
    private string? weatherPointError;

    protected override void OnInitialized()
    {
        var rng = new Random(69);
        var now = DateTime.Now;

        for (int i = 0; i < 24; i++)
        {
            string label = i == 0 ? "Now" : now.AddHours(i).ToString("h tt");
            hourly.Add(new ForecastItem(label, icons[rng.Next(icons.Length)], rng.Next(45, 66), null, rng.Next(0, 101)));
        }

        for (int i = 0; i < 7; i++)
        {
            string label = i == 0 ? "Today" : now.AddDays(i).ToString("ddd");
            int high = rng.Next(50, 72);
            daily.Add(new ForecastItem(label, icons[rng.Next(icons.Length)], high, high - rng.Next(8, 18), rng.Next(0, 101)));
        }
    }

    private void OpenLocationPrompt()
    {
        cityInput = string.Empty;
        locationError = null;
        showLocationPrompt = true;
    }

    private async Task SubmitCity()
    {
        if (string.IsNullOrWhiteSpace(cityInput))
            return;

        isSearching = true;
        locationError = null;

        try
        {
            var result = await GeoLocation.GetLocationData(cityInput);
            if (result is null)
            {
                locationError = $"Couldn't find \"{cityInput.Trim()}\". Try another city.";
                return;
            }

            selectedLocation = result;
            location = string.IsNullOrEmpty(result.Admin1)
                ? $"{result.Name}, {result.Country}"
                : $"{result.Name}, {result.Admin1}";
            lastUpdated = DateTime.Now.ToString("h:mm tt");
            hasLocation = true;
            showLocationPrompt = false;

            // TEMP: fetch weather point data so it can be inspected on the page
            (weatherPoint, weatherPointError) = await WeatherService.GetWeatherPointData(result.Latitude, result.Longitude);
        }
        catch (HttpRequestException)
        {
            locationError = "Location lookup failed. Check your connection and try again.";
        }
        finally
        {
            isSearching = false;
        }
    }
}
