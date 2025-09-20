namespace ExpressionEngine.Tests;

public class DateFunctionsTests
{
    private const string BaseDate = "2024-01-01T00:00:00.0000000Z";
    private const string LeapDate = "2024-02-29T12:00:00.0000000Z";

    [Theory]
    [InlineData("[ADDDAYS(\"2024-01-01T00:00:00Z\", 1)]", "2024-01-02T00:00:00.0000000Z")]
    [InlineData("[ADDDAYS(\"2024-01-01T00:00:00Z\", -1)]", "2023-12-31T00:00:00.0000000Z")]
    [InlineData("[ADDDAYS(\"2024-02-28T00:00:00Z\", 1)]", "2024-02-29T00:00:00.0000000Z")] // Leap year
    public void AddDays_Works(string expr, string expected)
    {
        var engine = TestHelper.CreateEngine();
        engine.Execute(expr, new Dictionary<string, string>()).Should().Be(expected);
    }

    [Theory]
    [InlineData("[ADDHOURS(\"2024-01-01T00:00:00Z\", 1)]", "2024-01-01T01:00:00.0000000Z")]
    [InlineData("[ADDHOURS(\"2024-01-01T00:00:00Z\", -1)]", "2023-12-31T23:00:00.0000000Z")]
    [InlineData("[ADDHOURS(\"2024-01-01T00:00:00Z\", 24)]", "2024-01-02T00:00:00.0000000Z")]
    public void AddHours_Works(string expr, string expected)
    {
        var engine = TestHelper.CreateEngine();
        engine.Execute(expr, new Dictionary<string, string>()).Should().Be(expected);
    }

    [Theory]
    [InlineData("[ADDMINUTES(\"2024-01-01T00:00:00Z\", 1)]", "2024-01-01T00:01:00.0000000Z")]
    [InlineData("[ADDMINUTES(\"2024-01-01T00:00:00Z\", -1)]", "2023-12-31T23:59:00.0000000Z")]
    [InlineData("[ADDMINUTES(\"2024-01-01T00:00:00Z\", 60)]", "2024-01-01T01:00:00.0000000Z")]
    public void AddMinutes_Works(string expr, string expected)
    {
        var engine = TestHelper.CreateEngine();
        engine.Execute(expr, new Dictionary<string, string>()).Should().Be(expected);
    }

    [Theory]
    [InlineData("[ADDSECONDS(\"2024-01-01T00:00:00Z\", 1)]", "2024-01-01T00:00:01.0000000Z")]
    [InlineData("[ADDSECONDS(\"2024-01-01T00:00:00Z\", -1)]", "2023-12-31T23:59:59.0000000Z")]
    [InlineData("[ADDSECONDS(\"2024-01-01T00:00:00Z\", 60)]", "2024-01-01T00:01:00.0000000Z")]
    public void AddSeconds_Works(string expr, string expected)
    {
        var engine = TestHelper.CreateEngine();
        engine.Execute(expr, new Dictionary<string, string>()).Should().Be(expected);
    }

    [Theory]
    [InlineData("[FORMATDATETIME(\"2024-01-01T12:34:56Z\",\"yyyy-MM-dd HH:mm:ss\")]", "2024-01-01 12:34:56")]
    [InlineData("[FORMATDATETIME(\"2024-01-01T12:34:56Z\",\"dd-MM-yyyy\")]", "01-01-2024")]
    [InlineData("[FORMATDATETIME(\"2024-01-01T12:34:56Z\",\"o\")]", "2024-01-01T12:34:56.0000000Z")]
    public void FormatDateTime_Works(string expr, string expected)
    {
        var engine = TestHelper.CreateEngine();
        engine.Execute(expr, new Dictionary<string, string>()).Should().Be(expected);
    }

    [Fact]
    public void UtcNow_And_Today_Work()
    {
        var engine = TestHelper.CreateEngine();
        var nowStr = engine.Execute("[UTCNOW()]", new Dictionary<string, string>()) as string;
        var todayStr = engine.Execute("[TODAY()]", new Dictionary<string, string>()) as string;

        nowStr.Should().NotBeNull();
        todayStr.Should().NotBeNull();

        var now = DateTime.Parse(nowStr!, null, System.Globalization.DateTimeStyles.AdjustToUniversal);
        var today = DateTime.Parse(todayStr!, null, System.Globalization.DateTimeStyles.AdjustToUniversal);

        now.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        today.TimeOfDay.Should().Be(TimeSpan.Zero);
        today.Date.Should().Be(DateTime.UtcNow.Date);
    }

    [Theory]
    [InlineData("[DAYOFWEEK(\"2024-01-07T00:00:00Z\")]", 0)] // Sunday
    [InlineData("[DAYOFWEEK(\"2024-01-01T00:00:00Z\")]", 1)] // Monday
    [InlineData("[DAYOFWEEK(\"2024-01-06T00:00:00Z\")]", 6)] // Saturday
    public void DayOfWeek_Works(string expr, int expected)
    {
        var engine = TestHelper.CreateEngine();
        engine.Execute(expr, new Dictionary<string, string>()).Should().Be(expected);
    }

    [Theory]
    [InlineData("[DAYOFMONTH(\"2024-01-07T00:00:00Z\")]", 7)]
    [InlineData("[DAYOFMONTH(\"2024-02-29T00:00:00Z\")]", 29)]
    public void DayOfMonth_Works(string expr, int expected)
    {
        var engine = TestHelper.CreateEngine();
        engine.Execute(expr, new Dictionary<string, string>()).Should().Be(expected);
    }

    [Theory]
    [InlineData("[DAYOFYEAR(\"2024-01-01T00:00:00Z\")]", 1)]
    [InlineData("[DAYOFYEAR(\"2024-12-31T00:00:00Z\")]", 366)] // Leap
    [InlineData("[DAYOFYEAR(\"2023-12-31T00:00:00Z\")]", 365)]
    public void DayOfYear_Works(string expr, int expected)
    {
        var engine = TestHelper.CreateEngine();
        engine.Execute(expr, new Dictionary<string, string>()).Should().Be(expected);
    }

    [Theory]
    [InlineData("[DATEDIFF(\"2024-01-01T00:00:00Z\",\"2024-01-03T00:00:00Z\",\"days\")]", 2)]
    [InlineData("[DATEDIFF(\"2024-01-01T00:00:00Z\",\"2024-01-01T02:00:00Z\",\"hours\")]", 2)]
    [InlineData("[DATEDIFF(\"2024-01-01T00:00:00Z\",\"2024-01-01T00:02:00Z\",\"minutes\")]", 2)]
    [InlineData("[DATEDIFF(\"2024-01-01T00:00:00Z\",\"2024-01-01T00:00:02Z\",\"seconds\")]", 2)]
    [InlineData("[DATEDIFF(\"2024-01-01T00:00:00Z\",\"2024-01-01T00:00:00.002Z\")]", 2L)]
    public void DateDiff_Works(string expr, object expected)
    {
        var engine = TestHelper.CreateEngine();
        engine.Execute(expr, new Dictionary<string, string>()).Should().Be(expected);
    }

    [Fact]
    public void Ticks_Works()
    {
        var engine = TestHelper.CreateEngine();
        var ticks = engine.Execute($"[TICKS(\"{BaseDate}\")]", new Dictionary<string, string>());
        var expected = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero).Ticks;
        Convert.ToInt64(ticks).Should().Be(expected);
    }
}
