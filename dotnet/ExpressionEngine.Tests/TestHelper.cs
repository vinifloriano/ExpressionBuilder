namespace ExpressionEngine.Tests;

internal static class TestHelper
{
    public static IExpressionService CreateEngine()
    {
        var registry = new DefaultFunctionRegistry();
        var lexer = new Lexer();
        var parser = new Parser();
        var evaluator = new Evaluator(registry);
        return new ExpressionService(lexer, parser, evaluator);
    }
}


