using System.Text;
using System.Text.Json;

public class GeminiRequest
{
    public Content? SystemInstruction { get; set; }
    public List<Content> Contents { get; set; } = new();
}

public class Content
{
    public List<Part> Parts { get; set; } = new();
}

public class Part
{
    public string Text { get; set; } = string.Empty;
}

public class GeminiResponse
{
    public List<Candidate>? Candidates { get; set; }
}

public class Candidate
{
    public Content? Content { get; set; }
}

public class GeminiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public GeminiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Gemini:ApiKey"] 
            ?? throw new ArgumentNullException("Gemini API key is not configured.");
    }

    public async Task<string> GenerateTextAsync(string prompt)
{
    var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.5-flash:generateContent?key={_apiKey}";

    var requestBody = new GeminiRequest
    {
        // Define system rules here
        SystemInstruction = new Content
        {
            Parts = new List<Part>
            {
                new Part 
                { 
                    Text = "You are an AI assistant for joke Blazor Web application. " +
                           "ALWAYS follow these rules:\n" +
                           "1. Keep answers brief and easy to read.\n" +
                           "2. Never give the correct answer but keep answers plausible.\n" +
                           "3. Maintain a humorous tone."
                }
            }
        },
        Contents = new List<Content>
        {
            new Content { Parts = new List<Part> { new Part { Text = prompt } } }
        }
    };

    var json = JsonSerializer.Serialize(requestBody, new JsonSerializerOptions 
    { 
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
    });

    int maxRetries = 3;
    int delaySeconds = 2;

    for (int i = 0; i < maxRetries; i++)
    {
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GeminiResponse>(responseJson, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            });

            return result?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text 
                   // Fallback if parts list is empty or structure varies
                   ?? "No response text generated.";
        }

        // If it's a 503 Service Unavailable or 429 Too Many Requests, wait and retry
        if ((int)response.StatusCode == 503 || (int)response.StatusCode == 429)
        {
            if (i == maxRetries - 1) 
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Gemini API is currently overloaded (503): {error}");
            }

            // Wait before trying again (exponential backoff: 2s, then 4s, etc.)
            await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
            delaySeconds *= 2; 
            continue;
        }

        // For other errors (like invalid keys), throw immediately without retrying
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Gemini API Error ({response.StatusCode}): {error}");
        }
    }

    throw new Exception("Failed to get a response from Gemini after multiple attempts.");
}
}