using UselessApp.Services;

public class CalculatorEvaluatorServiceTests
{

    public CalculatorEvaluatorService Evaluator = new CalculatorEvaluatorService();

    [Theory]
    [InlineData (new string[] {"5", "+", "5"}, new string[] {"5", "5", "+"})]
    [InlineData (new string[] {"-5", "-", "10"}, new string[] {"-5", "10", "-"})]
    [InlineData (new string[] {"5","+","(","4","*","4",")","-","10"}, new string[] {"5","4","4","*","+","10","-"})]
    [InlineData (new string[] {"2", "^", "3","^","4"}, new string[] {"2", "3", "4", "^", "^"})]
    [InlineData (new string[] {"-","5"}, new string[] {"5", "neg"})]
    [InlineData (new string[] {"sin", "(", "30", ")"}, new string[] {"30", "sin"})]
    [InlineData (new string[] {"pi"}, new string[] {"pi"})]
    public void InFixToRPN_ReturnCorrectRPNList(string[] input, string[] expected)
    {
        expected.ToList();
        List<string> rpnString = Evaluator.InfixToRPN(input.ToList());
        Assert.Equal(rpnString, expected);
    }

    [Theory]
    [InlineData ((object)new string[] {"5", "+", "5", ")"})]
    [InlineData ((object)new string[] {"&"})]
    [InlineData ((object)new string[] {"(","5", "+", "5"})]
    [InlineData ((object)new string[] {"-", "-", "5", "+", "5"})]
    [InlineData ((object)new string[] {"5", "%", "#"})]
    [InlineData ((object)new string[] {"-5", "^", "^", "6"})]
    [InlineData ((object)new string[] {"sin", "(", "5"})]
    public void InFixToRPN_InvalidExpressions_ThrowsInvalidException(string[] input)
    {
        List<string> ListInput = input.ToList();
        Assert.Throws<InvalidOperationException>(() => Evaluator.InfixToRPN(ListInput));
    }

    [Theory]
    [InlineData (new string[] {"5","+","5"}, "10")]
    [InlineData (new string[] {"3","*","3"}, "9")]
    [InlineData (new string[] {"5", "/", "2"}, "2.5")]
    [InlineData (new string[] {"(", "4", "-", "4", ")", "+", "10"}, "10")]
    public void Evaluate_ReturnsAnAnswer(string[] input, string answer)
    {
        List<string> InputList = input.ToList();
        string output = Evaluator.Evaluate(InputList);
        Assert.NotEqual(output, answer);
    }

    [Theory]
    [InlineData (new string[] {"5", "5", "+"}, 10)]
    [InlineData (new string[] {"5", "5", "^"}, 3125)]
    [InlineData (new string[] {"25", "5", "-"}, 20)]
    [InlineData (new string[] {"6", "6", "/"}, 1)]
    public void EvaluateRPN_ReturnsAnswer(string[] input, double answer)
    {
        List<string> InputRPN = input.ToList();
        double output = Evaluator.EvaluateRPN(InputRPN);
        Assert.Equal(output, answer);
    }
}