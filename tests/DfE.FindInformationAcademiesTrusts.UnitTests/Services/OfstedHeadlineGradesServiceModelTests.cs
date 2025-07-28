using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Services.School;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Services;

public class OfstedHeadlineGradesServiceModelTests
{
    [Fact]
    public void
        HasRecentShortInspection_returns_true_when_ShortInspection_InspectionDate_is_after_CurrentInspection_InspectionDate()
    {
        new OfstedHeadlineGradesServiceModel(
                new OfstedShortInspection(new DateTime(2022, 1, 1), "School remains Good"),
                new OfstedFullInspectionSummary(new DateTime(2021, 1, 1), OfstedRatingScore.Good),
                new OfstedFullInspectionSummary(new DateTime(2011, 1, 1), OfstedRatingScore.RequiresImprovement))
            .HasRecentShortInspection.Should().BeTrue();
    }

    [Theory]
    [InlineData(2021)]
    [InlineData(2020)]
    public void
        HasRecentShortInspection_returns_false_when_ShortInspection_InspectionDate_is_not_after_CurrentInspection_InspectionDate(int year)
    {
        new OfstedHeadlineGradesServiceModel(
                new OfstedShortInspection(new DateTime(year, 1, 1), "School remains Good"),
                new OfstedFullInspectionSummary(new DateTime(2021, 1, 1), OfstedRatingScore.Good),
                new OfstedFullInspectionSummary(new DateTime(2011, 1, 1), OfstedRatingScore.RequiresImprovement))
            .HasRecentShortInspection.Should().BeFalse();
    }

    [Fact]
    public void HasRecentShortInspection_returns_false_when_ShortInspection_InspectionDate_is_null()
    {
        new OfstedHeadlineGradesServiceModel(
                OfstedShortInspection.Unknown,
                new OfstedFullInspectionSummary(new DateTime(2021, 1, 1), OfstedRatingScore.Good),
                new OfstedFullInspectionSummary(new DateTime(2011, 1, 1), OfstedRatingScore.RequiresImprovement))
            .HasRecentShortInspection.Should().BeFalse();

        new OfstedHeadlineGradesServiceModel(
                OfstedShortInspection.Unknown,
                OfstedFullInspectionSummary.Unknown,
                OfstedFullInspectionSummary.Unknown)
            .HasRecentShortInspection.Should().BeFalse();
    }
}
