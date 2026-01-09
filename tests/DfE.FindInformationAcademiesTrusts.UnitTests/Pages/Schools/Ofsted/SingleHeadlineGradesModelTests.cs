using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Ofsted;
using DfE.FindInformationAcademiesTrusts.Services.School;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Schools.Ofsted;

public class SingleHeadlineGradesModelTests : BaseOfstedAreaModelTests<SingleHeadlineGradesModel>
{
    public SingleHeadlineGradesModelTests()
    {
        Sut = new SingleHeadlineGradesModel(MockSchoolService, MockSchoolOverviewDetailsService, MockTrustService,
                MockDataSourceService, MockOfstedSchoolDataExportService, MockDateTimeProvider,
                MockOtherServicesLinkBuilder, MockSchoolNavMenu)
            { Urn = SchoolUrn };
    }

    [Fact]
    public override async Task OnGetAsync_should_configure_PageMetadata_SubPageName()
    {
        await Sut.OnGetAsync();

        Sut.PageMetadata.SubPageName.Should().Be("Single headline grades");
    }

    [Fact]
    public async Task OnGetAsync_should_set_correct_HeadlineGrades_data()
    {
        var expectedGrades = new OfstedHeadlineGradesServiceModel(
            new OfstedShortInspection(
                DateTime.Parse("2025-04-03"),
                "School remains Outstanding"),
            new OfstedFullInspectionSummary(
                DateTime.Parse("2023-04-03"),
                OfstedRatingScore.Outstanding),
            new OfstedFullInspectionSummary(
                DateTime.Parse("2011-04-03"),
                OfstedRatingScore.Good));

        MockSchoolService.GetOfstedHeadlineGrades(SchoolUrn).Returns(expectedGrades);

        await Sut.OnGetAsync();

        Sut.HeadlineGrades.Should().Be(expectedGrades);
    }

    [Fact]
    public void GetBeforeOrAfterJoining_returns_NotApplicable_when_DateJoinedTrust_is_null()
    {
        Sut.DateJoinedTrust = null;
        var inspectionDate = DateTime.Parse("2020-01-01");

        var result = Sut.GetBeforeOrAfterJoining(inspectionDate);

        result.Should().Be(BeforeOrAfterJoining.NotApplicable);
    }

    [Fact]
    public void GetBeforeOrAfterJoining_returns_NotApplicable_when_inspectionDate_is_null()
    {
        Sut.DateJoinedTrust = DateTime.Parse("2015-01-01");

        var result = Sut.GetBeforeOrAfterJoining(null);

        result.Should().Be(BeforeOrAfterJoining.NotApplicable);
    }

    [Fact]
    public void GetBeforeOrAfterJoining_returns_Before_when_inspection_is_before_joining()
    {
        Sut.DateJoinedTrust = DateTime.Parse("2015-01-01");
        var inspectionDate = DateTime.Parse("2014-06-15");

        var result = Sut.GetBeforeOrAfterJoining(inspectionDate);

        result.Should().Be(BeforeOrAfterJoining.Before);
    }

    [Fact]
    public void GetBeforeOrAfterJoining_returns_After_when_inspection_is_after_joining()
    {
        Sut.DateJoinedTrust = DateTime.Parse("2015-01-01");
        var inspectionDate = DateTime.Parse("2016-06-15");

        var result = Sut.GetBeforeOrAfterJoining(inspectionDate);

        result.Should().Be(BeforeOrAfterJoining.After);
    }

    [Fact]
    public void GetBeforeOrAfterJoining_returns_After_when_inspection_is_on_same_day_as_joining()
    {
        Sut.DateJoinedTrust = DateTime.Parse("2015-01-01");
        var inspectionDate = DateTime.Parse("2015-01-01");

        var result = Sut.GetBeforeOrAfterJoining(inspectionDate);

        result.Should().Be(BeforeOrAfterJoining.After);
    }

    [Fact]
    public void GetScreenReaderText_returns_empty_string_when_DateJoinedTrust_is_null()
    {
        Sut.DateJoinedTrust = null;
        var inspectionDate = DateTime.Parse("2020-01-01");

        var result = Sut.GetScreenReaderText(inspectionDate);

        result.Should().BeEmpty();
    }

    [Fact]
    public void GetScreenReaderText_returns_empty_string_when_inspectionDate_is_null()
    {
        Sut.DateJoinedTrust = DateTime.Parse("2015-01-01");

        var result = Sut.GetScreenReaderText(null);

        result.Should().BeEmpty();
    }

    [Fact]
    public void GetScreenReaderText_returns_before_text_when_inspection_is_before_joining()
    {
        Sut.DateJoinedTrust = DateTime.Parse("2015-01-01");
        var inspectionDate = DateTime.Parse("2014-06-15");

        var result = Sut.GetScreenReaderText(inspectionDate);

        result.Should().Be("Inspected before joining the trust");
    }

    [Fact]
    public void GetScreenReaderText_returns_after_text_when_inspection_is_after_joining()
    {
        Sut.DateJoinedTrust = DateTime.Parse("2015-01-01");
        var inspectionDate = DateTime.Parse("2016-06-15");

        var result = Sut.GetScreenReaderText(inspectionDate);

        result.Should().Be("Inspected after joining the trust");
    }
}
