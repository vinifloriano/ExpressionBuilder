namespace ExpressionEngine.Domain;

public abstract record AstNode;

public sealed record LiteralNode(object? Value) : AstNode;

public sealed record FunctionCallNode(string Name, IReadOnlyList<AstNode> Arguments) : AstNode;

public sealed record VariableNode(string Name) : AstNode;
public sealed record VariablePropertyNode(string VarName, string Property) : AstNode;

public sealed record ObjectLiteralNode(IReadOnlyDictionary<string, AstNodeOrLiteral> Properties) : AstNode;
public sealed record ArrayLiteralNode(IReadOnlyList<AstNodeOrLiteral> Elements) : AstNode;

public readonly struct AstNodeOrLiteral
{
    public AstNodeOrLiteral(object? literal)
    {
        Literal = literal;
        Node = null;
    }
    public AstNodeOrLiteral(AstNode node)
    {
        Node = node;
        Literal = null;
    }
    public object? Literal { get; }
    public AstNode? Node { get; }
}


