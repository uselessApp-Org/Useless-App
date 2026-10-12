namespace UselessApp.Services;

using System.Globalization;

public class CalculatorEvaluatorService
{
    public record OpProps(int Precedence, char Associativity);

    private readonly Dictionary<string, OpProps> operators = new Dictionary<string, OpProps>
    {
        { "+",    new OpProps(1, 'L') },
        { "-",    new OpProps(1, 'L') },
        { "*",    new OpProps(2, 'L') },
        { "/",    new OpProps(2, 'L') },
        { "%",    new OpProps(2, 'L') },
        { "^",    new OpProps(3, 'R') },
        { "sin",  new OpProps(4, 'R') },
        { "cos",  new OpProps(4, 'R') },
        { "tan",  new OpProps(4, 'R') },
        { "asin", new OpProps(4, 'R') },
        { "acos", new OpProps(4, 'R') },
        { "atan", new OpProps(4, 'R') },
        { "sqrt", new OpProps(4, 'R') },
        { "neg",  new OpProps(4, 'R') },
        { "(",    new OpProps(0, 'L') }
    };

    private readonly HashSet<string> functions = new HashSet<string>
    {
        "sin", "cos", "tan", "asin", "acos", "atan", "sqrt"
    };

    public string Evaluate(List<string> tokens)
    {
        if (tokens == null || tokens.Count == 0)
        {
            return "0";
        }

        try
        {
            List<string> rpn = InfixToRPN(tokens);
            double result = EvaluateRPN(rpn);

            if (double.IsNaN(result))
            {
                return "Error: NaN";
            }
            if (double.IsInfinity(result))
            {
                return "Error: Divide by 0";
            }

            // Return formatted number
            return Math.Round(result, 8).ToString(CultureInfo.InvariantCulture);
        }
        catch (Exception ex)
        {
            return string.IsNullOrEmpty(ex.Message) ? "Error" : $"Error: {ex.Message}";
        }
    }

    public List<string> InfixToRPN(List<string> tokens)
    {
        var rpnList = new List<string>();
        var operatorStack = new Stack<string>();

        for (int i = 0; i < tokens.Count; i++)
        {
            string curToken = tokens[i];

            // Detect unary minus (e.g. at the start of expression or right after an operator or '(')
            if (curToken == "-")
            {
                bool isUnary = i == 0 || tokens[i - 1] == "(" || operators.ContainsKey(tokens[i - 1]) || functions.Contains(tokens[i - 1]);
                if (isUnary)
                {
                    curToken = "neg";
                }
            }

            if (double.TryParse(curToken, NumberStyles.Any, CultureInfo.InvariantCulture, out _) || curToken == "pi")
            {
                rpnList.Add(curToken);
            }
            else if (functions.Contains(curToken) || curToken == "(")
            {
                operatorStack.Push(curToken);
            }
            else if (curToken == ")")
            {
                while (operatorStack.Count > 0 && operatorStack.Peek() != "(")
                {
                    rpnList.Add(operatorStack.Pop());
                }

                if (operatorStack.Count == 0)
                {
                    throw new InvalidOperationException("Mismatched parentheses");
                }

                operatorStack.Pop(); // Discard '('

                if (operatorStack.Count > 0 && functions.Contains(operatorStack.Peek()))
                {
                    rpnList.Add(operatorStack.Pop());
                }
            }
            else if (operators.TryGetValue(curToken, out var curOp))
            {
                while (operatorStack.Count > 0 && operators.TryGetValue(operatorStack.Peek(), out var topOp))
                {
                    if (operatorStack.Peek() == "(")
                    {
                        break;
                    }

                    bool higherPrecedence = topOp.Precedence > curOp.Precedence;
                    bool equalAndLeftAssoc = topOp.Precedence == curOp.Precedence && curOp.Associativity == 'L';

                    if (higherPrecedence || equalAndLeftAssoc)
                    {
                        rpnList.Add(operatorStack.Pop());
                    }
                    else
                    {
                        break;
                    }
                }

                operatorStack.Push(curToken);
            }
            else
            {
                throw new InvalidOperationException($"Invalid token '{curToken}'");
            }
        }

        while (operatorStack.Count > 0)
        {
            string op = operatorStack.Pop();
            if (op == "(")
            {
                throw new InvalidOperationException("Mismatched parentheses");
            }
            rpnList.Add(op);
        }

        return rpnList;
    }

    private double EvaluateRPN(List<string> rpnTokens)
    {
        var stack = new Stack<double>();

        foreach (string token in rpnTokens)
        {
            if (double.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out double num))
            {
                stack.Push(num);
            }
            else if (token == "pi")
            {
                stack.Push(Math.PI);
            }
            else if (functions.Contains(token))
            {
                if (stack.Count < 1)
                {
                    throw new InvalidOperationException($"Missing operand for {token}");
                }

                double val = stack.Pop();
                double res = token switch
                {
                    "sin" => Math.Sin(val),
                    "cos" => Math.Cos(val),
                    "tan" => Math.Tan(val),
                    "asin" => Math.Asin(val),
                    "acos" => Math.Acos(val),
                    "atan" => Math.Atan(val),
                    "sqrt" => val < 0 ? throw new InvalidOperationException("Negative square root") : Math.Sqrt(val),
                    _ => throw new InvalidOperationException($"Unknown function {token}")
                };

                stack.Push(res);
            }
            else if (token == "neg")
            {
                if (stack.Count < 1)
                {
                    throw new InvalidOperationException("Missing operand for negation");
                }

                stack.Push(-stack.Pop());
            }
            else if (operators.ContainsKey(token))
            {
                if (stack.Count < 2)
                {
                    throw new InvalidOperationException($"Missing operands for {token}");
                }

                double right = stack.Pop();
                double left = stack.Pop();

                double res = token switch
                {
                    "+" => left + right,
                    "-" => left - right,
                    "*" => left * right,
                    "/" => right == 0 ? throw new DivideByZeroException() : left / right,
                    "%" => right == 0 ? throw new DivideByZeroException() : left % right,
                    "^" => Math.Pow(left, right),
                    _ => throw new InvalidOperationException($"Unknown operator {token}")
                };

                stack.Push(res);
            }
            else
            {
                throw new InvalidOperationException($"Unrecognized token {token}");
            }
        }

        if (stack.Count != 1)
        {
            throw new InvalidOperationException("Malformed expression");
        }

        return stack.Pop();
    }
}