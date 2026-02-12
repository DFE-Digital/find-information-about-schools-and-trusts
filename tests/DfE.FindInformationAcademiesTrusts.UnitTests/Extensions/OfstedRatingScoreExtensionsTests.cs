using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Extensions;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Extensions;

public class OfstedRatingScoreExtensionsTests
{
    [Theory]
    [InlineData(OfstedRatingScore.Outstanding, "Outstanding", true)]
    [InlineData(OfstedRatingScore.Outstanding, "Outstanding", false)]
    [InlineData(OfstedRatingScore.Good, "Good", true)]
    [InlineData(OfstedRatingScore.Good, "Good", false)]
    [InlineData(OfstedRatingScore.RequiresImprovement, "Requires improvement", true)]
    [InlineData(OfstedRatingScore.RequiresImprovement, "Requires improvement", false)]
    [InlineData(OfstedRatingScore.Inadequate, "Inadequate", true)]
    [InlineData(OfstedRatingScore.Inadequate, "Inadequate", false)]
    [InlineData(OfstedRatingScore.InsufficientEvidence, "Insufficient evidence", true)]
    [InlineData(OfstedRatingScore.InsufficientEvidence, "Insufficient evidence", false)]
    [InlineData(OfstedRatingScore.SingleHeadlineGradeNotAvailable, "Not available", true)]
    [InlineData(OfstedRatingScore.SingleHeadlineGradeNotAvailable, "Not available", false)]
    [InlineData(OfstedRatingScore.DoesNotApply, "Does not apply", true)]
    [InlineData(OfstedRatingScore.DoesNotApply, "Does not apply", false)]
    [InlineData(OfstedRatingScore.NotInspected, "Not inspected", true)]
    [InlineData(OfstedRatingScore.NotInspected, "Not inspected", false)]
    [InlineData((OfstedRatingScore)999, "Unknown", true)]
    [InlineData((OfstedRatingScore)999, "Unknown", false)]
    public void ToDisplayString_ReturnsCorrectString(OfstedRatingScore rating, string expected, bool isCurrent)
    {
        // Act
        var result = rating.ToDisplayString(isCurrent);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(OfstedRatingScore.Outstanding, 1)]
    [InlineData(OfstedRatingScore.Good, 2)]
    [InlineData(OfstedRatingScore.RequiresImprovement, 3)]
    [InlineData(OfstedRatingScore.Inadequate, 4)]
    [InlineData(OfstedRatingScore.InsufficientEvidence, 5)]
    [InlineData(OfstedRatingScore.SingleHeadlineGradeNotAvailable, 6)]
    [InlineData(OfstedRatingScore.DoesNotApply, 7)]
    [InlineData(OfstedRatingScore.NotInspected, 8)]
    public void ToDataSortValue_ReturnsCorrectValue_ForDefinedEnumValues(OfstedRatingScore rating, int expected)
    {
        // Act
        var result = rating.ToDataSortValue();

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ToDataSortValue_ReturnsUnknown_ForUndefinedEnumValue()
    {
        // Arrange
        // Cast an undefined integer value to the enum
        var undefinedRating = (OfstedRatingScore)999;

        // Act
        var result = undefinedRating.ToDataSortValue();

        // Assert
        result.Should().Be(-1);
    }
}
