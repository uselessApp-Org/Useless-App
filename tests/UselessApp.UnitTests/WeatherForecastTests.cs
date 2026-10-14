using UselessApp.Models;
namespace UselessApp.UnitTests;

public class WeatherForecastTests
{
    [Theory]
    [InlineData(-20, -3)]
    [InlineData(-10, 15)]
    [InlineData(0, 32)]
    [InlineData(10, 49)]
    [InlineData(20, 67)]
    [InlineData(54, 129)]
    public void TemperatureF_ConvertsCelsiusUsingExistingTruncation(int celsius, int expected)
    {
        var forecast = new WeatherForecast { TemperatureC = celsius };
        Assert.Equal(expected, forecast.TemperatureF);
    }

    [Fact]
    public void TemperatureF_UpdatesWhenCelsiusChanges()
    {
        var forecast = new WeatherForecast { TemperatureC = 0 };
        Assert.Equal(32, forecast.TemperatureF);
        forecast.TemperatureC = 20;
        Assert.Equal(67, forecast.TemperatureF);
    }
}
