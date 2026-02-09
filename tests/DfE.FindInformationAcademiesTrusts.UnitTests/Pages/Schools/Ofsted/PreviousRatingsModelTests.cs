using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Ofsted.Older;
using DfE.FindInformationAcademiesTrusts.Services.Academy;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using NSubstitute;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Schools.Ofsted;

public class PreviousRatingsModelTests : BaseOfstedAreaModelTests<PreviousRatingsModel>
{
    private readonly OlderSchoolOfstedServiceModel _dummySchoolOfstedServiceModel = new(
        SchoolUrn.ToString(),
        null,
        null,
        OfstedShortInspection.Unknown,
        [],
        [],
        false
    );

    private readonly TrustSummaryServiceModel _dummyTrustSummaryServiceModel = new(
        "1234",
        "Some Trust",
        "Some Trust Type",
        23
    );

    public PreviousRatingsModelTests()
    {
        Sut = new PreviousRatingsModel(
                MockSchoolService,
                MockSchoolOverviewDetailsService,
                MockTrustService,
                MockDataSourceService,
                MockOfstedSchoolDataExportService,
                MockDateTimeProvider,
                MockOtherServicesLinkBuilder,
                MockSchoolNavMenu,
                MockOfstedService)
            { Urn = SchoolUrn };

        MockOfstedService.GetSchoolOfstedRatingsAsBeforeAndAfterSeptemberGradeAsync(Arg.Any<int>()).Returns(_dummySchoolOfstedServiceModel);
        MockTrustService.GetTrustSummaryAsync(AcademyUrn).Returns(_dummyTrustSummaryServiceModel);
    }

    [Fact]
    public override async Task OnGetAsync_should_configure_PageMetadata_SubPageName()
    {
        await Sut.OnGetAsync();

        Sut.PageMetadata.SubPageName.Should().Be("Older inspections (before November 2025)");
        Sut.PageMetadata.TabName.Should().Be("Before September 2024");
    }

    [Fact]
    public async Task OnGetAsync_should_set_correct_OfstedRating_data()
    {
        var expectedRating = new OfstedRating(
            OfstedRatingScore.Good,
            OfstedRatingScore.Good,
            OfstedRatingScore.Good,
            OfstedRatingScore.Good,
            OfstedRatingScore.Good,
            OfstedRatingScore.Good,
            OfstedRatingScore.Good,
            CategoriesOfConcern.NoConcerns,
            SafeguardingScore.Yes,
            new DateTime(2025, 1, 1)
        );

        _dummySchoolOfstedServiceModel.RatingsWithSingleHeadlineGrade.Add(expectedRating);

        MockOfstedService
            .GetSchoolOfstedRatingsAsBeforeAndAfterSeptemberGradeAsync(SchoolUrn)
            .Returns(_dummySchoolOfstedServiceModel);

        await Sut.OnGetAsync();

        Sut.OfstedRatings.Should().BeEquivalentTo([expectedRating]);
    }

    [Fact]
    public override async Task OnGetAsync_should_call_populate_tablist()
    {
        _ = await Sut.OnGetAsync();

        _ = MockSchoolNavMenu.Received(1).GetTabLinksForOlderOfstedPages(Arg.Any<OlderBaseRatingsModel>());
    }
}
