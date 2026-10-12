using UselessApp.Services;

public class CalculatorEvaluatorServiceTests
{
    [Theory]
    [InlineData (new string[] {"5", "+", "5"}, new string[] {"5", "5", "+"})]
    [InlineData (new string[] {"-5", "-", "10"}, new string[] {"-5", "10", "-"})]
    [InlineData (new string[] {"5","+","(","4","*","4",")","-","10"}, new string[] {"5","4","4","*","+","10","-"})]
    public void InFixToRPN_ReturnCorrectRPNList(string[] input, string[] expected)
    {
        expected.ToList();
        var Evaluator = new CalculatorEvaluatorService();
        List<string> rpnString = Evaluator.InfixToRPN(input.ToList());

        Assert.Equal(rpnString, expected);
    }
}