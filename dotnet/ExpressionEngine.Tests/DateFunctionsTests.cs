namespace ExpressionEngine.Tests;

public class DateFunctionsTests
{
    [Fact]
    public void AddDays_Hours_Minutes_Seconds()
    {
        var engine = TestHelper.CreateEngine();
        var baseDate = "2024-01-01T00:00:00Z";
        engine.Execute($"[ADDDAYS(\"{baseDate}\",1)]", new Dictionary<string, string>()).Should().Be("2024-01-02T00:00:00.0000000Z");
        engine.Execute($"[ADDHOURS(\"{baseDate}\",1)]", new Dictionary<string, string>()).Should().Be("2024-01-01T01:00:00.0000000Z");
        engine.Execute($"[ADDMINUTES(\"{baseDate}\",1)]", new Dictionary<string, string>()).Should().Be("2024-01-01T00:01:00.0000000Z");
        engine.Execute($"[ADDSECONDS(\"{baseDate}\",1)]", new Dictionary<string, string>()).Should().Be("2024-01-01T00:00:01.0000000Z");
    }

    [Fact]
    public void Format_Today_UtcNow()
    {
        var engine = TestHelper.CreateEngine();
        var d = engine.Execute("[FORMATDATETIME(\"2024-01-01T12:34:56Z\",\"yyyy-MM-dd HH:mm:ss\")]", new Dictionary<string, string>())!.ToString();
        d.Should().Be("2024-01-01 12:34:56");

        engine.Execute("[TODAY()]", new Dictionary<string, string>())!.ToString().Should().EndWith("T00:00:00.0000000Z");
        engine.Execute("[UTCNOW()]", new Dictionary<string, string>())!.ToString().Should().EndWith("Z");
    }

    [Fact]
    public void Day_Diff_Ticks()
    {
        var engine = TestHelper.CreateEngine();
        Convert.ToInt32(engine.Execute("[DAYOFWEEK(\"2024-01-07T00:00:00Z\")]", new Dictionary<string, string>())).Should().Be(0); // Sunday
        Convert.ToInt32(engine.Execute("[DAYOFMONTH(\"2024-01-07T00:00:00Z\")]", new Dictionary<string, string>())).Should().Be(7);
        Convert.ToInt32(engine.Execute("[DAYOFYEAR(\"2024-12-31T00:00:00Z\")]", new Dictionary<string, string>())).Should().Be(366);
        Convert.ToInt32(engine.Execute("[DATEDIFF(\"2024-01-01T00:00:00Z\",\"2024-01-03T00:00:00Z\",\"days\")]", new Dictionary<string, string>())).Should().Be(2);
        var ticks = engine.Execute("[TICKS(\"2024-01-01T00:00:00Z\")]", new Dictionary<string, string>());
        Convert.ToInt64(ticks).Should().BeGreaterThan(0);
    }
}


