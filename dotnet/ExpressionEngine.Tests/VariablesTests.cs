namespace ExpressionEngine.Tests;

public class VariablesTests
{
    [Fact]
    public void Variable_Substitution_String()
    {
        var engine = TestHelper.CreateEngine();
        var vars = new Dictionary<string, string> { ["greeting"] = "Hello" };
        var result = engine.Execute("[CONCAT([@greeting], \" \", \"World\")]", vars);
        result.Should().Be("Hello World");
    }

    [Fact]
    public void Variable_Substitution_Number()
    {
        var engine = TestHelper.CreateEngine();
        var vars = new Dictionary<string, string> { ["a"] = "2", ["b"] = "3" };
        var result = engine.Execute("[ADD([@a],[@b])]", vars);
        Convert.ToDouble(result).Should().Be(5d);
    }

    [Fact]
    public void Variable_Missing_Throws()
    {
        var engine = TestHelper.CreateEngine();
        var act = () => engine.Execute("[ADD([@x],1)]", new Dictionary<string, string>());
        act.Should().Throw<Exception>().WithMessage("*Variable not defined*");
    }
}


