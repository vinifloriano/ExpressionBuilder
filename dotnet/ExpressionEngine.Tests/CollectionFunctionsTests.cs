namespace ExpressionEngine.Tests;

public class CollectionFunctionsTests
{
    [Fact]
    public void Union_Intersection_Last_Count_Skip_Take_Append()
    {
        var engine = TestHelper.CreateEngine();
        var union = engine.Execute("[UNION([1,2],[2,3])]", new Dictionary<string, string>()) as IList<object?>;
        union.Should().BeEquivalentTo(new object?[]{1d,2d,3d});

        var inter = engine.Execute("[INTERSECTION([1,2],[2,3])]", new Dictionary<string, string>()) as IList<object?>;
        inter.Should().BeEquivalentTo(new object?[]{2d});

        Convert.ToDouble(engine.Execute("[LAST([1,2,3])]", new Dictionary<string, string>())).Should().Be(3d);
        Convert.ToInt32(engine.Execute("[COUNT([1,2,3])]", new Dictionary<string, string>())).Should().Be(3);

        var skip = engine.Execute("[SKIP([1,2,3],1)]", new Dictionary<string, string>()) as IList<object?>;
        skip.Should().BeEquivalentTo(new object?[]{2d,3d});
        var take = engine.Execute("[TAKE([1,2,3],2)]", new Dictionary<string, string>()) as IList<object?>;
        take.Should().BeEquivalentTo(new object?[]{1d,2d});
        var append = engine.Execute("[APPEND([1,2],3)]", new Dictionary<string, string>()) as IList<object?>;
        append.Should().BeEquivalentTo(new object?[]{1d,2d,3d});
    }

    [Fact]
    public void SumBy_ConcatBy_Empty_Between_Includes()
    {
        var engine = TestHelper.CreateEngine();
        var sum = engine.Execute("[SUMBYPROPERTY([{\"v\":1},{\"v\":2}],\"v\")]", new Dictionary<string, string>());
        Convert.ToDouble(sum).Should().Be(3d);
        var concat = engine.Execute("[CONCATBYPROPERTY([{\"n\":\"a\"},{\"n\":\"b\"}],\"n\",\"-\")]", new Dictionary<string, string>()).ToString();
        concat.Should().Be("a-b");
        Convert.ToBoolean(engine.Execute("[EMPTY([])]", new Dictionary<string, string>())).Should().BeTrue();
        Convert.ToBoolean(engine.Execute("[BETWEEN(5,1,10,true)]", new Dictionary<string, string>())).Should().BeTrue();
        Convert.ToBoolean(engine.Execute("[INCLUDES([1,2,3],2)]", new Dictionary<string, string>())).Should().BeTrue();
    }
}


