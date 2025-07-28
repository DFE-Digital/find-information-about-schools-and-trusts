using DfE.FindInformationAcademiesTrusts.Data;
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
}
