namespace ExpressionEngine.Domain;

public enum TokenType
{
    LeftBracket,
    RightBracket,
    LeftParen,
    RightParen,
    Comma,
    Function,
    Identifier,
    At,
    LeftBrace,
    RightBrace,
    Colon,
    String,
    Number,
    Boolean,
    Eof
}

public sealed record Token(TokenType Type, string? Lexeme, int Position);


