namespace UselessApp.Services;

// Placeholder class to do backend stuff with

public class CalculatorEvaluatorService
{
    public string Evaluate(List<string> tokens)
    {
        if (tokens == null || tokens.Count == 0)
        {
            return "0";
        }

        return string.Join("", tokens);
    }
}

