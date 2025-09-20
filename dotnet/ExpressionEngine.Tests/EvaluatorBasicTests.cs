namespace ExpressionEngine.Tests;

public class EvaluatorBasicTests
{
    [Theory]
    [InlineData("[ADD(1,2)]", 3d)]
    [InlineData("[ADD(2,3,4)]", 9d)]
    [InlineData("[SUB(5,2)]", 3d)]
    [InlineData("[MUL(2,3,4)]", 24d)]
    [InlineData("[DIV(10,2)]", 5d)]
    public void Math_Works(string expr, double expected)
    {
        var engine = TestHelper.CreateEngine();
        var result = engine.Execute(expr, new Dictionary<string, string>());
        Convert.ToDouble(result).Should().Be(expected);
    }

    [Theory]
    [InlineData("[CONCAT(\"A\",\"B\")]", "AB")]
    [InlineData("[TOUPPER(\"abc\")]", "ABC")]
    [InlineData("[TOLOWER(\"ABC\")]", "abc")]
    [InlineData("[TRIM(\"  x  \" )]", "x")]
    [InlineData("[SUBSTRING(\"Hello\",1,3)]", "ell")]
    public void Strings_Works(string expr, string expected)
    {
        var engine = TestHelper.CreateEngine();
        var result = engine.Execute(expr, new Dictionary<string, string>());
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("[EQUALS(1,1)]", true)]
    [InlineData("[NOTEQUALS(1,2)]", true)]
    [InlineData("[GREATER(2,1)]", true)]
    [InlineData("[LESS(1,2)]", true)]
    [InlineData("[GREATEROREQUALS(2,2)]", true)]
    [InlineData("[LESSOREQUALS(2,2)]", true)]
    public void Comparison_Works(string expr, bool expected)
    {
        var engine = TestHelper.CreateEngine();
        var result = engine.Execute(expr, new Dictionary<string, string>());
        Convert.ToBoolean(result).Should().Be(expected);
    }

    [Theory]
    [InlineData("[IF(true,\"Y\",\"N\")]", "Y")]
    [InlineData("[IF(false,\"Y\",\"N\")]", "N")]
    [InlineData("[AND(true,true,true)]", true)]
    [InlineData("[OR(false,false,true)]", true)]
    [InlineData("[NOT(false)]", true)]
    public void Logic_Works(string expr, object expected)
    {
        var engine = TestHelper.CreateEngine();
        var result = engine.Execute(expr, new Dictionary<string, string>());
        if (expected is bool b) Convert.ToBoolean(result).Should().Be(b);
        else result.Should().Be(expected);
    }

    [Fact]
    public void Nested_Expressions_Work()
    {
        var engine = TestHelper.CreateEngine();
        var result = engine.Execute("[MUL(5,[ADD(2,3)])]", new Dictionary<string, string>());
        Convert.ToDouble(result).Should().Be(25d);
    }

    [Fact]
    public void Whitespace_Is_Ignored()
    {
        var engine = TestHelper.CreateEngine();
        var expr = "  [   IF   ([EQUALS(1,2)], \"True\", \"False\")]  ";
        var result = engine.Execute(expr, new Dictionary<string, string>());
        result.Should().Be("False");
    }
}


