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
        if (!variables.TryGetValue(v.Name, out var value)) throw new Exception($"Variable not defined: {v.Name}");
        return value;
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


