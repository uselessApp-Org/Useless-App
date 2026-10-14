using UselessApp.Services;
namespace UselessApp.UnitTests;
public class CalculatorAndConversionTests
{
    private readonly CalculatorEvaluatorService calculator = new();
    [Theory]
    [InlineData("2+3*4", "14")]
    [InlineData("(2+3)*4", "20")]
    [InlineData("2^3^2", "512")]
    [InlineData("sqrt(81)", "9")]
    [InlineData("sin(30)", "0.5")]
    [InlineData("-2+4", "2")]
    [InlineData("10%3", "1")]
    public void EvaluatesExpressions(string expression, string expected) => Assert.Equal(expected, calculator.Evaluate([expression]));
    [Theory]
    [InlineData("2/0")]
    [InlineData("sqrt(")]
    [InlineData("hello(3)")]
    public void BadExpressionsGiveErrors(string expression) => Assert.StartsWith("Error:", calculator.Evaluate([expression]));
    [Theory]
    [InlineData("Length", "Miles", "Feet", 1, 5280)]
    [InlineData("Mass", "Kilograms", "Grams", 1, 1000)]
    [InlineData("Temperature", "Celsius", "Fahrenheit", 100, 212)]
    [InlineData("Temperature", "Kelvin", "Celsius", 273.15, 0)]
    [InlineData("Time", "Hours", "Minutes", 2, 120)]
    [InlineData("Speed", "Kilometers per hour", "Meters per second", 36, 10)]
    public void ConvertsUnits(string category, string from, string to, double input, double expected) =>
        Assert.Equal(expected, UnitConverter.Convert(category, from, to, input), 7);
    [Fact]
    public void RejectsBelowAbsoluteZero() => Assert.Throws<ArgumentOutOfRangeException>(() => UnitConverter.Convert("Temperature", "Kelvin", "Celsius", -1));
    [Fact]
    public void RejectsIncompatibleUnits() => Assert.Throws<ArgumentException>(() => UnitConverter.Convert("Length", "Meters", "Pounds", 1));
}
