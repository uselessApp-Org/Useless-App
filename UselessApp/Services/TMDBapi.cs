using System.Text.Json;
using UselessApp.model;
namespace UselessApp.Services;

public class TMDBService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public TMDBService(
        IConfiguration configuration,
        HttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public async Task<MovieRecommendation> GetMovieRecommendation(string genre)
    {
        var apiKey = _configuration["MovieAPI:ApiKey"];

        var genres = new Dictionary<string, int>
        {
            { "Action", 28 },
            { "Comedy", 35 },
            { "Drama", 18 },
            { "Horror", 27 },
            { "Romance", 10749 },
            { "SciFi", 878 }
        };

        if (!genres.TryGetValue(genre, out int genreId))
        {
            return new MovieRecommendation { Title = "Genre not found." };
        }

        var url =
            $"https://api.themoviedb.org/3/discover/movie" +
            $"?api_key={apiKey}" +
            $"&with_genres={genreId}" +
            $"&vote_count.gte=100";

        var json = await _httpClient.GetStringAsync(url);

        using var document = JsonDocument.Parse(json);

        var results = document.RootElement
            .GetProperty("results");

        if (results.GetArrayLength() == 0)
        {
            return new MovieRecommendation { Title = "No movies found." };
        }

        var random = new Random();

        var movie =
            results[random.Next(results.GetArrayLength())];

       var posterPath = movie.GetProperty("poster_path").GetString();

return new MovieRecommendation
{
    Title = movie.GetProperty("title").GetString()
        ?? "No title found.",

    Overview = movie.GetProperty("overview").GetString()
        ?? "No overview found.",

    PosterUrl = string.IsNullOrEmpty(posterPath)
        ? ""
        : $"https://image.tmdb.org/t/p/w500{posterPath}"
};
    }
}