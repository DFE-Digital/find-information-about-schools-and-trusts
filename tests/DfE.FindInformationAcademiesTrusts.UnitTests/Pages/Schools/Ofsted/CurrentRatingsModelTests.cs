using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Ofsted.Older;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Ofsted.ReportCards;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Schools.Ofsted;

public class CurrentRatingsModelTests : BaseOfstedAreaModelTests<CurrentRatingsModel>
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

    public CurrentRatingsModelTests()
    {
        Sut = new CurrentRatingsModel(
                MockSchoolService,
                MockSchoolOverviewDetailsService,
                MockTrustService,
                MockDataSourceService,
                MockOtherServicesLinkBuilder,
                MockSchoolNavMenu,
                MockOfstedService,
                MockPowerBiLinkBuilderService)
            { Urn = SchoolUrn };

        MockOfstedService.GetSchoolOfstedRatingsAsBeforeAndAfterSeptemberGradeAsync(Arg.Any<int>()).Returns(_dummySchoolOfstedServiceModel);
        MockTrustService.GetTrustSummaryAsync(AcademyUrn).Returns(_dummyTrustSummaryServiceModel);
    }

    [Fact]
    public override async Task OnGetAsync_should_configure_PageMetadata_SubPageName()
    {
        await Sut.OnGetAsync();

        Sut.PageMetadata.SubPageName.Should().Be("Older inspections (before November 2025)");
        Sut.PageMetadata.TabName.Should().Be("After September 2024");
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

        _dummySchoolOfstedServiceModel.RatingsWithoutSingleHeadlineGrade.Add(expectedRating);

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

        _ = MockSchoolNavMenu.Received(1).GetTabLinksForOlderOfstedPages(Arg.Any<CurrentRatingsModel>());
    }

    [Fact]
    public override async Task OnGetAsync_ShouldSetPowerBiLinkUrl()
    {
        Sut.PowerBiLink.Should().BeNullOrEmpty();

        _ = await Sut.OnGetAsync();

        Sut.PowerBiLink.Should().Be("https://powerbi.com/ofstedpublished");
        MockPowerBiLinkBuilderService.Received(1).BuildOfstedPublishedLink(Sut.Urn);
    }

}
