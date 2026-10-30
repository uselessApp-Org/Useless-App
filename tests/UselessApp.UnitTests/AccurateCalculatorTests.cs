using UselessApp.Services;

namespace UselessApp.UnitTests;

public class AccurateCalculatorTests
{
    [Theory]
    [InlineData(new[] { "1", "+", "1" }, "2")]
    [InlineData(new[] { "12", "+", "3" }, "15")]
    [InlineData(new[] { "2", "+", "3", "*", "4" }, "14")]
    [InlineData(new[] { "(", "24", "+", "6", ")", "/", "2" }, "15")]
    public void ArcadeEvaluation_UsesActualOperators(string[] tokens, string expected)
    {
        var evaluator = new CalculatorEvaluatorService();
        for (var i = 0; i < 20; i++)
            Assert.Equal(expected, evaluator.Evaluate(tokens.ToList(), scrambleOperators: false));
    }
}
