namespace UselessApp.Services;

public static class ToolCatalog
{
    public sealed record AppIdea(string Id, string Name, string Icon, string Color, string Description, string? Route, string Status);
    public static readonly AppIdea[] Apps = [
        new("arcade-gym", "Password Strength Gym", "🏋️", "mint", "Your password trains a tiny weightlifter. Entirely browser-side, never sent or saved.", "arcade-extra/gym", "Local · Private"),
        new("arcade-captcha", "Ridiculous CAPTCHA", "🥔", "peach", "Prove humanity by identifying deeply suspicious vegetables.", "arcade-extra/captcha", "Local · Game"),
        new("arcade-rps", "RPS Championship", "✊", "lilac", "Beat peculiar opponents in a best-of-five showdown.", "arcade-extra/rps", "Local · Game"),
        new("arcade-platformer", "One-Button Platformer", "🏃", "mint", "One button, precise jumping and a changing obstacle course.", "arcade-extra/platformer", "Local · Game"),
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
        new("counter", "Counter", "＋", "mint", "For when counting on your fingers feels too reliable.", "counter", "Ready"),
        new("weather", "Weather", "☀", "peach", "Bring an umbrella. Or sunscreen. We refuse to pick a side.", "weather", "Sample data"),
        new("calculator", "Calculator", "▦", "lilac", "Math with the confidence of a group project at 11:59.", "calculator", "Prototype"),
        new("dictionary", "API Search", "⌕", "yellow", "Words mean things. We are investigating alternatives.", "api-search-term", "Dictionary API"),
        new("trivia", "Definitely Incorrect", "?", "mint", "A trivia game that insists your correct answer is wrong.", null, "Demo"),
    ];
}
