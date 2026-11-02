namespace UselessApp.Components.WeatherPage.Components;

public record CurrentConditions(string Condition, string Icon, int Temp, int FeelsLike, int Humidity, string WindDirection, int WindDeg, int WindSpeed);
