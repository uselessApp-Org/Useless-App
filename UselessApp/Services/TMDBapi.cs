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

    public async Task<MovieRecommendation> GetMovieRecommendation(int genreId)
    {
        var apiKey = _configuration["MovieAPI:ApiKey"];

       

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
    public async Task<List<Genre>> GetGenres()
    {
        var apiKey = _configuration["MovieAPI:ApiKey"];
        var url = $"https://api.themoviedb.org/3/genre/movie/list?api_key={apiKey}";
        var json = await _httpClient.GetStringAsync(url);

        using var document = JsonDocument.Parse(json);
        var genres = new List<Genre>();

        foreach (var genre in document.RootElement.GetProperty("genres").EnumerateArray())
        {
            genres.Add(new Genre
            {
                Id = genre.GetProperty("id").GetInt32(),
                Name = genre.GetProperty("name").GetString() ?? string.Empty
            });
        }

        return genres;
    }
}