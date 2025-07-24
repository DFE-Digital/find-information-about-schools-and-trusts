using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Overview;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using NSubstitute.ReturnsExtensions;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Schools.Overview;

public class DetailsModelTests : BaseOverviewAreaModelTests<DetailsModel>
{
    private readonly ISchoolOverviewDetailsService _mockSchoolOverviewDetailsService =
        Substitute.For<ISchoolOverviewDetailsService>();

    private readonly IOtherServicesLinkBuilder _mockOtherServicesLinkBuilder =
        Substitute.For<IOtherServicesLinkBuilder>();

    private readonly SchoolOverviewServiceModel _dummySchoolDetails =
        new("Cool school", "some street, in a town", "Yorkshire", "Leeds", "Secondary", new AgeRange(11, 18),
            NurseryProvision.NotRecorded);

    public DetailsModelTests()
    {
        _mockSchoolOverviewDetailsService.GetSchoolOverviewDetailsAsync(Arg.Any<int>(), Arg.Any<SchoolCategory>())
            .Returns(_dummySchoolDetails);

        Sut = new DetailsModel(MockSchoolService, MockTrustService, _mockSchoolOverviewDetailsService,
            _mockOtherServicesLinkBuilder, MockDataSourceService, MockSchoolNavMenu)
        { Urn = SchoolUrn };
    }

    [Fact]
    public override async Task OnGetAsync_should_configure_PageMetadata_SubPageName()
    {
        await OnGetAsync_should_configure_PageMetadata_SubPageName_for_school();
        await OnGetAsync_should_configure_PageMetadata_SubPageName_for_academy();
    }

    private async Task OnGetAsync_should_configure_PageMetadata_SubPageName_for_school()
    {
        Sut.Urn = SchoolUrn;

        await Sut.OnGetAsync();

        Sut.PageMetadata.SubPageName.Should().Be("School details");
    }

    private async Task OnGetAsync_should_configure_PageMetadata_SubPageName_for_academy()
    {
        Sut.Urn = AcademyUrn;

        await Sut.OnGetAsync();

        Sut.PageMetadata.SubPageName.Should().Be("Academy details");
    }

    [Theory]
    [InlineData(SchoolCategory.LaMaintainedSchool, "School details")]
    [InlineData(SchoolCategory.Academy, "Academy details")]
    public void SubPageName_should_include_school_type(SchoolCategory schoolCategory, string expectedSubPageName)
    {
        DetailsModel.SubPageName(schoolCategory).Should().Be(expectedSubPageName);
    }

    [Fact]
    public void SubPageName_should_throw_for_unrecognised_school_type()
    {
        var act = () => DetailsModel.SubPageName((SchoolCategory)999);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task OnGetAsync_should_set_GetInformationAboutSchoolsLink()
    {
        _mockOtherServicesLinkBuilder.GetInformationAboutSchoolsListingLinkForSchool(SchoolUrn.ToString())
            .Returns("url");

        await Sut.OnGetAsync();

        Sut.GetInformationAboutSchoolsLink.Should().Be("url");
    }

    [Fact]
    public async Task OnGetAsync_should_set_FinancialBenchmarkingInsightsToolLink()
    {
        _mockOtherServicesLinkBuilder.FinancialBenchmarkingLinkForSchool(SchoolUrn).Returns("url");

        await Sut.OnGetAsync();

        Sut.FinancialBenchmarkingInsightsToolLink.Should().Be("url");
    }

    [Fact]
    public async Task OnGetAsync_should_set_FindSchoolPerformanceLink()
    {
        _mockOtherServicesLinkBuilder.FindSchoolPerformanceDataListingLink(SchoolUrn).Returns("url");

        await Sut.OnGetAsync();
        Sut.FindSchoolPerformanceLink.Should().Be("url");
    }

    [Fact]
    public async Task OnGetAsync_should_set_SchoolOverviewModel()
    {
        await Sut.OnGetAsync();

        Sut.SchoolOverviewModel.Should().Be(_dummySchoolDetails);
    }

    [Fact]
    public async Task OnGetAsync_should_set_TrustInformationIsAvailable_to_true_when_trust_information_is_available()
    {
        Sut.Urn = AcademyUrn;
        _mockSchoolOverviewDetailsService.GetSchoolOverviewDetailsAsync(AcademyUrn, SchoolCategory.Academy)
            .Returns(_dummySchoolDetails with { DateJoinedTrust = DateOnly.Parse("2025-01-01") });
        MockTrustService.GetTrustSummaryAsync(AcademyUrn)
            .Returns(new TrustSummaryServiceModel("1234", "Some Trust", "Some Type", 9001));

        await Sut.OnGetAsync();

        Sut.TrustInformationIsAvailable.Should().Be(true);
    }

    [Fact]
    public async Task OnGetAsync_should_set_TrustInformationIsAvailable_to_false_when_DateJoinedTrust_is_null()
    {
        Sut.Urn = AcademyUrn;
        _mockSchoolOverviewDetailsService.GetSchoolOverviewDetailsAsync(AcademyUrn, SchoolCategory.Academy)
            .Returns(_dummySchoolDetails with { DateJoinedTrust = null });
        MockTrustService.GetTrustSummaryAsync(AcademyUrn)
            .Returns(new TrustSummaryServiceModel("1234", "Some Trust", "Some Type", 9001));

        await Sut.OnGetAsync();

        Sut.TrustInformationIsAvailable.Should().Be(false);
    }

    [Fact]
    public async Task
        OnGetAsync_should_set_TrustInformationIsAvailable_to_false_when_GetTrustSummaryAsync_returns_null()
    {
        Sut.Urn = AcademyUrn;
        _mockSchoolOverviewDetailsService.GetSchoolOverviewDetailsAsync(AcademyUrn, SchoolCategory.Academy)
            .Returns(_dummySchoolDetails with { DateJoinedTrust = DateOnly.Parse("2025-01-01") });
        MockTrustService.GetTrustSummaryAsync(AcademyUrn)
            .ReturnsNull();

        await Sut.OnGetAsync();

        Sut.TrustInformationIsAvailable.Should().Be(false);
    }

    [Fact]
    public async Task OnGetAsync_should_set_TrustSummaryIsAvailable_to_true_when_trust_summary_is_available()
    {
        Sut.Urn = SchoolUrn;
        _mockSchoolOverviewDetailsService.GetSchoolOverviewDetailsAsync(SchoolUrn, SchoolCategory.LaMaintainedSchool)
            .Returns(_dummySchoolDetails);

        MockTrustService.GetTrustSummaryAsync(SchoolUrn)
            .Returns(new TrustSummaryServiceModel("1234", "Some Trust", "Some Type", 1));

        await Sut.OnGetAsync();

        Sut.TrustSummaryIsAvailable.Should().Be(true);
    }

    [Fact]
    public async Task OnGetAsync_should_set_TrustSummaryIsAvailable_to_false_when_GetTrustSummaryAsync_returns_null()
    {
        Sut.Urn = SchoolUrn;
        _mockSchoolOverviewDetailsService.GetSchoolOverviewDetailsAsync(SchoolUrn, SchoolCategory.LaMaintainedSchool)
            .Returns(_dummySchoolDetails);

        MockTrustService.GetTrustSummaryAsync(SchoolUrn)
            .ReturnsNull();

        await Sut.OnGetAsync();

        Sut.TrustInformationIsAvailable.Should().Be(false);
    }
}
