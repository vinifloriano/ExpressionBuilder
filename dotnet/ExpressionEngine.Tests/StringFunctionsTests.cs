namespace ExpressionEngine.Tests;

public class StringFunctionsTests
{
    [Theory]
    [InlineData(@"[CONCAT(""hello"", "" "", ""world"")]", "hello world")]
    [InlineData(@"[CONCAT(""a"", ""b"", ""c"", ""d"")]", "abcd")]
    [InlineData(@"[CONCAT(""a"")]", "a")]
    [InlineData(@"[CONCAT("""", """")]", "")]
    public void Concat_Works(string expr, string expected)
    {
        var engine = TestHelper.CreateEngine();
        engine.Execute(expr, new Dictionary<string, string>()).Should().Be(expected);
    }

    [Theory]
    [InlineData("[TOUPPER(\"hello\")]", "HELLO")]
    [InlineData("[TOUPPER(\"HeLLo\")]", "HELLO")]
    [InlineData("[TOUPPER(\"\")]", "")]
    public void ToUpper_Works(string expr, string expected)
    {
        var engine = TestHelper.CreateEngine();
        engine.Execute(expr, new Dictionary<string, string>()).Should().Be(expected);
    }

    [Theory]
    [InlineData("[TOLOWER(\"HELLO\")]", "hello")]
    [InlineData("[TOLOWER(\"HeLLo\")]", "hello")]
    [InlineData("[TOLOWER(\"\")]", "")]
    public void ToLower_Works(string expr, string expected)
    {
        var engine = TestHelper.CreateEngine();
        engine.Execute(expr, new Dictionary<string, string>()).Should().Be(expected);
    }

    [Theory]
    [InlineData("[TRIM(\"  hello  \")]", "hello")]
    [InlineData("[TRIM(\"hello\")]", "hello")]
    [InlineData("[TRIM(\"  \")]", "")]
    [InlineData("[TRIM(\"\")]", "")]
    public void Trim_Works(string expr, string expected)
    {
        var engine = TestHelper.CreateEngine();
        engine.Execute(expr, new Dictionary<string, string>()).Should().Be(expected);
    }

    [Theory]
    [InlineData("[LENGTH(\"hello\")]", 5)]
    [InlineData("[LENGTH(\"\")]", 0)]
    public void Length_Works(string expr, int expected)
    {
        var engine = TestHelper.CreateEngine();
        engine.Execute(expr, new Dictionary<string, string>()).Should().Be(expected);
    }

    [Theory]
    [InlineData(@"[SUBSTRING(""hello"", 0, 3)]", "hel")]
    [InlineData(@"[SUBSTRING(""hello"", 1, 2)]", "el")]
    [InlineData(@"[SUBSTRING(""hello"", 4, 1)]", "o")]
    public void Substring_Works(string expr, string expected)
    {
        var engine = TestHelper.CreateEngine();
        engine.Execute(expr, new Dictionary<string, string>()).Should().Be(expected);
    }

    [Theory]
    [InlineData(@"[SPLIT(""a,b,c"", "","")]", new[] { "a", "b", "c" })]
    [InlineData(@"[SPLIT(""a-b-c"", ""-"")]", new[] { "a", "b", "c" })]
    [InlineData(@"[SPLIT(""a,b,c"", ""."")]", new[] { "a,b,c" })]
    [InlineData(@"[SPLIT("""", "","")]", new[] { "" })]
    public void Split_Works(string expr, object[] expected)
    {
        var engine = TestHelper.CreateEngine();
        var result = engine.Execute(expr, new Dictionary<string, string>()) as object[];
        result.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(@"[JOIN([""a"",""b"",""c""], ""-"")]", "a-b-c")]
    [InlineData(@"[JOIN([""a"",""b"",""c""], """")]", "abc")]
    [InlineData(@"[JOIN([], ""-"")]", "")]
    public void Join_Works(string expr, string expected)
    {
        var engine = TestHelper.CreateEngine();
        engine.Execute(expr, new Dictionary<string, string>()).Should().Be(expected);
    }

    [Theory]
    [InlineData(@"[REPLACE(""a-b-a"",""-"",""+"")]", "a+b+a")]
    [InlineData(@"[REPLACE(""hello"",""l"",""x"")]", "hexxo")]
    [InlineData(@"[REPLACE(""hello"",""z"",""x"")]", "hello")]
    public void Replace_Works(string expr, string expected)
    {
        var engine = TestHelper.CreateEngine();
        engine.Execute(expr, new Dictionary<string, string>()).Should().Be(expected);
    }

    [Theory]
    [InlineData(@"[INDEXOF(""hello"",""l"")]", 2)]
    [InlineData(@"[INDEXOF(""hello"",""o"")]", 4)]
    [InlineData(@"[INDEXOF(""hello"",""z"")]", -1)]
    public void IndexOf_Works(string expr, int expected)
    {
        var engine = TestHelper.CreateEngine();
        engine.Execute(expr, new Dictionary<string, string>()).Should().Be(expected);
    }

    [Theory]
    [InlineData("[INCLUDES(\"abc\",\"b\")]", true)]
    [InlineData("[INCLUDES(\"abc\",\"d\")]", false)]
    [InlineData("[INCLUDES(\"abc\",\"\")]", true)]
    public void Includes_Works(string expr, bool expected)
    {
        var engine = TestHelper.CreateEngine();
        Convert.ToBoolean(engine.Execute(expr, new Dictionary<string, string>())).Should().Be(expected);
    }

    [Theory]
    [InlineData("[STARTSWITH(\"abc\",\"a\")]", true)]
    [InlineData("[STARTSWITH(\"abc\",\"b\")]", false)]
    [InlineData("[STARTSWITH(\"abc\",\"\")]", true)]
    public void StartsWith_Works(string expr, bool expected)
    {
        var engine = TestHelper.CreateEngine();
        Convert.ToBoolean(engine.Execute(expr, new Dictionary<string, string>())).Should().Be(expected);
    }

    [Theory]
    [InlineData("[ENDSWITH(\"abc\",\"c\")]", true)]
    [InlineData("[ENDSWITH(\"abc\",\"b\")]", false)]
    [InlineData("[ENDSWITH(\"abc\",\"\")]", true)]
    public void EndsWith_Works(string expr, bool expected)
    {
        var engine = TestHelper.CreateEngine();
        Convert.ToBoolean(engine.Execute(expr, new Dictionary<string, string>())).Should().Be(expected);
    }

    [Theory]
    [InlineData("[BASE64(\"hi\")]", "aGk=")]
    [InlineData("[BASE64(\"hello world\")]", "aGVsbG8gd29ybGQ=")]
    [InlineData("[BASE64(\"\")]", "")]
    public void Base64_Works(string expr, string expected)
    {
        var engine = TestHelper.CreateEngine();
        engine.Execute(expr, new Dictionary<string, string>()).Should().Be(expected);
    }

    [Theory]
    [InlineData("[BASE64TOSTRING(\"aGk=\")]", "hi")]
    [InlineData("[BASE64TOSTRING(\"aGVsbG8gd29ybGQ=\")]", "hello world")]
    [InlineData("[BASE64TOSTRING(\"\")]", "")]
    public void Base64ToString_Works(string expr, string expected)
    {
        var engine = TestHelper.CreateEngine();
        engine.Execute(expr, new Dictionary<string, string>()).Should().Be(expected);
    }
}
