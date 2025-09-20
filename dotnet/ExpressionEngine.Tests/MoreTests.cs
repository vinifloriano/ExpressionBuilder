namespace ExpressionEngine.Tests;

public class MoreTests
{
    public static IEnumerable<object[]> MathCases() => new List<object[]>
    {
        new object[]{ "[ADD( 1 , 2 , 3 , 4 )]", 10d },
        new object[]{ "[MUL( 1 , 2 , 3 , 4 )]", 24d },
        new object[]{ "[SUB( 10 , [ADD(3,2)] )]", 5d },
        new object[]{ "[DIV( [MUL(3,3)] , 3 )]", 3d },
        new object[]{ "[ADD([@a],5)]", 7d }
    };

    [Theory]
    [MemberData(nameof(MathCases))]
    public void Extended_Math_Tests(string expr, double expected)
    {
        var engine = TestHelper.CreateEngine();
        var vars = new Dictionary<string, string> { ["a"] = "2" };
        var result = engine.Execute(expr, vars);
        Convert.ToDouble(result).Should().Be(expected);
    }

    [Theory]
    [InlineData("[CONTAINS(\"abc\",\"b\")]", true)]
    [InlineData("[STARTSWITH(\"abc\",\"a\")]", true)]
    [InlineData("[ENDSWITH(\"abc\",\"c\")]", true)]
    [InlineData("[LENGTH(\"abcd\")]", 4)]
    public void More_String_Tests(string expr, object expected)
    {
        var engine = TestHelper.CreateEngine();
        var result = engine.Execute(expr, new Dictionary<string, string>());
        if (expected is int i) Convert.ToInt32(result).Should().Be(i);
        else Convert.ToBoolean(result).Should().BeTrue();
    }

    [Theory]
    [InlineData("[GETJSONPROPERTY({Name: \"X\"}, \"Name\")]", "X")]
    [InlineData("[FIRST([[@a],\"x\"]) ]", "2")] // variable a=2
    public void Json_And_Arrays_Robust(string expr, string expected)
    {
        var engine = TestHelper.CreateEngine();
        var vars = new Dictionary<string, string> { ["a"] = "2" };
        var result = engine.Execute(expr, vars);
        (result?.ToString() ?? "").Should().Be(expected);
    }
}


