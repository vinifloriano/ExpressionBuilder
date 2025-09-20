namespace ExpressionEngine.Tests;

public class StringFunctionsTests
{
    [Fact]
    public void Split_Join_Replace_IndexOf()
    {
        var engine = TestHelper.CreateEngine();
        var split = engine.Execute("[SPLIT(\"a,b,c\",\",\")]", new Dictionary<string, string>()) as object[];
        split.Should().NotBeNull();
        split!.Length.Should().Be(3);

        var join = engine.Execute("[JOIN([\"a\",\"b\",\"c\"], \"-\")]", new Dictionary<string, string>());
        join.Should().Be("a-b-c");

        var repl = engine.Execute("[REPLACE(\"a-b-a\",\"-\",\"+\")]", new Dictionary<string, string>());
        repl.Should().Be("a+b+a");

        var idx = engine.Execute("[INDEXOF(\"hello\",\"l\")]", new Dictionary<string, string>());
        Convert.ToInt32(idx).Should().Be(2);
    }

    [Fact]
    public void Includes_Starts_Ends()
    {
        var engine = TestHelper.CreateEngine();
        Convert.ToBoolean(engine.Execute("[INCLUDES(\"abc\",\"b\")]", new Dictionary<string, string>())).Should().BeTrue();
        Convert.ToBoolean(engine.Execute("[STARTSWITH(\"abc\",\"a\")]", new Dictionary<string, string>())).Should().BeTrue();
        Convert.ToBoolean(engine.Execute("[ENDSWITH(\"abc\",\"c\")]", new Dictionary<string, string>())).Should().BeTrue();
    }

    [Fact]
    public void Base64_Roundtrip()
    {
        var engine = TestHelper.CreateEngine();
        var b64 = engine.Execute("[BASE64(\"hi\")]", new Dictionary<string, string>())!.ToString();
        var str = engine.Execute($"[BASE64TOSTRING(\"{b64}\")]", new Dictionary<string, string>())!.ToString();
        str.Should().Be("hi");
    }
}


