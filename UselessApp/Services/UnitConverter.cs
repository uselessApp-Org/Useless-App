using System.Globalization;
namespace UselessApp.Services;

public static class UnitConverter
{
    private static readonly Dictionary<string, Dictionary<string, double>> Factors = new()
    {
        ["Length"] = new() { ["Meters"] = 1, ["Kilometers"] = 1000, ["Centimeters"] = .01, ["Millimeters"] = .001, ["Inches"] = .0254, ["Feet"] = .3048, ["Yards"] = .9144, ["Miles"] = 1609.344 },
        ["Mass"] = new() { ["Grams"] = 1, ["Kilograms"] = 1000, ["Ounces"] = 28.349523125, ["Pounds"] = 453.59237 },
        ["Volume"] = new() { ["Milliliters"] = 1, ["Liters"] = 1000, ["US fluid ounces"] = 29.5735295625, ["US cups"] = 236.5882365, ["US gallons"] = 3785.411784 },
        ["Speed"] = new() { ["Meters per second"] = 1, ["Kilometers per hour"] = 1.0/3.6, ["Miles per hour"] = .44704 },
        ["Time"] = new() { ["Seconds"] = 1, ["Minutes"] = 60, ["Hours"] = 3600, ["Days"] = 86400 }
    };
    public static double Convert(string category, string from, string to, double amount)
    {
        if (!double.IsFinite(amount)) throw new ArgumentException("Amount must be finite");
        if (!ToolOptions.Units.TryGetValue(category, out var units) || !units.Contains(from) || !units.Contains(to)) throw new ArgumentException("Invalid unit selection");
        if (category == "Temperature")
        {
            var celsius = from switch { "Celsius" => amount, "Fahrenheit" => (amount - 32)*5/9, "Kelvin" => amount - 273.15, _ => throw new ArgumentException("Unknown temperature unit") };
            if (celsius < -273.15 - 0.0000001) throw new ArgumentOutOfRangeException(nameof(amount), "Temperature cannot be below absolute zero");
            return to switch { "Celsius" => celsius, "Fahrenheit" => celsius * 9/5 + 32, "Kelvin" => celsius + 273.15, _ => throw new ArgumentException("Unknown temperature unit") };
        }
        return amount * Factors[category][from] / Factors[category][to];
    }
    public static string Format(double value) => value.ToString("G12", CultureInfo.InvariantCulture);
}
