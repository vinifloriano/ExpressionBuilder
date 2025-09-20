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

public class LogicalFunctionsTests
{
    [Theory]
    [InlineData("[EQUALS(1, 1)]", true)]
    [InlineData("[EQUALS(1, 2)]", false)]
    [InlineData("[EQUALS(\"a\", \"a\")]", true)]
    [InlineData("[EQUALS(\"a\", \"b\")]", false)]
    [InlineData("[EQUALS(true, true)]", true)]
    [InlineData("[EQUALS(null, null)]", true)]
    public void Equals_Works(string expr, bool expected)
    {
        var engine = TestHelper.CreateEngine();
        ((bool)engine.Execute(expr, new Dictionary<string, string>())).Should().Be(expected);
    }

    [Theory]
    [InlineData("[NOTEQUALS(1, 2)]", true)]
    [InlineData("[NOTEQUALS(1, 1)]", false)]
    public void NotEquals_Works(string expr, bool expected)
    {
        var engine = TestHelper.CreateEngine();
        ((bool)engine.Execute(expr, new Dictionary<string, string>())).Should().Be(expected);
    }

    [Theory]
    [InlineData("[GREATER(2, 1)]", true)]
    [InlineData("[GREATER(1, 2)]", false)]
    [InlineData("[GREATER(1, 1)]", false)]
    public void Greater_Works(string expr, bool expected)
    {
        var engine = TestHelper.CreateEngine();
        ((bool)engine.Execute(expr, new Dictionary<string, string>())).Should().Be(expected);
    }

    [Theory]
    [InlineData("[LESS(1, 2)]", true)]
    [InlineData("[LESS(2, 1)]", false)]
    [InlineData("[LESS(1, 1)]", false)]
    public void Less_Works(string expr, bool expected)
    {
        var engine = TestHelper.CreateEngine();
        ((bool)engine.Execute(expr, new Dictionary<string, string>())).Should().Be(expected);
    }

    [Theory]
    [InlineData("[GREATEROREQUALS(2, 1)]", true)]
    [InlineData("[GREATEROREQUALS(1, 1)]", true)]
    [InlineData("[GREATEROREQUALS(1, 2)]", false)]
    public void GreaterOrEquals_Works(string expr, bool expected)
    {
        var engine = TestHelper.CreateEngine();
        ((bool)engine.Execute(expr, new Dictionary<string, string>())).Should().Be(expected);
    }

    [Theory]
    [InlineData("[LESSOREQUALS(1, 2)]", true)]
    [InlineData("[LESSOREQUALS(1, 1)]", true)]
    [InlineData("[LESSOREQUALS(2, 1)]", false)]
    public void LessOrEquals_Works(string expr, bool expected)
    {
        var engine = TestHelper.CreateEngine();
        ((bool)engine.Execute(expr, new Dictionary<string, string>())).Should().Be(expected);
    }

    [Theory]
    [InlineData("[IF(true, \"a\", \"b\")]", "a")]
    [InlineData("[IF(false, \"a\", \"b\")]", "b")]
    [InlineData("[IF([GREATER(2,1)], 1, 0)]", 1d)]
    public void If_Works(string expr, object expected)
    {
        var engine = TestHelper.CreateEngine();
        engine.Execute(expr, new Dictionary<string, string>()).Should().Be(expected);
    }

    [Theory]
    [InlineData("[AND(true, true, true)]", true)]
    [InlineData("[AND(true, false, true)]", false)]
    public void And_Works(string expr, bool expected)
    {
        var engine = TestHelper.CreateEngine();
        ((bool)engine.Execute(expr, new Dictionary<string, string>())).Should().Be(expected);
    }

    [Theory]
    [InlineData("[OR(false, true, false)]", true)]
    [InlineData("[OR(false, false, false)]", false)]
    public void Or_Works(string expr, bool expected)
    {
        var engine = TestHelper.CreateEngine();
        ((bool)engine.Execute(expr, new Dictionary<string, string>())).Should().Be(expected);
    }

    [Theory]
    [InlineData("[NOT(true)]", false)]
    [InlineData("[NOT(false)]", true)]
    public void Not_Works(string expr, bool expected)
    {
        var engine = TestHelper.CreateEngine();
        ((bool)engine.Execute(expr, new Dictionary<string, string>())).Should().Be(expected);
    }

    [Theory]
    [InlineData("[EMPTY(\"\")]", true)]
    [InlineData("[EMPTY(\"a\")]", false)]
    [InlineData("[EMPTY([])]", true)]
    [InlineData("[EMPTY([1])]", false)]
    [InlineData("[EMPTY(null)]", true)]
    public void Empty_Works(string expr, bool expected)
    {
        var engine = TestHelper.CreateEngine();
        ((bool)engine.Execute(expr, new Dictionary<string, string>())).Should().Be(expected);
    }
}
