namespace UselessApp.Services;

using System.Text.RegularExpressions;

public class CalculatorEvaluatorService
{
    private Stack<string> OperatorStack = new Stack<string>();
    private List<string> RPN_List = new List<string>();


    public string Evaluate(List<string> RPN_TOKENS)
    {
        
        return "turds";
    }

    public List<string> infixToRPN(List<string> tokens)
    {
        foreach(string curToken in tokens)
        {
            

        }

        return RPN_List;
    }

    public record OpProps(int Precedence, char Associativity);

    private readonly Dictionary<string, OpProps> operators = new Dictionary<string, OpProps>
    {
        {"+", new OpProps(1, 'L') },
        {"-", new OpProps(1, 'L') },
        {"*", new OpProps(2, 'L') },
        {"/", new OpProps(2, 'L') },
        {"%", new OpProps(2, 'L') },
        {"^", new OpProps(3, 'R') },
        {"(", new OpProps(4, 'L') },
        
    };
}

