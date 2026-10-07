namespace UselessApp.Services;

public static class ToolOptions
{
    public static readonly Dictionary<string, string[]> Units = new()
    {
        ["Length"] = ["Meters", "Kilometers", "Centimeters", "Millimeters", "Inches", "Feet", "Yards", "Miles"],
        ["Mass"] = ["Grams", "Kilograms", "Ounces", "Pounds"],
        ["Temperature"] = ["Celsius", "Fahrenheit", "Kelvin"],
        ["Volume"] = ["Milliliters", "Liters", "US fluid ounces", "US cups", "US gallons"],
        ["Speed"] = ["Meters per second", "Kilometers per hour", "Miles per hour"],
        ["Time"] = ["Seconds", "Minutes", "Hours", "Days"]
    };
    public static readonly Dictionary<string, string> Languages = new()
    {
        ["en"] = "English", ["es"] = "Spanish", ["fr"] = "French", ["de"] = "German",
        ["it"] = "Italian", ["pt"] = "Portuguese", ["ja"] = "Japanese", ["ko"] = "Korean",
        ["zh"] = "Chinese (Simplified)", ["ar"] = "Arabic", ["hi"] = "Hindi", ["ru"] = "Russian"
    };
    public static readonly Dictionary<string, string> TranslationJokes = new()
    {
        ["en"] = "The toaster has requested annual leave.",
        ["es"] = "La tostadora ha solicitado vacaciones.",
        ["fr"] = "Le grille-pain a demandé des vacances.",
        ["de"] = "Der Toaster hat Urlaub beantragt.",
        ["it"] = "Il tostapane ha chiesto le ferie.",
        ["pt"] = "A torradeira pediu férias.",
        ["ja"] = "トースターが休暇を申請しました。",
        ["ko"] = "토스터가 휴가를 신청했습니다.",
        ["zh"] = "烤面包机申请了休假。",
        ["ar"] = "طلبت محمصة الخبز إجازة.",
        ["hi"] = "टोस्टर ने छुट्टी मांगी है।",
        ["ru"] = "Тостер попросил отпуск."
    };
}
