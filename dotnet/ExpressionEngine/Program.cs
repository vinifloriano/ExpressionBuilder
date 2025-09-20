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
            ["@a"] = "1",
            ["@b"] = "2",
            ["@greeting"] = "Hello \"",
            ["@json"] = "{\"Name\":\"Test\"}",
            ["4.JsonArray"] = "[{\"Name\":\"Test\"},{\"Name\":\"Test2\"}]",

        };

        var samples = new[]
        {
            "[ADD(1,2)]",
            "   [CONCAT(\"Hello\", \" \", \"World\")]",
            "  [   IF   ([EQUALS(1,2)], \"True\", \"False\")]",
            "[ADD(  [@a]  ,   [@b])]",
            "[CONCAT([@greeting], \"\\\" \", \"World\")]",
            "   [GETJSONPROPERTY({\"Name\":\"Test\"}, \"Name\")]",
            "[FIRST([1,2,3])]",
            "[FIRST([1.2,2,3])]",
            "[GETJSONPROPERTY([@json], \"Name\")]",
            "[GETXMLPROPERTY(<root><xml>v</xml></root>, \"xml\")]",
            "[FIRST([4.JsonArray])]"
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
        if (value is null) return "null";
        if (value is string s) return $"\"{s}\"";
        if (value is bool b) return b ? "true" : "false";
        try
        {
            return System.Text.Json.JsonSerializer.Serialize(value);
        }
        catch
        {
            return value.ToString() ?? string.Empty;
        }
    }
}


