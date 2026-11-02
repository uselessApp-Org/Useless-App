using UselessApp.Components.WeatherPage.Components;

namespace UselessApp.Components.WeatherPage;

public partial class Weather
{
    // Sample data until the weather API is wired up
    private string location = "Menomonie, WI";
    private string lastUpdated = DateTime.Now.ToString("h:mm tt");
    private List<ForecastItem> hourly = new();
    private List<ForecastItem> daily = new();

    // Location prompt
    private bool showLocationPrompt = true;
    private bool hasLocation = false;
    private GeoLocationResult? selectedLocation;
    public WeatherProperties? weatherPoint;
    public ForecastData? RealHourlyForecast;
    public ForecastData? RealDailyForecast;
    private string? weatherPointError;
    private string? forecastDailyError;
    private string? forecastHourlyError;

    protected override void OnInitialized()
    {
        var rng = new Random(69);
        var now = DateTime.Now;

        for (int i = 0; i < 24; i++)
        {
            string label = i == 0 ? "Now" : now.AddHours(i).ToString("h tt");
            hourly.Add(new ForecastItem(label, rng.Next(45, 66), null, rng.Next(0, 101)));
        }

        for (int i = 0; i < 7; i++)
        {
            string label = i == 0 ? "Today" : now.AddDays(i).ToString("ddd");
            int high = rng.Next(50, 72);
            daily.Add(new ForecastItem(label, high, high - rng.Next(8, 18), rng.Next(0, 101)));
        }
    }

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

        // TEMP: fetch weather point data so it can be inspected on the page
        (weatherPoint, weatherPointError) = await WeatherService.GetWeatherPointData(result.Latitude, result.Longitude);
        if (!string.IsNullOrEmpty(weatherPointError))
        {
            await GetDailyHour();
            await GetHourly();
        }
    }

    private async Task GetDailyHour()
    {
        (RealDailyForecast, forecastDailyError) = await WeatherService.GetWeatherData(weatherPoint.Forecast);
    }

    private async Task GetHourly()
    {
        (RealHourlyForecast, forecastHourlyError) = await WeatherService.GetWeatherData(weatherPoint.ForecastHourly);
    }
}
