using System.Globalization;

namespace UselessApp.Services;

/// <summary>Independent, stateless expression parser. Trigonometry uses degrees.</summary>
public sealed class CalculatorEvaluatorService
{
    public string Evaluate(List<string> tokens)
    {
        try
        {
            var parser = new Parser(string.Concat(tokens));
            var result = parser.Parse();
            return double.IsFinite(result) ? result.ToString("G12", CultureInfo.InvariantCulture) : "Error: undefined result";
        }
        catch (Exception ex) when (ex is FormatException or DivideByZeroException or ArgumentOutOfRangeException or OverflowException)
        {
            return "Error: " + ex.Message;
        }
    }

    private sealed class Parser(string text)
    {
        private int pos;
        public double Parse()
        {
            var value = AddSubtract();
            Skip();
            if (pos != text.Length) throw new FormatException("Unexpected input at position " + (pos + 1));
            return value;
        }
        private void Skip() { while (pos < text.Length && char.IsWhiteSpace(text[pos])) pos++; }
        private bool Eat(char ch) { Skip(); if (pos < text.Length && text[pos] == ch) { pos++; return true; } return false; }
        private double AddSubtract()
        {
            var n = MultiplyDivide();
            while (true)
            {
                if (Eat('+')) n += MultiplyDivide();
                else if (Eat('-')) n -= MultiplyDivide();
                else return n;
            }
        }
        private double MultiplyDivide()
        {
            var n = Unary();
            while (true)
            {
                if (Eat('*')) n *= Unary();
                else if (Eat('/')) { var denominator = Unary(); if (denominator == 0) throw new DivideByZeroException("Division by zero"); n /= denominator; }
                else if (Eat('%')) { var denominator = Unary(); if (denominator == 0) throw new DivideByZeroException("Modulo by zero"); n %= denominator; }
                else return n;
            }
        }
        private double Unary()
        {
            if (Eat('+')) return Unary();
            if (Eat('-')) return -Unary();
            return Power();
        }
        private double Power()
        {
            var left = Primary();
            return Eat('^') ? Math.Pow(left, Unary()) : left;
        }
        private double Primary()
        {
            if (Eat('(')) { var n = AddSubtract(); if (!Eat(')')) throw new FormatException("Missing closing parenthesis"); return n; }
            Skip();
            var start = pos;
            while (pos < text.Length && (char.IsDigit(text[pos]) || text[pos] == '.')) pos++;
            if (start != pos)
            {
                if (!double.TryParse(text[start..pos], NumberStyles.Float, CultureInfo.InvariantCulture, out var n)) throw new FormatException("Invalid number");
                return n;
            }
            while (pos < text.Length && char.IsLetter(text[pos])) pos++;
            if (pos == start) throw new FormatException("Expected number or function");
            var name = text[start..pos].ToLowerInvariant();
            if (name == "pi") return Math.PI;
            if (name == "e") return Math.E;
            if (!Eat('(')) throw new FormatException("Expected '(' after " + name);
            var value = AddSubtract();
            if (!Eat(')')) throw new FormatException("Missing closing parenthesis");
            return name switch
            {
                "sqrt" => Math.Sqrt(value),
                "sin" => Math.Sin(value * Math.PI / 180),
                "cos" => Math.Cos(value * Math.PI / 180),
                "tan" => Math.Tan(value * Math.PI / 180),
                "asin" => Math.Asin(value) * 180 / Math.PI,
                "acos" => Math.Acos(value) * 180 / Math.PI,
                "atan" => Math.Atan(value) * 180 / Math.PI,
                _ => throw new FormatException("Unknown function: " + name)
            };
        }
    }
}
