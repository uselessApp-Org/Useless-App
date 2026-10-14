using System.Net.Http.Json;
using System.Text.Json;
namespace UselessApp.Services;

public sealed record ExperimentResult(string Text, string Source);

// Replace this interface implementation to use a vendor SDK or a different response schema.
public interface IExperimentService
{
    bool IsConfigured(string id);
    Task<ExperimentResult> RunAsync(string id, string input, bool connected, IReadOnlyDictionary<string, string>? options = null, CancellationToken cancellationToken = default);
}

public sealed class ExperimentService(IHttpClientFactory clients, IConfiguration configuration) : IExperimentService
{
    public bool IsConfigured(string id) => TryEndpoint(id, out _);
    private bool TryEndpoint(string id, out Uri? endpoint)
    {
        endpoint = null;
        return ToolCatalog.Apps.Any(a => a.Id == id && a.Route == null)
            && Uri.TryCreate(configuration[$"ToolProviders:{id}:Endpoint"], UriKind.Absolute, out endpoint)
            && endpoint.Scheme == Uri.UriSchemeHttps;
    }
    public async Task<ExperimentResult> RunAsync(string id, string input, bool connected, IReadOnlyDictionary<string, string>? options = null, CancellationToken cancellationToken = default)
    {
        if (!ToolCatalog.Apps.Any(a => a.Id == id && a.Route == null)) throw new ArgumentException("Unknown experiment.");
        if (string.IsNullOrWhiteSpace(input)) return new("Give me something to misunderstand first.", "Demo");
        if (id == "converter" && options != null)
        {
            if (!options.TryGetValue("category", out var category) || !ToolOptions.Units.TryGetValue(category, out var units)
                || !options.TryGetValue("fromUnit", out var from) || !units.Contains(from)
                || !options.TryGetValue("toUnit", out var to) || !units.Contains(to) || from == to
                || !options.TryGetValue("amount", out var amount) || !decimal.TryParse(amount, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out _))
                return new("Choose an amount and two different units from the same category.", "Validation error");
        }
        if (id == "translator" && options != null)
        {
            if (!options.TryGetValue("sourceLanguage", out var from) || (from != "auto" && !ToolOptions.Languages.ContainsKey(from))
                || !options.TryGetValue("targetLanguage", out var to) || !ToolOptions.Languages.ContainsKey(to) || from == to)
                return new("Choose different source and target languages.", "Validation error");
        }
        if (!connected) return new(Demo(id, input.Trim(), options), "Local demo");
        if (!TryEndpoint(id, out var endpoint)) return new("This connection needs setup. Try the demo in the meantime.", "Not configured");
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Content = JsonContent.Create(new { tool = id, input, options, instruction = "Return a clearly fictional, comically incorrect answer. Avoid actionable dangerous advice." });
            var key = configuration[$"ToolProviders:{id}:ApiKey"];
            if (!string.IsNullOrEmpty(key)) request.Headers.Authorization = new("Bearer", key);
            using var response = await clients.CreateClient("experiments").SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode) return new("The connection had a moment. Try again or switch to Demo.", "Connection error");
            var payload = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
            if (payload.ValueKind != JsonValueKind.Object || !payload.TryGetProperty("result", out var result) || result.ValueKind != JsonValueKind.String)
                return new("The provider returned something we cannot read. Expected a result string.", "Response error");
            return new(result.GetString() ?? "An impressively empty answer.", "Connected provider");
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested) { return new("The provider took a coffee break. Try again.", "Timed out"); }
        catch (HttpRequestException) { return new("Could not reach the provider. Demo mode is still available.", "Connection error"); }
        catch (JsonException) { return new("The provider sent an unreadable answer.", "Response error"); }
    }
    private static string Demo(string id, string input, IReadOnlyDictionary<string, string>? options)
    {
        if (id == "converter" && options != null)
        {
            var amount = decimal.Parse(options["amount"], System.Globalization.CultureInfo.InvariantCulture);
            // A deliberately bogus scale; stays in range even for extreme decimal inputs.
            var wrong = amount / 2 + 17;
            return $"{amount.ToString("G", System.Globalization.CultureInfo.InvariantCulture)} {options["fromUnit"]} → {wrong.ToString("G", System.Globalization.CultureInfo.InvariantCulture)} {options["toUnit"]}\nConversion factor approved by a spoon. This is intentionally incorrect.";
        }
        if (id == "translator" && options != null)
        {
            var source = options["sourceLanguage"] == "auto" ? "Auto-detect" : ToolOptions.Languages[options["sourceLanguage"]];
            var target = ToolOptions.Languages[options["targetLanguage"]];
            return $"{source} → {target}\n{ToolOptions.TranslationJokes[options["targetLanguage"]]}\n\nThe language is right. The meaning took a different bus.";
        }
        return id switch
    {
        "converter" => $"{input} = 12 emotional support potatoes. Conversion rate subject to vibes.",
        "translator" => $"Translation of ‘{input}’: The toaster has requested annual leave.",
        "directions" => $"To reach ‘{input}’, turn left at the concept of Thursday. This is fictional; please use a real map.",
        "timer" => $"Timer for ‘{input}’ started in spirit. Estimated completion: after one more episode.",
        "recipes" => $"For ‘{input}’: arrange three imaginary chairs around a table. Serves nobody. Fictional recipe; do not eat the furniture.",
        "trivia" => $"‘{input}’? Obviously the answer is a fax machine wearing a tiny hat. Our imaginary judges are unanimous.",
        "music" => $"Playlist for ‘{input}’: Printer Jam (10-hour remix), Dial-Up Solo, and Someone Vacuuming Upstairs.",
        "todo" => $"Your plan for ‘{input}’: 1. Open a tab. 2. Open 17 more. 3. Wonder why you entered this room.",
        _ => "The department of answers is out to lunch."
    };
    }
}
