using ExpressionEngine.Domain;

namespace ExpressionEngine.Application;

public sealed class ExpressionService : IExpressionService
{
    private readonly ILexer _lexer;
    private readonly IParser _parser;
    private readonly IEvaluator _evaluator;

    public ExpressionService(ILexer lexer, IParser parser, IEvaluator evaluator)
    {
        _lexer = lexer;
        _parser = parser;
        _evaluator = evaluator;
    }

    public object? Execute(string expression, IReadOnlyDictionary<string, string> variables)
    {
        var tokens = _lexer.Tokenize(expression);
        var ast = _parser.Parse(tokens);
        return _evaluator.Evaluate(ast, variables);
    }
}


