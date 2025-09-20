namespace ExpressionEngine.Tests;

public class XmlFunctionsTests
{
    [Theory]
    [InlineData("<root><item>value</item></root>", "item", "value")]
    [InlineData("<root><item>value</item><item>value2</item></root>", "item", "value")]
    [InlineData("<root><parent><child>value</child></parent></root>", "child", "value")]
    [InlineData("<root></root>", "item", null)]
    public void GetXmlProperty_Works(string xml, string tag, string expected)
    {
        var engine = TestHelper.CreateEngine();
        engine.Execute($"[GETXMLPROPERTY('{xml}', '{tag}')]", new Dictionary<string, string>()).Should().Be(expected);
    }

    [Theory]
    [InlineData("<root><item>value</item></root>", "item", "newValue", "<root><item>newValue</item></root>")]
    [InlineData("<root></root>", "item", "newValue", "<root><item>newValue</item></root>")]
    [InlineData("<root><item>value</item></root>", "newItem", "newValue", "<root><item>value</item><newItem>newValue</newItem></root>")]
    public void SetXmlProperty_Works(string xml, string tag, string value, string expected)
    {
        var engine = TestHelper.CreateEngine();
        var newXml = (string)engine.Execute($"[SETXMLPROPERTY('{xml}', '{tag}', '{value}')]", new Dictionary<string, string>());
        System.Xml.Linq.XDocument.Parse(newXml).ToString(System.Xml.Linq.SaveOptions.DisableFormatting)
            .Should().Be(System.Xml.Linq.XDocument.Parse(expected).ToString(System.Xml.Linq.SaveOptions.DisableFormatting));
    }

    [Fact]
    public void SetXmlProperty_AddsToRoot_IfNoRoot()
    {
        var engine = TestHelper.CreateEngine();
        var newXml = (string)engine.Execute($"[SETXMLPROPERTY('', 'item', 'newValue')]", new Dictionary<string, string>());
        newXml.Should().Be("<root><item>newValue</item></root>");
    }
}
