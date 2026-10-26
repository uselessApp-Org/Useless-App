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

    private readonly Random rng = new();

    /// <summary>
    /// Evaluates the user string and returns an answer
    /// </summary>
    /// <param name="tokens"></param>
    /// <returns>an answer to the expression or an error </returns>
    public string Evaluate(List<string> tokens, bool scrambleOperators = true)
    {
        if (tokens == null || tokens.Count == 0)
        {
            return "0";
        }

        try
        {
            List<string> rpn = InfixToRPN(tokens);
            List<string> corruptedRpn = scrambleOperators ? CorruptRPN(rpn) : rpn;
            double result = EvaluateRPN(corruptedRpn);

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

    /// <summary>
    /// Takes the RPN List and randomizes operators by precedence to generate
    /// a randomish output
    /// </summary>
    /// <param name="rpn"></param>
    /// <returns>an RPN String that is modified from what user inputed</returns>
    public List<string> CorruptRPN(List<string> rpn)
    {
        var corrupted = new List<string>(rpn);

        var swapGroups = new Dictionary<string, string[]>
        {
            { "+", new[] { "-" } },
            { "-", new[] { "+" } },
            { "*", new[] { "/", "%" } },
            { "/", new[] { "*", "%" } },
            { "%", new[] { "*", "/" } },
            { "sin",  new[] { "cos", "tan", "asin", "acos", "atan", "sqrt" } },
            { "cos",  new[] { "sin", "tan", "asin", "acos", "atan", "sqrt" } },
            { "tan",  new[] { "sin", "cos", "asin", "acos", "atan", "sqrt" } },
            { "asin", new[] { "sin", "cos", "tan", "acos", "atan", "sqrt" } },
            { "acos", new[] { "sin", "cos", "tan", "asin", "atan", "sqrt" } },
            { "atan", new[] { "sin", "cos", "tan", "asin", "acos", "sqrt" } },
            { "sqrt", new[] { "sin", "cos", "tan", "asin", "acos", "atan" } }
        };

        var swappableIndices = corrupted
            .Select((token, idx) => (token, idx))
            .Where(x => swapGroups.ContainsKey(x.token))
            .Select(x => x.idx)
            .ToList();

        if (swappableIndices.Count > 0)
        {
            foreach (int idx in swappableIndices)
            {
                string current = corrupted[idx];
                string[] alternatives = swapGroups[current];
                corrupted[idx] = alternatives[rng.Next(alternatives.Length)];
            }
        }

        return corrupted;
    }

    /// <summary>
    /// Converts an InFix List<string> to an RPN version and validates expression syntax
    /// </summary>
    /// <param name="tokens"></param>
    /// <returns>List<string> in RPN format</returns>
    /// <exception cref="InvalidOperationException">throws for invalid syntax, mismatched (), or unknown symbols</exception>
    public List<string> InfixToRPN(List<string> tokens)
    {
        if (tokens == null || tokens.Count == 0)
        {
            throw new InvalidOperationException("Expression cannot be empty");
        }

        var rpnList = new List<string>();
        var operatorStack = new Stack<string>();

        // Validation state machine:
        // An expression starts expecting an operand (number, constant, function, '(', or unary minus).
        bool expectOperand = true;

        bool prevWasUnaryMinus = false;

        for (int i = 0; i < tokens.Count; i++)
        {
            string curToken = tokens[i];

            // Detect unary minus
            if (curToken == "-")
            {
                if (prevWasUnaryMinus)
                {
                    throw new InvalidOperationException("Consecutive negation operators are not allowed");
                }

                bool isUnary = i == 0 || tokens[i - 1] == "(" || operators.ContainsKey(tokens[i - 1]) || functions.Contains(tokens[i - 1]);
                if (isUnary)
                {
                    curToken = "neg";
                    prevWasUnaryMinus = true;
                }
            }
            else
            {
                prevWasUnaryMinus = false;
            }

            if (curToken == "neg")
            {
                if (!expectOperand)
                {
                    throw new InvalidOperationException("Unexpected negation operator");
                }
                operatorStack.Push(curToken);
                // After 'neg', we still expect an operand
            }
            else if (double.TryParse(curToken, NumberStyles.Any, CultureInfo.InvariantCulture, out _) || curToken == "pi")
            {
                if (!expectOperand)
                {
                    throw new InvalidOperationException($"Unexpected number or constant '{curToken}'");
                }
                rpnList.Add(curToken);
                expectOperand = false;
            }
            else if (functions.Contains(curToken))
            {
                if (!expectOperand)
                {
                    throw new InvalidOperationException($"Unexpected function '{curToken}'");
                }
                operatorStack.Push(curToken);
                // After a function, an opening parenthesis or operand is expected
            }
            else if (curToken == "(")
            {
                if (!expectOperand)
                {
                    throw new InvalidOperationException("Unexpected '('");
                }
                operatorStack.Push(curToken);
                expectOperand = true;
            }
            else if (curToken == ")")
            {
                if (expectOperand)
                {
                    throw new InvalidOperationException("Empty or incomplete parentheses");
                }

                while (operatorStack.Count > 0 && operatorStack.Peek() != "(")
                {
                    rpnList.Add(operatorStack.Pop());
                }

                if (operatorStack.Count == 0)
                {
                    throw new InvalidOperationException("Mismatched parentheses");
                }

                operatorStack.Pop(); // Discard '('

                if (operatorStack.Count > 0 && (functions.Contains(operatorStack.Peek()) || operatorStack.Peek() == "neg"))
                {
                    rpnList.Add(operatorStack.Pop());
                }

                expectOperand = false;
            }
            else if (operators.TryGetValue(curToken, out var curOp))
            {
                if (expectOperand)
                {
                    throw new InvalidOperationException($"Unexpected operator '{curToken}'");
                }

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
                expectOperand = true;
            }
            else
            {
                throw new InvalidOperationException($"Invalid token '{curToken}'");
            }
        }

        if (expectOperand)
        {
            throw new InvalidOperationException("Expression ends with an incomplete operation");
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

    /// <summary>
    /// takes an RPN List and returns the value
    /// </summary>
    /// <param name="rpnTokens"></param>
    /// <returns>double that is the calculated value of the expression</returns>
    public double EvaluateRPN(List<string> rpnTokens)
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
                double val = stack.Pop();
                stack.Push(ApplyFunction(token, val));
            }
            else if (token == "neg")
            {
                stack.Push(-stack.Pop());
            }
            else if (operators.ContainsKey(token))
            {
                double right = stack.Pop();
                double left = stack.Pop();
                stack.Push(ApplyOperator(token, left, right));
            }
        }

        return stack.Pop();
    }

    /// <summary>
    /// Applies functions like sin/square root etc
    /// </summary>
    /// <param name="function"></param>
    /// <param name="val"></param>
    /// <returns>a double</returns>
    /// <exception cref="InvalidOperationException"></exception>
    private double ApplyFunction(string function, double val) => function switch
    {
        "sin" => Math.Sin(val),
        "cos" => Math.Cos(val),
        "tan" => Math.Tan(val),
        "asin" => Math.Asin(val),
        "acos" => Math.Acos(val),
        "atan" => Math.Atan(val),
        "sqrt" => val < 0 ? throw new InvalidOperationException("Negative square root") : Math.Sqrt(val),
        _ => throw new InvalidOperationException($"Unknown function {function}")
    };

    /// <summary>
    /// Applies operators like + - / *
    /// </summary>
    /// <param name="op"></param>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns> a double </returns>
    /// <exception cref="DivideByZeroException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    private double ApplyOperator(string op, double left, double right) => op switch
    {
        "+" => left + right,
        "-" => left - right,
        "*" => left * right,
        "/" => right == 0 ? throw new DivideByZeroException() : left / right,
        "%" => right == 0 ? throw new DivideByZeroException() : left % right,
        "^" => Math.Pow(left, right),
        _ => throw new InvalidOperationException($"Unknown operator {op}")
    };
}