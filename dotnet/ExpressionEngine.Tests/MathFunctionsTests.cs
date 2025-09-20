namespace ExpressionEngine.Tests;

public class MathFunctionsTests
{
    [Theory]
    [InlineData("[ADD(1, 2, 3)]", 6d)]
    [InlineData("[ADD(-1, -2, -3)]", -6d)]
    [InlineData("[ADD(1.5, 2.5, 3.5)]", 7.5d)]
    [InlineData("[ADD(1, -2, 3)]", 2d)]
    [InlineData("[ADD(10)]", 10d)]
    [InlineData("[ADD(0, 0, 0)]", 0d)]
    public void Add_Works(string expr, double expected)
    {
        var engine = TestHelper.CreateEngine();
        Convert.ToDouble(engine.Execute(expr, new Dictionary<string, string>())).Should().Be(expected);
    }

    [Theory]
    [InlineData("[SUB(5, 3)]", 2d)]
    [InlineData("[SUB(3, 5)]", -2d)]
    [InlineData("[SUB(-5, -3)]", -2d)]
    [InlineData("[SUB(5.5, 3.2)]", 2.3d)]
    [InlineData("[SUB(0, 0)]", 0d)]
    public void Sub_Works(string expr, double expected)
    {
        var engine = TestHelper.CreateEngine();
        Convert.ToDouble(engine.Execute(expr, new Dictionary<string, string>())).Should().BeApproximately(expected, 1e-9);
    }

    [Theory]
    [InlineData("[MUL(2, 3, 4)]", 24d)]
    [InlineData("[MUL(-2, 3, 4)]", -24d)]
    [InlineData("[MUL(-2, -3, -4)]", -24d)]
    [InlineData("[MUL(1.5, 2.5, 3.5)]", 13.125d)]
    [InlineData("[MUL(10, 0, 5)]", 0d)]
    public void Mul_Works(string expr, double expected)
    {
        var engine = TestHelper.CreateEngine();
        Convert.ToDouble(engine.Execute(expr, new Dictionary<string, string>())).Should().Be(expected);
    }

    [Theory]
    [InlineData("[DIV(10, 2)]", 5d)]
    [InlineData("[DIV(10, 4)]", 2.5d)]
    [InlineData("[DIV(-10, 2)]", -5d)]
    [InlineData("[DIV(10, -2)]", -5d)]
    [InlineData("[DIV(0, 5)]", 0d)]
    public void Div_Works(string expr, double expected)
    {
        var engine = TestHelper.CreateEngine();
        Convert.ToDouble(engine.Execute(expr, new Dictionary<string, string>())).Should().Be(expected);
    }

    [Theory]
    [InlineData("[MOD(10,3)]", 1d)]
    [InlineData("[MOD(-1,3)]", 2d)]
    [InlineData("[MOD(10, -3)]", -2d)]
    [InlineData("[MOD(-10, -3)]", -1d)]
    [InlineData("[MOD(5.5, 2.1)]", 1.3d)]
    public void Mod_Works(string expr, double expected)
    {
        var engine = TestHelper.CreateEngine();
        Convert.ToDouble(engine.Execute(expr, new Dictionary<string, string>())).Should().BeApproximately(expected, 1e-9);
    }

    [Theory]
    [InlineData("[POW(2,8)]", 256d)]
    [InlineData("[POW(9,0.5)]", 3d)]
    [InlineData("[POW(4, -2)]", 0.0625d)]
    [InlineData("[POW(-2, 3)]", -8d)]
    [InlineData("[POW(10, 0)]", 1d)]
    public void Pow_Works(string expr, double expected)
    {
        var engine = TestHelper.CreateEngine();
        Convert.ToDouble(engine.Execute(expr, new Dictionary<string, string>())).Should().BeApproximately(expected, 1e-9);
    }

    [Theory]
    [InlineData("[ROUND(3.14159,2)]", 3.14)]
    [InlineData("[ROUND(2.5,0)]", 3d)]
    [InlineData("[ROUND(2.49,0)]", 2d)]
    [InlineData("[ROUND(3.14159)]", 3d)]
    [InlineData("[ROUND(-3.14159, 2)]", -3.14d)]
    public void Round_Works(string expr, double expected)
    {
        var engine = TestHelper.CreateEngine();
        Convert.ToDouble(engine.Execute(expr, new Dictionary<string, string>())).Should().Be(expected);
    }

    [Theory]
    [InlineData("[MAX(1,5,3)]", 5d)]
    [InlineData("[MAX(-1,-5,-3)]", -1d)]
    [InlineData("[MAX(1.5, 5.2, 3.1)]", 5.2d)]
    [InlineData("[MAX(10)]", 10d)]
    [InlineData("[MAX(0, 0, 0)]", 0d)]
    public void Max_Works(string expr, double expected)
    {
        var engine = TestHelper.CreateEngine();
        Convert.ToDouble(engine.Execute(expr, new Dictionary<string, string>())).Should().Be(expected);
    }

    [Theory]
    [InlineData("[MIN(1,5,3)]", 1d)]
    [InlineData("[MIN(-1,-5,-3)]", -5d)]
    [InlineData("[MIN(1.5, 5.2, 3.1)]", 1.5d)]
    [InlineData("[MIN(10)]", 10d)]
    [InlineData("[MIN(0, 0, 0)]", 0d)]
    public void Min_Works(string expr, double expected)
    {
        var engine = TestHelper.CreateEngine();
        Convert.ToDouble(engine.Execute(expr, new Dictionary<string, string>())).Should().Be(expected);
    }

    [Theory]
    [InlineData("[BETWEEN(5, 1, 10)]", true)]
    [InlineData("[BETWEEN(1, 1, 10)]", false)]
    [InlineData("[BETWEEN(10, 1, 10)]", false)]
    [InlineData("[BETWEEN(0, 1, 10)]", false)]
    [InlineData("[BETWEEN(11, 1, 10)]", false)]
    [InlineData("[BETWEEN(5, 1, 10, true)]", true)]
    [InlineData("[BETWEEN(1, 1, 10, true)]", true)]
    [InlineData("[BETWEEN(10, 1, 10, true)]", true)]
    public void Between_Works(string expr, bool expected)
    {
        var engine = TestHelper.CreateEngine();
        ((bool)engine.Execute(expr, new Dictionary<string, string>())).Should().Be(expected);
    }
}
