using DfE.FindInformationAcademiesTrusts.Extensions;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Extensions;

public class HtmlStringExtensionsTests
{
    [Theory]
    [InlineData("Hello World!", "hello-world-")]
    [InlineData("Test_123", "test_123")]
    [InlineData("Space Test", "space-test")]
    [InlineData("!@#$%^&*()", "----------")]
    [InlineData("1234abcXYZ", "1234abcxyz")]
    public void Stub_ShouldReplaceNonAlphaNumericWithDash(string input, string expected)
    {
        var result = input.Stub();

        result.ToString().Should().Be(expected);
    }

    [Fact]
    public void Stub_ShouldHandleEmptyString()
    {
        var result = "".Stub();

        result.ToString().Should().Be("");
    }
}
