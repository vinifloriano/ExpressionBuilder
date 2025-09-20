using ExpressionEngine.Domain;

namespace ExpressionEngine.Application;

public interface IEvaluator
{
    object? Evaluate(AstNode node, IReadOnlyDictionary<string, string> variables);
}


