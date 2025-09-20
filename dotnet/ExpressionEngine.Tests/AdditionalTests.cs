namespace ExpressionEngine.Tests;

public class AdditionalTests
{
    [Theory]
    [InlineData("[ADD(1,[ADD(2,3)])]", 6d)]
    [InlineData("[IF([GREATER(5,3)],\"T\",\"F\")]", "T")]
    [InlineData("[CONCAT(\"Hello\",\" \",[TOUPPER(\"world\")])]", "Hello WORLD")]
    public void Mixed_Nesting_Tests(string expr, object expected)
    {
        var engine = TestHelper.CreateEngine();
        var result = engine.Execute(expr, new Dictionary<string, string>());
        if (expected is double d) Convert.ToDouble(result).Should().Be(d);
        else result.Should().Be(expected);
    }

    [Fact]
    public void Complex_Object_In_Array_And_Access()
    {
        var engine = TestHelper.CreateEngine();
        var expr = "[GETJSONPROPERTY([FIRST([{\"Name\":\"A\"},{\"Name\":\"B\"}])],\"Name\")]";
        var result = engine.Execute(expr, new Dictionary<string, string>());
        result.Should().Be("A");
    }
}


