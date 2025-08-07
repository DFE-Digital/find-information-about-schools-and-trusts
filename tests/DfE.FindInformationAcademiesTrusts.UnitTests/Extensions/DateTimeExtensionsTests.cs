using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.Pages;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Extensions;

public class DateTimeExtensionsTests
{
    [Fact]
    public void ShowDateStringOrReplaceWithText_ReturnsFormattedDate_WhenNotNull()
    {
        DateTime? testTime = DateTime.Today;
        var result = testTime.ShowDateStringOrReplaceWithText();
        result.Should().BeEquivalentTo(DateTime.Today.ToString(StringFormatConstants.DisplayDateFormat));
    }

    [Fact]
    public void ShowDateStringOrReplaceWithText_returns_NoData_WhenNull()
    {
        DateTime? testTime = null;
        var result = testTime.ShowDateStringOrReplaceWithText();
        result.Should().Be("No data");
    }

    [Fact]
    public void ShowFullDateStringOrReplaceWithText_ReturnsFormattedDate_WhenNotNull()
    {
        DateTime? testTime = DateTime.Today;
        var result = testTime.ShowFullDateStringOrReplaceWithText();
        result.Should().BeEquivalentTo(DateTime.Today.ToString(StringFormatConstants.DisplayFullDateFormat));
    }

    [Fact]
    public void ShowFullDateStringOrReplaceWithText_returns_NoData_WhenNull()
    {
        DateTime? testTime = null;
        var result = testTime.ShowFullDateStringOrReplaceWithText();
        result.Should().Be("No data");
    }

    [Theory]
    [InlineData(2024, 12, 31, "20241231")]
    [InlineData(2024, 12, 03, "20241203")]
    [InlineData(2025, 01, 01, "20250101")]
    [InlineData(2025, 05, 13, "20250513")]
    public void ToDataSortValue_returns_numerical_representation_of_date(int year, int month, int day, string expected)
    {
        var testTime = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
        var result = testTime.ToDataSortValue();
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(2024, 12, 31, "20241231")]
    [InlineData(2024, 12, 03, "20241203")]
    [InlineData(2025, 01, 01, "20250101")]
    [InlineData(2025, 05, 13, "20250513")]
    public void ToDataSortValue_nullable_returns_numerical_representation_of_date(int year, int month, int day,
        string expected)
    {
        DateTime? testTime = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
        var result = testTime.ToDataSortValue();
        result.Should().Be(expected);
    }

    [Fact]
    public void ToDataSortValue_nullable_returns_empty_string_when_null()
    {
        DateTime? testTime = null;
        var result = testTime.ToDataSortValue();
        result.Should().BeEmpty();
    }
}
