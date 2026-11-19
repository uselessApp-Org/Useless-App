using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace UselessApp.Services;

public class UserStatsService
{
    public int CalcEnters { get; set; }
    public string LastExpression { get; set; } = string.Empty;
    public string LastResult { get; set; } = string.Empty;
    public string WeatherLocation { get; set; } = string.Empty;
    public float Latitude { get; set; }
    public float Longitude { get; set; }
    public string LastWordSearched { get; set; } = string.Empty;
    public string LastDefinition { get; set; } = string.Empty;
    public bool IsDirty { get; set; }

    public void RecordCalcEnter(string expression, string result)
    {
        CalcEnters++;
        LastExpression = expression;
        LastResult = result;
        IsDirty = true;
    }

    public void Reset()
    {
        CalcEnters = 0;
        LastExpression = string.Empty;
        LastResult = string.Empty;
        WeatherLocation = string.Empty;
        Latitude = 0.0f;
        Longitude = 0.0f;
        LastWordSearched = string.Empty;
        LastDefinition = string.Empty;
        IsDirty = false;
    }

    public void RecordWeatherLocation(string weatherLocation, float latitude, float longitude)
    {
        WeatherLocation = weatherLocation;
        Latitude = latitude;
        Longitude = longitude;
        IsDirty = true;
    }

    public void RecordDictionarySearch(string lastdefinition, string lastwordsearched)
    {
        LastWordSearched = lastwordsearched;
        LastDefinition = lastdefinition;
        IsDirty = true;
    }
}
