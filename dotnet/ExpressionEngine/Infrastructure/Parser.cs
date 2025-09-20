using ExpressionEngine.Application;
using ExpressionEngine.Domain;

namespace ExpressionEngine.Infrastructure;

public sealed class Parser : IParser
{
    public AstNode Parse(IReadOnlyList<Token> tokens)
    {
        var state = new State(tokens);
        var expr = ParseExpression(state);
        if (state.Peek().Type != TokenType.Eof) throw new Exception("Unexpected tokens after expression");
        return expr;
    }

    private static AstNode ParseExpression(State s)
    {
        s.Expect(TokenType.LeftBracket, "Expected [ at the beginning of expression");
        if (s.Peek().Type == TokenType.At)
        {
            s.Advance();
            var ident = s.Expect(TokenType.Identifier, "Expected identifier after @");
            s.Expect(TokenType.RightBracket, "Expected ] at the end of variable reference");
            return new VariableNode(ident.Lexeme!);
        }

        var func = s.Expect(TokenType.Function, "Expected function name or @variable after [");
        s.Expect(TokenType.LeftParen, "Expected ( after function name");
        var args = ParseArguments(s);
        s.Expect(TokenType.RightParen, "Expected ) after function arguments");
        s.Expect(TokenType.RightBracket, "Expected ] at the end of expression");
        return new FunctionCallNode(func.Lexeme!, args);
    }

    private static List<AstNode> ParseArguments(State s)
    {
        var args = new List<AstNode>();
        if (s.Peek().Type == TokenType.RightParen) return args;
        while (true)
        {
            args.Add(ParseArgument(s));
            if (s.Match(TokenType.Comma)) continue;
            if (s.Peek().Type == TokenType.RightParen) break;
            throw new Exception("Expected , or ) after argument");
        }
        return args;
    }

    private static AstNode ParseArgument(State s)
    {
        var t = s.Peek();
        return t.Type switch
        {
            TokenType.String => new LiteralNode(s.Advance().Lexeme),
            TokenType.Number => new LiteralNode(ParseNumber(s.Advance().Lexeme!)),
            TokenType.Boolean => new LiteralNode(ParseBoolean(s.Advance().Lexeme!)),
            TokenType.LeftBracket =>
                // lookahead: function/variable vs array literal
                s.PeekNext().Type is TokenType.Function or TokenType.At ? ParseExpression(s) : ParseArrayLiteral(s),
            TokenType.LeftBrace => ParseObjectLiteral(s),
            _ => throw new Exception($"Unexpected token {t.Type}")
        };
    }

    private static AstNode ParseObjectLiteral(State s)
    {
        s.Expect(TokenType.LeftBrace, "Expected {");
        var dict = new Dictionary<string, AstNodeOrLiteral>(StringComparer.Ordinal);
        if (s.Peek().Type == TokenType.RightBrace)
        {
            s.Advance();
            return new ObjectLiteralNode(dict);
        }
        while (true)
        {
            var keyTok = s.Peek();
            if (keyTok.Type != TokenType.String && keyTok.Type != TokenType.Identifier) throw new Exception("Expected property name");
            s.Advance();
            s.Expect(TokenType.Colon, "Expected : after property name");
            dict[keyTok.Lexeme!] = ParseValueForObject(s);
            if (s.Match(TokenType.Comma)) continue;
            if (s.Peek().Type == TokenType.RightBrace) { s.Advance(); break; }
            throw new Exception("Expected , or } after property");
        }
        return new ObjectLiteralNode(dict);
    }

    private static AstNodeOrLiteral ParseValueForObject(State s)
    {
        var t = s.Peek();
        return t.Type switch
        {
            TokenType.String => new AstNodeOrLiteral(s.Advance().Lexeme),
            TokenType.Number => new AstNodeOrLiteral(ParseNumber(s.Advance().Lexeme!)),
            TokenType.Boolean => new AstNodeOrLiteral(ParseBoolean(s.Advance().Lexeme!)),
            TokenType.LeftBrace => new AstNodeOrLiteral(ParseObjectLiteral(s)),
            TokenType.LeftBracket => new AstNodeOrLiteral(s.PeekNext().Type is TokenType.Function or TokenType.At ? ParseExpression(s) : ParseArrayLiteral(s)),
            _ => throw new Exception("Unsupported value in object literal")
        };
    }

    private static AstNode ParseArrayLiteral(State s)
    {
        s.Expect(TokenType.LeftBracket, "Expected [");
        var elements = new List<AstNodeOrLiteral>();
        if (s.Peek().Type == TokenType.RightBracket) { s.Advance(); return new ArrayLiteralNode(elements); }
        while (true)
        {
            var t = s.Peek();
            switch (t.Type)
            {
                case TokenType.String: elements.Add(new AstNodeOrLiteral(s.Advance().Lexeme)); break;
                case TokenType.Number: elements.Add(new AstNodeOrLiteral(ParseNumber(s.Advance().Lexeme!))); break;
                case TokenType.Boolean: elements.Add(new AstNodeOrLiteral(ParseBoolean(s.Advance().Lexeme!))); break;
                case TokenType.LeftBrace: elements.Add(new AstNodeOrLiteral(ParseObjectLiteral(s))); break;
                case TokenType.LeftBracket:
                    elements.Add(new AstNodeOrLiteral(s.PeekNext().Type is TokenType.Function or TokenType.At ? ParseExpression(s) : ParseArrayLiteral(s)));
                    break;
                default:
                    throw new Exception("Unsupported array element");
            }
            if (s.Match(TokenType.Comma)) continue;
            if (s.Peek().Type == TokenType.RightBracket) { s.Advance(); break; }
            throw new Exception("Expected , or ] after array element");
        }
        return new ArrayLiteralNode(elements);
    }

    private static object ParseNumber(string text)
        => text.Contains('.') ? double.Parse(text, System.Globalization.CultureInfo.InvariantCulture) : int.Parse(text, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);

    private static bool ParseBoolean(string text) => text == "true";

    private sealed class State
    {
        private readonly IReadOnlyList<Token> _tokens;
        private int _pos;
        public State(IReadOnlyList<Token> tokens) { _tokens = tokens; _pos = 0; }
        public Token Peek() => _tokens[_pos];
        public Token Advance() => _tokens[_pos++];
        public bool Match(TokenType type) { if (Peek().Type == type) { Advance(); return true; } return false; }
        public Token PeekNext() => _tokens[_pos + 1];
        public Token Expect(TokenType type, string message)
        {
            if (Peek().Type != type) throw new Exception(message);
            return Advance();
        }
    }
}


