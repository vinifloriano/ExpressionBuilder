namespace ExpressionEngine.Tests;

public class JsonAndArrayTests
{
    [Theory]
    [InlineData("[GETJSONPROPERTY({\"Name\":\"Test\"}, \"Name\")]", "Test")]
    [InlineData("[GETJSONPROPERTY('{\"Name\":\"Test\"}', 'Name')]", "Test")]
    [InlineData("[GETJSONPROPERTY('{\"a\":{\"b\":\"c\"}}', 'a.b')]", null)] // not supported
    public void GetJsonProperty_Works(string expr, object expected)
    {
        var engine = TestHelper.CreateEngine();
        var result = engine.Execute(expr, new Dictionary<string, string>());
        result.Should().Be(expected);
    }

    [Fact]
    public void SetJsonProperty_Works()
    {
        var engine = TestHelper.CreateEngine();
        var result = engine.Execute("[SETJSONPROPERTY({\"Name\":\"Test\"}, \"Name\", \"NewTest\")]", new Dictionary<string, string>()) as IDictionary<string, object?>;
        result.Should().ContainKey("Name").WhoseValue.Should().Be("NewTest");
    }

    [Fact]
    public void SetJsonProperty_OnString_Works()
    {
        var engine = TestHelper.CreateEngine();
        var result = engine.Execute("[SETJSONPROPERTY('{\"Name\":\"Test\"}', 'Name', 'NewTest')]", new Dictionary<string, string>()) as IDictionary<string, object?>;
        result.Should().ContainKey("Name").WhoseValue.Should().Be("NewTest");
    }

    [Fact]
    public void GetJsonProperty_Nested_Call()
    {
        var engine = TestHelper.CreateEngine();
        var result = engine.Execute("[TOUPPER([GETJSONPROPERTY({\"Name\":\"abc\"}, \"Name\")])]", new Dictionary<string, string>());
        result.Should().Be("ABC");
    }

    [Theory]
    [InlineData("[FIRST([1,2,3])]", 1d)]
    [InlineData("[FIRST([\"a\",\"b\",\"c\"])]", "a")]
    [InlineData("[FIRST([])]", null)]
    public void First_Works(string expr, object expected)
    {
        var engine = TestHelper.CreateEngine();
        var result = engine.Execute(expr, new Dictionary<string, string>());
        if (expected is double d)
            Convert.ToDouble(result).Should().Be(d);
        else
            result.Should().Be(expected);
    }

    [Fact]
    public void ObjectLiteral_As_Argument_Works()
    {
        var engine = TestHelper.CreateEngine();
        var expr = "[EQUALS([GETJSONPROPERTY({\"X\":1},\"X\")],1)]";
        var result = engine.Execute(expr, new Dictionary<string, string>());
        Convert.ToBoolean(result).Should().BeTrue();
    }

    [Fact]
    public void Array_Of_Objects_First_Property()
    {
        var engine = TestHelper.CreateEngine();
        var expr = "[GETJSONPROPERTY([FIRST([{\"X\":1},{\"X\":2}])],\"X\")]";
        var result = engine.Execute(expr, new Dictionary<string, string>());
        // FIRST returns first dict, then GETJSONPROPERTY extracts X
        Convert.ToDouble(result).Should().Be(1d);
    }
}
