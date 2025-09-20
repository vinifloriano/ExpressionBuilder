namespace ExpressionEngine.Application;

public interface IExpressionService
{
    object? Execute(string expression, IReadOnlyDictionary<string, string> variables);
}


