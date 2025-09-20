using ExpressionEngine.Application;
using ExpressionEngine.Domain;

namespace ExpressionEngine.Infrastructure;

public sealed class Evaluator : IEvaluator
{
    private readonly IFunctionRegistry _functions;
    public Evaluator(IFunctionRegistry functions) { _functions = functions; }

    public object? Evaluate(AstNode node, IReadOnlyDictionary<string, string> variables)
    {
        return node switch
        {
            LiteralNode l => l.Value,
            FunctionCallNode f => EvaluateFunction(f, variables),
            VariableNode v => EvaluateVariable(v, variables),
            VariablePropertyNode vp => EvaluateVariableProperty(vp, variables),
            ObjectLiteralNode o => EvaluateObject(o, variables),
            ArrayLiteralNode a => EvaluateArray(a, variables),
            _ => throw new InvalidOperationException("Unknown AST node")
        };
    }

    private object? EvaluateFunction(FunctionCallNode call, IReadOnlyDictionary<string, string> variables)
    {
        var args = call.Arguments.Select(a => Evaluate(a, variables)).ToArray();
        var fn = _functions.Resolve(call.Name);
        return fn(args, variables);
    }

    private static object? EvaluateVariable(VariableNode v, IReadOnlyDictionary<string, string> variables)
    {
        if (!TryResolveVariable(v.Name, variables, out var value)) throw new Exception($"Variable not defined: {v.Name}");
        return value;
    }

    private static object? EvaluateVariableProperty(VariablePropertyNode v, IReadOnlyDictionary<string, string> variables)
    {
        if (!TryResolveVariable(v.VarName, variables, out var raw)) throw new Exception($"Variable not defined: {v.VarName}");
        // If value is JSON string, parse and get property
        try
        {
            using var doc = System.Text.Json.JsonDocument.Parse(raw);
            if (doc.RootElement.ValueKind == System.Text.Json.JsonValueKind.Object && doc.RootElement.TryGetProperty(v.Property, out var prop))
            {
                return prop.ValueKind switch
                {
                    System.Text.Json.JsonValueKind.String => prop.GetString(),
                    System.Text.Json.JsonValueKind.Number => prop.TryGetInt64(out var l) ? l : prop.GetDouble(),
                    System.Text.Json.JsonValueKind.True => true,
                    System.Text.Json.JsonValueKind.False => false,
                    _ => prop.GetRawText()
                };
            }
        }
        catch { /* not JSON */ }
        return null;
    }

    private static bool TryResolveVariable(string name, IReadOnlyDictionary<string, string> variables, out string value)
    {
        // Try exact
        if (variables.TryGetValue(name, out value)) return true;
        // Try with @ prefix
        if (!name.StartsWith('@') && variables.TryGetValue("@" + name, out value)) return true;
        // Try without @ prefix
        if (name.StartsWith('@') && variables.TryGetValue(name.TrimStart('@'), out value)) return true;
        return false;
    }

    private object EvaluateObject(ObjectLiteralNode node, IReadOnlyDictionary<string, string> variables)
    {
        var result = new Dictionary<string, object?>(StringComparer.Ordinal);
        foreach (var kv in node.Properties)
        {
            result[kv.Key] = kv.Value.Node is null ? kv.Value.Literal : Evaluate(kv.Value.Node, variables);
        }
        return result;
    }

    private IList<object?> EvaluateArray(ArrayLiteralNode node, IReadOnlyDictionary<string, string> variables)
    {
        var list = new List<object?>();
        foreach (var el in node.Elements)
        {
            list.Add(el.Node is null ? el.Literal : Evaluate(el.Node, variables));
        }
        return list;
    }
}


