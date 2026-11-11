using System.Text.Json;
using System.Text.Json.Serialization;

#region RainViewer Data
public class RainViewerMapsResponse
{
    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    // unix timestamp (UTC) of when this index was generated
    [JsonPropertyName("generated")]
    public long Generated { get; set; }

    // base URL for every tile, e.g. https://tilecache.rainviewer.com
    [JsonPropertyName("host")]
    public string Host { get; set; } = string.Empty;

    [JsonPropertyName("radar")]
    public RainViewerRadar? Radar { get; set; }
}

public class RainViewerRadar
{
    // past 2 hours in 10 minute steps, oldest first
    [JsonPropertyName("past")]
    public List<RadarFrame> Past { get; set; } = new List<RadarFrame>();
}

public class RadarFrame
{
    [JsonPropertyName("time")]
    public long Time { get; set; }

    // e.g. /v2/radar/d524f3197a43 (just use it as-is, it's not always a timestamp)
    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;
}
#endregion

public class RainViewerService
{
    private readonly HttpClient _httpClient;
    private const string mapsURL = "https://api.rainviewer.com/public/weather-maps.json";
    private const int MaxZoom = 7;

    public RainViewerService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Gets the list of available radar frames from RainViewer
    /// </summary>
    /// <returns>the maps index (host + frames) or an error string</returns>
    public async Task<(RainViewerMapsResponse? Maps, string? Error)> GetRadarMaps()
    {
        HttpResponseMessage responseMessage = await _httpClient.GetAsync(mapsURL);

        if (!responseMessage.IsSuccessStatusCode)
            return (null, $"Error: {(int)responseMessage.StatusCode} {responseMessage.ReasonPhrase}");

        string mapsString = await responseMessage.Content.ReadAsStringAsync();
        RainViewerMapsResponse? maps = JsonSerializer.Deserialize<RainViewerMapsResponse>(mapsString);

        if (maps is null || string.IsNullOrEmpty(maps.Host) || maps.Radar is null || maps.Radar.Past.Count == 0)
            return (null, "RainViewer returned no radar frames.");

        return (maps, null);
    }

    /// <summary>
    /// Builds the URL for one map tile (standard z/x/y slippy map tile) so radar can be laid over a base map
    /// </summary>
    /// <param name="zoom">0-7 (RainViewer max is 7)</param>
    /// <param name="size">256 or 512</param>
    /// <param name="color">color scheme id (2 = Universal Blue)</param>
    public string BuildRadarTileUrl(string host, RadarFrame frame, int zoom, int x, int y, int size = 256, int color = 2, bool smooth = true, bool snow = false)
    {
        zoom = Math.Clamp(zoom, 0, MaxZoom);
        size = size == 512 ? 512 : 256;
        string options = $"{(smooth ? 1 : 0)}_{(snow ? 1 : 0)}";

        return $"{host}{frame.Path}/{size}/{zoom}/{x}/{y}/{color}/{options}.png";
    }
}
