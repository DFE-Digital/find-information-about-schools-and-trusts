using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Ofsted.Older;
using DfE.FindInformationAcademiesTrusts.Services.Academy;
using DfE.FindInformationAcademiesTrusts.Services.Trust;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Schools.Ofsted;

public class CurrentRatingsModelTests : BaseOfstedAreaModelTests<CurrentRatingsModel>
{
    private readonly SchoolOfstedServiceModel _dummySchoolOfstedServiceModel = new(
        SchoolUrn.ToString(),
        null,
        null,
        OfstedShortInspection.Unknown,
        OfstedRating.Unknown,
        OfstedRating.Unknown,
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
                MockOfstedSchoolDataExportService,
                MockDateTimeProvider,
                MockOtherServicesLinkBuilder,
                MockSchoolNavMenu)
            { Urn = SchoolUrn };

        MockSchoolService.GetSchoolOfstedRatingsAsBeforeAndAfterSeptemberGradeAsync(Arg.Any<int>()).Returns(_dummySchoolOfstedServiceModel);
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

        MockSchoolService
            .GetSchoolOfstedRatingsAsBeforeAndAfterSeptemberGradeAsync(SchoolUrn)
            .Returns(_dummySchoolOfstedServiceModel with { CurrentOfstedRating = expectedRating });

        await Sut.OnGetAsync();

        Sut.OfstedRating.Should().Be(expectedRating);
    }

    [Fact]
    public async Task OnGetAsync_should_set_InspectionBeforeOrAfterJoiningTrust_to_not_applicable_when_school_is_la()
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

        MockSchoolService
            .GetSchoolOfstedRatingsAsBeforeAndAfterSeptemberGradeAsync(SchoolUrn)
            .Returns(_dummySchoolOfstedServiceModel with { CurrentOfstedRating = expectedRating });

        await Sut.OnGetAsync();

        Sut.InspectionBeforeOrAfterJoiningTrust.Should().Be(BeforeOrAfterJoining.NotApplicable);
    }

    [Fact]
    public async Task
        OnGetAsync_should_set_InspectionBeforeOrAfterJoiningTrust_to_after_when_school_is_academy_and_previous_inspection_date_is_after_date_joined_trust()
    {
        Sut.Urn = AcademyUrn;

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

        MockSchoolService
            .GetSchoolOfstedRatingsAsBeforeAndAfterSeptemberGradeAsync(AcademyUrn)
            .Returns(_dummySchoolOfstedServiceModel with
            {
                DateAcademyJoinedTrust = new DateTime(2024, 1, 1),
                CurrentOfstedRating = expectedRating
            });

        await Sut.OnGetAsync();

        Sut.InspectionBeforeOrAfterJoiningTrust.Should().Be(BeforeOrAfterJoining.After);
    }

    [Fact]
    public async Task
        OnGetAsync_should_set_InspectionBeforeJoiningTrust_to_before_when_school_is_academy_and_previous_inspection_date_is_before_date_joined_trust()
    {
        Sut.Urn = AcademyUrn;

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

        MockSchoolService
            .GetSchoolOfstedRatingsAsBeforeAndAfterSeptemberGradeAsync(AcademyUrn)
            .Returns(_dummySchoolOfstedServiceModel with
            {
                DateAcademyJoinedTrust = new DateTime(2026, 1, 1),
                CurrentOfstedRating = expectedRating
            });

        await Sut.OnGetAsync();

        Sut.InspectionBeforeOrAfterJoiningTrust.Should().Be(BeforeOrAfterJoining.Before);
    }

    [Fact]
    public override async Task OnGetAsync_should_populate_TabList_to_tabs()
    {
        _ = await Sut.OnGetAsync();

        Sut.TabList.Should()
            .SatisfyRespectively(
                l =>
                {
                    l.LinkDisplayText.Should().Be("After September 2024");
                    l.AspPage.Should().Be("./CurrentRatings");
                    l.TestId.Should().Be("older-after-september-2024-tab");
                },
                l =>
                {
                    l.LinkDisplayText.Should().Be("Before September 2024");
                    l.AspPage.Should().Be("./PreviousRatings");
                    l.TestId.Should().Be("older-before-september-2024-tab");
                });
    }

}
