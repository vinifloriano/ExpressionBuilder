using System.Globalization;
using ExpressionEngine.Application;
using ExpressionEngine.Domain;

namespace ExpressionEngine.Infrastructure;

public sealed class Lexer : ILexer
{
    public IReadOnlyList<Token> Tokenize(string input)
    {
        var tokens = new List<Token>();
        var position = 0;
        while (position < input.Length)
        {
            var c = input[position];
            if (char.IsWhiteSpace(c)) { position++; continue; }

            switch (c)
            {
                case '[': tokens.Add(new Token(TokenType.LeftBracket, "[", position++)); continue;
                case ']': tokens.Add(new Token(TokenType.RightBracket, "]", position++)); continue;
                case '(': tokens.Add(new Token(TokenType.LeftParen, "(", position++)); continue;
                case ')': tokens.Add(new Token(TokenType.RightParen, ")", position++)); continue;
                case ',': tokens.Add(new Token(TokenType.Comma, ",", position++)); continue;
                case '@': tokens.Add(new Token(TokenType.At, "@", position++)); continue;
                case '{': tokens.Add(new Token(TokenType.LeftBrace, "{", position++)); continue;
                case '}': tokens.Add(new Token(TokenType.RightBrace, "}", position++)); continue;
                case ':': tokens.Add(new Token(TokenType.Colon, ":", position++)); continue;
                case '.': tokens.Add(new Token(TokenType.Dot, ".", position++)); continue;
                case '<':
                {
                    // Capture a simple XML literal until matching closing root tag
                    var start = position;
                    // find first tag name
                    int i = position + 1;
                    while (i < input.Length && char.IsWhiteSpace(input[i])) i++;
                    int nameStart = i;
                    while (i < input.Length && (char.IsLetterOrDigit(input[i]) || input[i] == '_' || input[i] == '-')) i++;
                    var rootName = input[nameStart..i];
                    int depth = 0;
                    while (position < input.Length)
                    {
                        if (input[position] == '<')
                        {
                            if (position + 1 < input.Length && input[position + 1] == '/') depth--;
                            else if (position + 1 < input.Length && char.IsLetter(input[position + 1])) depth++;
                            // advance to next '>'
                            position++;
                            while (position < input.Length && input[position] != '>') position++;
                            if (position < input.Length && input[position] == '>')
                            {
                                // self closing
                                if (position - 1 >= 0 && input[position - 1] == '/') depth--;
                                position++;
                                if (depth == 0) break;
                            }
                        }
                        else position++;
                    }
                    var xml = input[start..position];
                    tokens.Add(new Token(TokenType.String, xml, start));
                    continue;
                }
                case '"':
                case '\'':
                {
                    var quote = c;
                    var start = position + 1;
                    position++;
                    var value = new System.Text.StringBuilder();
                    while (position < input.Length && input[position] != quote)
                    {
                        if (input[position] == '\\' && position + 1 < input.Length)
                        {
                            position++;
                            var next = input[position];
                            value.Append(next switch
                            {
                                'n' => '\n',
                                't' => '\t',
                                'r' => '\r',
                                '\\' => '\\',
                                '"' => '"',
                                '\'' => '\'',
                                _ => next
                            });
                        }
                        else
                        {
                            value.Append(input[position]);
                        }
                        position++;
                    }
                    if (position >= input.Length)
                        throw new Exception($"Unterminated string literal at position {position}");
                    tokens.Add(new Token(TokenType.String, value.ToString(), start));
                    position++; // closing quote
                    continue;
                }
            }

            // Signed or unsigned number literal
            if (char.IsAsciiDigit(c) || ((c == '-' || c == '+') && position + 1 < input.Length && char.IsAsciiDigit(input[position + 1])))
            {
                var start = position;
                if (input[position] == '-' || input[position] == '+') position++;
                bool seenDot = false;
                while (position < input.Length)
                {
                    if (char.IsAsciiDigit(input[position])) { position++; continue; }
                    if (input[position] == '.' && !seenDot && position + 1 < input.Length && char.IsAsciiDigit(input[position + 1]))
                    { seenDot = true; position++; continue; }
                    break;
                }
                var lexeme = input[start..position];
                tokens.Add(new Token(TokenType.Number, lexeme, start));
                continue;
            }

            if (char.IsLower(c))
            {
                if (input.AsSpan(position).StartsWith("true"))
                {
                    tokens.Add(new Token(TokenType.Boolean, "true", position));
                    position += 4; continue;
                }
                if (input.AsSpan(position).StartsWith("false"))
                {
                    tokens.Add(new Token(TokenType.Boolean, "false", position));
                    position += 5; continue;
                }
            }

            if (char.IsLetter(c) || c == '_')
            {
                var start = position;
                while (position < input.Length && (char.IsLetterOrDigit(input[position]) || input[position] == '_')) position++;
                var name = input[start..position];
                // Uppercase -> function; otherwise identifier
                if (name.All(ch => char.IsUpper(ch) || char.IsDigit(ch) || ch == '_'))
                    tokens.Add(new Token(TokenType.Function, name, start));
                else
                    tokens.Add(new Token(TokenType.Identifier, name, start));
                continue;
            }

            throw new Exception($"Unexpected character '{c}' at position {position}");
        }

        tokens.Add(new Token(TokenType.Eof, null, position));
        return tokens;
    }
}


