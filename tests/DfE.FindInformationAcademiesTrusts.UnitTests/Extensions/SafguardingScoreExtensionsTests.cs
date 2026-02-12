using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Extensions;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Extensions;

public class SafeguardingScoreExtensionsTests
{
    [Theory]
    [InlineData(SafeguardingScore.NotInspected, "Not inspected")]
    [InlineData(SafeguardingScore.Yes, "Yes")]
    [InlineData(SafeguardingScore.No, "No")]
    [InlineData(SafeguardingScore.NotRecorded, "Not recorded")]
    [InlineData((SafeguardingScore)(-999), "Unknown")]
    public void ToDisplayString_ReturnsCorrectString_ForDefinedEnumValues(SafeguardingScore rating, string expected)
    {
        // Act
        var result = rating.ToDisplayString();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(SafeguardingScore.NotInspected, "not inspected")]
    [InlineData(SafeguardingScore.Yes, "yes")]
    [InlineData(SafeguardingScore.No, "no")]
    [InlineData(SafeguardingScore.NotRecorded, "not recorded")]
    [InlineData((SafeguardingScore)(-999), "unknown")]
    public void ToDataSortValue_ReturnsCorrectValue_ForDefinedEnumValues(SafeguardingScore rating, string expected)
    {
        // Act
        var result = rating.ToDataSortValue();

        // Assert
        result.Should().Be(expected);
    }
}
