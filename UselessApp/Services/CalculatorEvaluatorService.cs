namespace UselessApp.Services;

public class CalculatorEvaluatorService
{
    public string Evaluate(List<string> tokens)
    {
        if (tokens == null || tokens.Count == 0)
        {
            return "0";
        }

        // Placeholder logic: concatenated string
        // You can replace this later with your full expression parsing/evaluation logic!
        return string.Join("", tokens);
    }
}

