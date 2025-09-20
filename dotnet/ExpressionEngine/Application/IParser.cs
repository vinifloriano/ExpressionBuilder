using ExpressionEngine.Domain;

namespace ExpressionEngine.Application;

public interface IParser
{
    AstNode Parse(IReadOnlyList<Token> tokens);
}


