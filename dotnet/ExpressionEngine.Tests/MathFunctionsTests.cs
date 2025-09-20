namespace ExpressionEngine.Tests;

public class MathFunctionsTests
{
    [Theory]
    [InlineData("[MOD(10,3)]", 1d)]
    [InlineData("[MOD(-1,3)]", 2d)]
    public void Mod_Works(string expr, double expected)
    {
        var engine = TestHelper.CreateEngine();
        Convert.ToDouble(engine.Execute(expr, new Dictionary<string, string>())).Should().Be(expected);
    }

    [Theory]
    [InlineData("[POW(2,8)]", 256d)]
    [InlineData("[POW(9,0.5)]", 3d)]
    public void Pow_Works(string expr, double expected)
    {
        var engine = TestHelper.CreateEngine();
        Convert.ToDouble(engine.Execute(expr, new Dictionary<string, string>())).Should().BeApproximately(expected, 1e-9);
    }

    [Theory]
    [InlineData("[ROUND(3.14159,2)]", 3.14)]
    [InlineData("[ROUND(2.5,0)]", 3d)]
    public void Round_Works(string expr, double expected)
    {
        var engine = TestHelper.CreateEngine();
        Convert.ToDouble(engine.Execute(expr, new Dictionary<string, string>())).Should().Be(expected);
    }

    [Fact]
    public void Max_Min_Works()
    {
        var engine = TestHelper.CreateEngine();
        Convert.ToDouble(engine.Execute("[MAX(1,5,3)]", new Dictionary<string, string>())).Should().Be(5d);
        Convert.ToDouble(engine.Execute("[MIN(1,5,3)]", new Dictionary<string, string>())).Should().Be(1d);
    }
}


