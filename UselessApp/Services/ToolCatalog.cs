namespace UselessApp.Services;

public static class ToolCatalog
{
    public sealed record AppIdea(string Id, string Name, string Icon, string Color, string Description, string? Route, string Status);
    public static readonly AppIdea[] Apps = [
        new("arcade-converter", "Bananas for Scale", "🍌", "mint", "Convert 100 school buses to giraffes, Eiffel Towers, and hundreds of other estimates.", "arcade/converter", "Arcade · Local"),
        new("arcade-calculator", "Arcade Calculator", "🎰", "lilac", "Catch a runaway equals button, try the slot machine, or fight math bosses.", "arcade/calculator", "Arcade · Local"),
        new("arcade-counter", "Click Speed & Achievements", "🏆", "yellow", "Count clicks, track CPS, and unlock ridiculous achievements.", "arcade/counter", "Arcade · Local"),
        new("arcade-todo", "Excuse Office", "📝", "peach", "Submit a task and receive a deeply official excuse.", "arcade/todo", "Arcade · Local"),
        new("arcade-translator", "Slang & Personality Translator", "🌐", "lilac", "Translate to and from Gen Z slang and several theatrical voices.", "arcade/translator", "Arcade · Local"),
        new("arcade-travel", "Trip Fortune", "🔮", "yellow", "Find your destination's prophecy and a fictional weather forecast.", "arcade/travel", "Arcade · Local"),
        new("arcade-trivia", "Trick Trivia", "❓", "mint", "Three answers fit. One is just a little ridiculous.", "arcade/trivia", "Arcade · Local"),
        new("arcade-sounds", "Pointless Soundboard", "🎶", "peach", "Make applause, explosions, record scratches, and more.", "arcade/soundboard", "Arcade · Local"),
        new("arcade-rock", "Pet Rock Sanctuary", "🪨", "yellow", "Take care of something that absolutely doesn't need it.", "arcade/rock", "Arcade · Local"),
        new("arcade-bubbles", "Bubble Wrap", "🫧", "mint", "Pop your way to the world's least meaningful fortune.", "arcade/bubbles", "Arcade · Local"),
        new("counter", "Classic Counter", "＋", "mint", "The traditional counter, for anyone who prefers the original.", "counter", "Classic"),
        new("weather", "Weather", "☀", "peach", "Weather data and an umbrella suggestion of dubious quality.", "weather", "Sample data"),
        new("calculator", "Classic Calculator", "▦", "lilac", "A normal calculator in an abnormal building.", "calculator", "Classic"),
        new("dictionary", "API Search", "⌕", "yellow", "Look up real words with a free public dictionary API.", "api-search-term", "Connected · no key"),
    ];
}
