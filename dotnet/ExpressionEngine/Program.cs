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
        string json = "{\"expand\":\"schema,names\",\"startAt\":0,\"maxResults\":3,\"total\":57,\"issues\":[{\"expand\":\"\",\"id\":\"10001\",\"self\":\"https://your-domain.atlassian.net/rest/api/2/issue/10001\",\"key\":\"TEST-1\",\"fields\":{\"summary\":\"User login is not working\",\"status\":{\"self\":\"https://your-domain.atlassian.net/rest/api/2/status/1\",\"description\":\"The issue is open and ready for the assignee to start work on it.\",\"iconUrl\":\"https://your-domain.atlassian.net/icons/status_open.png\",\"name\":\"To Do\",\"id\":\"1\",\"statusCategory\":{\"id\":2,\"key\":\"new\",\"colorName\":\"blue-gray\",\"name\":\"To Do\"}},\"priority\":{\"self\":\"https://your-domain.atlassian.net/rest/api/2/priority/2\",\"iconUrl\":\"https://your-domain.atlassian.net/icons/priority_major.png\",\"name\":\"Major\",\"id\":\"2\"},\"issuetype\":{\"self\":\"https://your-domain.atlassian.net/rest/api/2/issuetype/1\",\"id\":\"1\",\"description\":\"A problem which impairs or prevents the functions of the product.\",\"iconUrl\":\"https://your-domain.atlassian.net/icons/bug.png\",\"name\":\"Bug\",\"subtask\":false},\"assignee\":{\"self\":\"https://your-domain.atlassian.net/rest/api/2/user?accountId=12345\",\"accountId\":\"12345\",\"displayName\":\"Jane Doe\",\"active\":true},\"reporter\":{\"self\":\"https://your-domain.atlassian.net/rest/api/2/user?accountId=67890\",\"accountId\":\"67890\",\"displayName\":\"John Smith\",\"active\":true},\"created\":\"2025-09-18T09:30:00.000+0000\",\"updated\":\"2025-09-19T15:00:00.000+0000\"}},{\"id\":\"10002\",\"key\":\"TEST-2\",\"fields\":{\"summary\":\"Add search filter to dashboard\",\"status\":{\"name\":\"In Progress\"},\"priority\":{\"name\":\"Medium\"},\"issuetype\":{\"name\":\"Story\"},\"assignee\":{\"displayName\":\"Vinicius Floriano\"},\"reporter\":{\"displayName\":\"Jane Doe\"},\"created\":\"2025-09-17T12:15:00.000+0000\",\"updated\":\"2025-09-18T14:20:00.000+0000\"}},{\"id\":\"10003\",\"key\":\"TEST-3\",\"fields\":{\"summary\":\"Investigate API performance issues\",\"status\":{\"name\":\"Done\"},\"priority\":{\"name\":\"High\"},\"issuetype\":{\"name\":\"Task\"},\"assignee\":{\"displayName\":\"John Smith\"},\"reporter\":{\"displayName\":\"Vinicius Floriano\"},\"created\":\"2025-09-15T08:45:00.000+0000\",\"updated\":\"2025-09-16T10:10:00.000+0000\"}}]}";

        var variables = new Dictionary<string, string>
        {
            ["@a"] = "1",
            ["@b"] = "2",
            ["@greeting"] = "Hello \"",
            ["@JSON"] = "{\"Name\":\"Test\"}",
            ["4.JsonArray"] = "[{\"Id\":1,\"Name\":\"Test\"},{\"Id\":2,\"Name\":\"Test2\"}]",
            ["1.Users"] = "John,Jane,Jim",
            ["1.UsersArray"] = json,
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
            "[GETJSONPROPERTY([@JSON], \"Name\")]",
            "[GETXMLPROPERTY(<root><xml>v</xml></root>, \"xml\")]",
            "[FIRST([4.JsonArray])]",
            "[CONCAT(\"1\", 23, \"43423\")]",
            "[SPLIT([1.Users], \",\")]",
            "[GETJSONPROPERTY([1.UsersArray], \"issues\")]"
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


