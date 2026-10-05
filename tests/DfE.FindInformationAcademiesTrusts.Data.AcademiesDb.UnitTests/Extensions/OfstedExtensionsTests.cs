using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Extensions;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.UnitTests.Extensions;

public class OfstedExtensionsTests
{
    [Theory]
    [InlineData(null, CategoriesOfConcern.NotInspected)]
    [InlineData("", CategoriesOfConcern.NoConcerns)]
    [InlineData("NTI", CategoriesOfConcern.NoticeToImprove)]
    [InlineData("SM", CategoriesOfConcern.SpecialMeasures)]
    [InlineData("SWK", CategoriesOfConcern.SeriousWeakness)]
    public void ToCategoriesOfConcern_should_transform_given_string(string? input, CategoriesOfConcern expected)
    {
        input.ToCategoriesOfConcern().Should().Be(expected);
    }

    [Theory]
    [InlineData(null, SafeguardingScore.NotInspected)]
    [InlineData("NULL", SafeguardingScore.NotInspected)]
    [InlineData("Yes", SafeguardingScore.Yes)]
    [InlineData("No", SafeguardingScore.No)]
    [InlineData("9", SafeguardingScore.NotRecorded)]
    public void ToSafeguardingScore_should_transform_given_string(string? input, SafeguardingScore expected)
    {
        input.ToSafeguardingScore().Should().Be(expected);
    }

    [Theory]
    [InlineData("Not judged")]
    [InlineData("not judged")]
    [InlineData("NOT JUDGED")]
    [InlineData("NoT JuDgEd")]
    public void
        ConvertOverallEffectivenessToOfstedRatingScore_Should_Return_NoJudgement_When_Rating_Is_NotJudged_CaseInsensitive(
            string rating)
    {
        rating.ConvertOverallEffectivenessToOfstedRatingScore().Should()
            .Be(OfstedRatingScore.SingleHeadlineGradeNotAvailable);
    }

    [Theory]
    [InlineData(null, OfstedRatingScore.NotInspected)]
    [InlineData("-1", OfstedRatingScore.NotInspected)]
    [InlineData("1", OfstedRatingScore.Outstanding)]
    [InlineData("2", OfstedRatingScore.Good)]
    [InlineData("3", OfstedRatingScore.RequiresImprovement)]
    [InlineData("4", OfstedRatingScore.Inadequate)]
    [InlineData("8", OfstedRatingScore.DoesNotApply)]
    [InlineData("9", OfstedRatingScore.SingleHeadlineGradeNotAvailable)]
    [InlineData("0", OfstedRatingScore.InsufficientEvidence)]
    public void ConvertOverallEffectivenessToOfstedRatingScore_should_transform_given_string(string? rating,
        OfstedRatingScore expected)
    {
        rating.ConvertOverallEffectivenessToOfstedRatingScore().Should().Be(expected);
    }

    [Theory]
    [InlineData("5")]
    [InlineData("10")]
    [InlineData("-2")]
    public void
        ConvertOverallEffectivenessToOfstedRatingScore_should_return_unknown_when_rating_is_integer_not_defined_in_enum(
            string rating)
    {
        rating.ConvertOverallEffectivenessToOfstedRatingScore().Should().Be(OfstedRatingScore.Unknown);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("Good")]
    [InlineData("Outstanding")]
    [InlineData("Requires Improvement")]
    [InlineData("Inadequate")]
    [InlineData("N/A")]
    [InlineData("Unknown")]
    public void ConvertOverallEffectivenessToOfstedRatingScore_should_return_unknown_when_rating_is_invalid_string(
        string rating)
    {
        rating.ConvertOverallEffectivenessToOfstedRatingScore().Should().Be(OfstedRatingScore.Unknown);
    }

    [Theory]
    [InlineData("abc", OfstedRatingScore.Unknown)]
    [InlineData("Good", OfstedRatingScore.Unknown)]
    [InlineData("Unknown", OfstedRatingScore.Unknown)]
    [InlineData(null, OfstedRatingScore.NotInspected)]
    [InlineData("-1", OfstedRatingScore.NotInspected)]
    [InlineData("1", OfstedRatingScore.Outstanding)]
    [InlineData("2", OfstedRatingScore.Good)]
    [InlineData("3", OfstedRatingScore.RequiresImprovement)]
    [InlineData("4", OfstedRatingScore.Inadequate)]
    [InlineData("8", OfstedRatingScore.DoesNotApply)]
    [InlineData("9", OfstedRatingScore.SingleHeadlineGradeNotAvailable)]
    [InlineData("0", OfstedRatingScore.InsufficientEvidence)]
    public void ConvertNullableStringToOfstedRatingScore_should_transform_given_string(string? rating,
        OfstedRatingScore expected)
    {
        rating.ConvertNullableStringToOfstedRatingScore().Should().Be(expected);
    }

    [Theory]
    [InlineData(-20, OfstedRatingScore.Unknown)]
    [InlineData(null, OfstedRatingScore.NotInspected)]
    [InlineData(-1, OfstedRatingScore.NotInspected)]
    [InlineData(1, OfstedRatingScore.Outstanding)]
    [InlineData(2, OfstedRatingScore.Good)]
    [InlineData(3, OfstedRatingScore.RequiresImprovement)]
    [InlineData(4, OfstedRatingScore.Inadequate)]
    [InlineData(8, OfstedRatingScore.DoesNotApply)]
    [InlineData(9, OfstedRatingScore.SingleHeadlineGradeNotAvailable)]
    [InlineData(0, OfstedRatingScore.InsufficientEvidence)]
    public void ToOfstedRatingScore_should_transform_given_int(int? rating, OfstedRatingScore expected)
    {
        rating.ToString().ToOfstedRatingScore().Should().Be(expected);
    }
}
