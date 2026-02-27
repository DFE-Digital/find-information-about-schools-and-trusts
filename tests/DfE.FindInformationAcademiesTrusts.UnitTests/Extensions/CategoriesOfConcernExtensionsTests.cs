using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Extensions;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Extensions;

public class CategoriesOfConcernExtensionsTests
{
    [Theory]
    [InlineData(CategoriesOfConcern.NotInspected, "Not inspected")]
    [InlineData(CategoriesOfConcern.DoesNotApply, "Does not apply")]
    [InlineData(CategoriesOfConcern.NoConcerns, "None")]
    [InlineData(CategoriesOfConcern.SpecialMeasures, "Special measures")]
    [InlineData(CategoriesOfConcern.SeriousWeakness, "Serious weakness")]
    [InlineData(CategoriesOfConcern.NoticeToImprove, "Notice to improve")]
    [InlineData((CategoriesOfConcern)(-999), "Unknown")] // Default case for unrecognized value
    public void ToDisplayString_ReturnsCorrectString_ForDefinedEnumValues(CategoriesOfConcern rating, string expected)
    {
        // Act
        var result = rating.ToDisplayString();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(CategoriesOfConcern.NotInspected, "not inspected")]
    [InlineData(CategoriesOfConcern.DoesNotApply, "does not apply")]
    [InlineData(CategoriesOfConcern.NoConcerns, "none")]
    [InlineData(CategoriesOfConcern.SpecialMeasures, "special measures")]
    [InlineData(CategoriesOfConcern.SeriousWeakness, "serious weakness")]
    [InlineData(CategoriesOfConcern.NoticeToImprove, "notice to improve")]
    [InlineData((CategoriesOfConcern)(-999), "unknown")] // Default case for unrecognized value
    public void ToDataSortValue_ReturnsCorrectValue_ForDefinedEnumValues(CategoriesOfConcern rating, string expected)
    {
        // Act
        var result = rating.ToDataSortValue();

        // Assert
        result.Should().Be(expected);
    }
}
