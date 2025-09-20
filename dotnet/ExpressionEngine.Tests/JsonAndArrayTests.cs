namespace ExpressionEngine.Tests;

public class JsonAndArrayTests
{
    [Fact]
    public void GetJsonProperty_Works()
    {
        var engine = TestHelper.CreateEngine();
        var result = engine.Execute("[GETJSONPROPERTY({\"Name\":\"Test\"}, \"Name\")]", new Dictionary<string, string>());
        result.Should().Be("Test");
    }

    [Fact]
    public void GetJsonProperty_Nested_Call()
    {
        var engine = TestHelper.CreateEngine();
        var result = engine.Execute("[TOUPPER([GETJSONPROPERTY({\"Name\":\"abc\"}, \"Name\")])]", new Dictionary<string, string>());
        result.Should().Be("ABC");
    }

    [Fact]
    public void First_On_ArrayLiteral_Works()
    {
        var engine = TestHelper.CreateEngine();
        var result = engine.Execute("[FIRST([1,2,3])]", new Dictionary<string, string>());
        Convert.ToDouble(result).Should().Be(1d);
    }

    [Fact]
    public void First_On_StringArray_Works()
    {
        var engine = TestHelper.CreateEngine();
        var result = engine.Execute("[FIRST([\"a\",\"b\",\"c\"]) ]", new Dictionary<string, string>());
        result.Should().Be("a");
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


