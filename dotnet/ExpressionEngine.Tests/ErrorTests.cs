namespace ExpressionEngine.Tests;

public class ErrorTests
{
    [Theory]
    [InlineData("[")]
    [InlineData("]")]
    [InlineData("[ADD(]")]
    [InlineData("[UNKNOWN(1)]")]
    public void ParserOrRuntime_Throws(string expr)
    {
        var engine = TestHelper.CreateEngine();
        Action act = () => engine.Execute(expr, new Dictionary<string, string>());
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Divide_By_Zero_Throws()
    {
        var engine = TestHelper.CreateEngine();
        Action act = () => engine.Execute("[DIV(1,0)]", new Dictionary<string, string>());
        act.Should().Throw<Exception>().WithMessage("*zero*");
    }
}


