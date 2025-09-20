using ExpressionEngine.Domain;

namespace ExpressionEngine.Application;

public interface ILexer
{
    IReadOnlyList<Token> Tokenize(string input);
}


