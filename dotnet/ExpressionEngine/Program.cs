using System;
using ExpressionEngine.Application;
using ExpressionEngine.Domain;
using ExpressionEngine.Infrastructure;

namespace ExpressionEngine;

internal static class Program
{
    private static void Main(string[] args)
    {
        // Wire up services (simple manual DI to keep it lightweight)
        var functionRegistry = new DefaultFunctionRegistry();
        var lexer = new Lexer();
        var parser = new Parser();
        var evaluator = new Evaluator(functionRegistry);
        var engine = new ExpressionService(lexer, parser, evaluator);

        var variables = new Dictionary<string, string>
        {
            ["a"] = "1",
            ["b"] = "2",
            ["greeting"] = "Hello"
        };

        var samples = new[]
        {
            "[ADD(1,2)]",
            "   [CONCAT(\"Hello\", \" \", \"World\")]",
            "  [   IF   ([EQUALS(1,2)], \"True\", \"False\")]",
            "[ADD(  [@a]  ,   [@b])]",
            "[CONCAT([@greeting], \"\\\" \", \"World\")]",
            "   [GETJSONPROPERTY({\"Name\":\"Test\"}, \"Name\")]",
            "[FIRST([1,2,3])]"
        };

        foreach (var expr in samples)
        {
            try
            {
                var result = engine.Execute(expr, variables);
                Console.WriteLine($"{expr} => {FormatValue(result)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{expr} => Error: {ex.Message}");
            }
        }
    }

    private static string FormatValue(object? value)
    {
        return value switch
        {
            null => "null",
            string s => $"\"{s}\"",
            bool b => b ? "true" : "false",
            _ => value.ToString() ?? ""
        };
    }
}


