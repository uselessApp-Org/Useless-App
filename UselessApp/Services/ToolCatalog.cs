namespace UselessApp.Services;

public static class ToolCatalog
{
    public sealed record AppIdea(string Id, string Name, string Icon, string Color, string Description, string? Route, string Status);
    public static readonly AppIdea[] Apps = [
        new("counter", "Counter", "＋", "mint", "For when counting on your fingers feels too reliable.", "counter", "Ready"),
        new("weather", "Weather", "☀", "peach", "Bring an umbrella. Or sunscreen. We refuse to pick a side.", "weather", "Sample data"),
        new("calculator", "Calculator", "▦", "lilac", "Math with the confidence of a group project at 11:59.", "calculator", "Prototype"),
        new("dictionary", "API Search", "⌕", "yellow", "Words mean things. We are investigating alternatives.", "api-search-term", "Dictionary API"),
        new("converter", "Unit Confuser", "⇄", "mint", "Meters to miles. Celsius to Fahrenheit. Confidence to disappointment.", null, "Demo"),
        new("translator", "Lost in Translation", "文", "lilac", "Translate a sentence into something confidently unrelated.", null, "Demo"),
        new("directions", "Wrong Way", "↗", "peach", "Ask for directions. Get sent in the opposite direction.", null, "Demo"),
        new("timer", "Eventually", "◷", "yellow", "A countdown that keeps finding more time.", null, "Demo"),
        new("recipes", "Recipe for Disaster", "♨", "peach", "Ask for dinner ideas. Receive a recipe for assembling a chair.", null, "Demo"),
        new("trivia", "Definitely Incorrect", "?", "mint", "A trivia game that insists your correct answer is wrong.", null, "Demo"),
        new("music", "Wrong Vibes", "♫", "lilac", "Request relaxing music. Discover the least appropriate genre.", null, "Demo"),
        new("todo", "To-Don't", "✓", "yellow", "Organize everything you absolutely should not do today.", null, "Demo")
    ];
}
