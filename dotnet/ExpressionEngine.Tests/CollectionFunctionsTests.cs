namespace ExpressionEngine.Tests;

public class CollectionFunctionsTests
{
    [Theory]
    [InlineData("[UNION([1,2],[2,3])]", new object[] { 1d, 2d, 3d })]
    [InlineData("[UNION([1,2],[3,4])]", new object[] { 1d, 2d, 3d, 4d })]
    [InlineData("[UNION([], [1,2])]", new object[] { 1d, 2d })]
    [InlineData("[UNION([1,2], [])]", new object[] { 1d, 2d })]
    [InlineData("[UNION([], [])]", new object[] { })]
    public void Union_Works(string expr, object[] expected)
    {
        var engine = TestHelper.CreateEngine();
        var result = engine.Execute(expr, new Dictionary<string, string>()) as IList<object?>;
        result.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("[INTERSECTION([1,2],[2,3])]", new object[] { 2d })]
    [InlineData("[INTERSECTION([1,2],[3,4])]", new object[] { })]
    [InlineData("[INTERSECTION([1,2,2],[2,3])]", new object[] { 2d })]
    [InlineData("[INTERSECTION([], [1,2])]", new object[] { })]
    [InlineData("[INTERSECTION([1,2], [])]", new object[] { })]
    public void Intersection_Works(string expr, object[] expected)
    {
        var engine = TestHelper.CreateEngine();
        var result = engine.Execute(expr, new Dictionary<string, string>()) as IList<object?>;
        result.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("[LAST([1,2,3])]", 3d)]
    [InlineData("[LAST([\"a\",\"b\",\"c\"])]", "c")]
    [InlineData("[LAST([])]", null)]
    public void Last_Works(string expr, object expected)
    {
        var engine = TestHelper.CreateEngine();
        engine.Execute(expr, new Dictionary<string, string>()).Should().Be(expected);
    }

    [Theory]
    [InlineData("[COUNT([1,2,3])]", 3)]
    [InlineData("[COUNT([])]", 0)]
    [InlineData("[COUNT(\"hello\")]", 5)]
    public void Count_Works(string expr, int expected)
    {
        var engine = TestHelper.CreateEngine();
        engine.Execute(expr, new Dictionary<string, string>()).Should().Be(expected);
    }

    [Theory]
    [InlineData("[SKIP([1,2,3],1)]", new object[] { 2d, 3d })]
    [InlineData("[SKIP([1,2,3],0)]", new object[] { 1d, 2d, 3d })]
    [InlineData("[SKIP([1,2,3],3)]", new object[] { })]
    [InlineData("[SKIP([], 1)]", new object[] { })]
    public void Skip_Works(string expr, object[] expected)
    {
        var engine = TestHelper.CreateEngine();
        var result = engine.Execute(expr, new Dictionary<string, string>()) as IList<object?>;
        result.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("[TAKE([1,2,3],2)]", new object[] { 1d, 2d })]
    [InlineData("[TAKE([1,2,3],0)]", new object[] { })]
    [InlineData("[TAKE([1,2,3],3)]", new object[] { 1d, 2d, 3d })]
    [InlineData("[TAKE([], 1)]", new object[] { })]
    public void Take_Works(string expr, object[] expected)
    {
        var engine = TestHelper.CreateEngine();
        var result = engine.Execute(expr, new Dictionary<string, string>()) as IList<object?>;
        result.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("[APPEND([1,2],3)]", new object[] { 1d, 2d, 3d })]
    [InlineData("[APPEND([], 1)]", new object[] { 1d })]
    [InlineData(@"[APPEND([""a"",""b""], ""c"")]", new object[] { "a", "b", "c" })]
    public void Append_Works(string expr, object[] expected)
    {
        var engine = TestHelper.CreateEngine();
        var result = engine.Execute(expr, new Dictionary<string, string>()) as IList<object?>;
        result.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("[SUMBYPROPERTY([{\"v\":1},{\"v\":2}],\"v\")]", 3d)]
    [InlineData("[SUMBYPROPERTY([{\"v\":1.5},{\"v\":2.5}],\"v\")]", 4d)]
    [InlineData("[SUMBYPROPERTY([],\"v\")]", 0d)]
    public void SumByProperty_Works(string expr, double expected)
    {
        var engine = TestHelper.CreateEngine();
        Convert.ToDouble(engine.Execute(expr, new Dictionary<string, string>())).Should().Be(expected);
    }

    [Theory]
    [InlineData("[CONCATBYPROPERTY([{\"n\":\"a\"},{\"n\":\"b\"}],\"n\",\"-\")]", "a-b")]
    [InlineData("[CONCATBYPROPERTY([{\"n\":\"a\"},{\"n\":\"b\"}],\"n\",\", \")]", "a, b")]
    [InlineData("[CONCATBYPROPERTY([],\"n\",\"-\")]", "")]
    public void ConcatByProperty_Works(string expr, string expected)
    {
        var engine = TestHelper.CreateEngine();
        engine.Execute(expr, new Dictionary<string, string>()).Should().Be(expected);
    }

    [Theory]
    [InlineData("[EMPTY([])]", true)]
    [InlineData("[EMPTY([1])]", false)]
    [InlineData("[]", true, Skip = "Not a valid expression on its own")]
    public void Empty_Works(string expr, bool expected)
    {
        var engine = TestHelper.CreateEngine();
        Convert.ToBoolean(engine.Execute(expr, new Dictionary<string, string>())).Should().Be(expected);
    }

    [Theory]
    [InlineData("[INCLUDES([1,2,3],2)]", true)]
    [InlineData("[INCLUDES([1,2,3],4)]", false)]
    [InlineData("[INCLUDES([\"a\",\"b\",\"c\"],\"b\")]", true)]
    [InlineData("[INCLUDES([], 1)]", false)]
    public void Includes_Works(string expr, bool expected)
    {
        var engine = TestHelper.CreateEngine();
        Convert.ToBoolean(engine.Execute(expr, new Dictionary<string, string>())).Should().Be(expected);
    }
}
