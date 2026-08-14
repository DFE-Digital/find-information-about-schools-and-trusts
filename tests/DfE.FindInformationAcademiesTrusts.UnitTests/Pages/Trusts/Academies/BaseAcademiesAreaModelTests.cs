using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Academies;
using DfE.FindInformationAcademiesTrusts.Services.Academy;
using DfE.FindInformationAcademiesTrusts.Services.Export;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Trusts.Academies;

public abstract class BaseAcademiesAreaModelTests<T> : BaseTrustPageTests<T>, ITestSubpages, ITestTabPages, ITestExport
    where T : AcademiesAreaModel
{
    protected readonly IAcademyService MockAcademyService = Substitute.For<IAcademyService>();

    protected readonly IPipelineAcademiesExportService MockPipelineAcademiesExportService =
        Substitute.For<IPipelineAcademiesExportService>();

    protected const string TrustReferenceNumber = TrustReference;

    public BaseAcademiesAreaModelTests()
    {
        //Set default GetAcademiesPipelineSummaryAsync to enable base tests with different UIDs
        MockAcademyService
            .GetAcademiesPipelineSummaryAsync(Arg.Any<string>())
            .Returns(Task.FromResult(new AcademyPipelineSummaryServiceModel(0, 0, 0)));

        MockAcademyService
            .GetAcademiesPipelineSummaryAsync(TrustReferenceNumber)
            .Returns(Task.FromResult(new AcademyPipelineSummaryServiceModel(1, 2, 3)));
    }

    [Fact]
    public override async Task OnGetAsync_should_configure_TrustPageMetadata_PageName()
    {
        _ = await Sut.OnGetAsync();

        Sut.PageMetadata.PageName.Should().Be("Academies");
    }

    [Fact]
    public abstract Task OnGetAsync_sets_academies_from_academyService();

    [Fact]
    public abstract Task OnGetAsync_should_configure_TrustPageMetadata_SubPageName();

    [Fact]
    public abstract Task OnGetAsync_should_set_active_TabList_to_current_tab();

    [Fact]
    public abstract Task OnGetAsync_should_populate_TabList_to_tabs();

    [Theory]
    [InlineData("1234", "TR1234")]
    [InlineData("5678", "TR5678")]
    public async Task OnGetAsync_should_populate_TabList_route_data_with_uid(string expectedUid,
        string expectedReferenceNumber)
    {
        MockTrustService.GetTrustSummaryAsync(expectedReferenceNumber)
            .Returns(DummyTrustSummary with { Uid = expectedUid, ReferenceNumber = expectedReferenceNumber });
        Sut.Uid = expectedUid;
        Sut.ReferenceNumber = expectedReferenceNumber;

        _ = await Sut.OnGetAsync();

        Sut.TabList.Should().AllSatisfy(link =>
        {
            link.AspAllRouteData.Should().BeEquivalentTo(new Dictionary<string, string>
            {
                { "uid", expectedUid },
                { "referenceNumber", expectedReferenceNumber }
            });
        });
    }

    [Fact]
    public async Task OnGetAsync_should_populate_TabList_hidden_text_to_academies()
    {
        _ = await Sut.OnGetAsync();

        Sut.TabList.Should().AllSatisfy(link => { link.VisuallyHiddenLinkText.Should().Be("Academies"); });
    }

    [Fact]
    public abstract Task OnGetAsync_should_configure_TrustPageMetadata_TabPageName();

    [Fact]
    public abstract Task OnGetExportAsync_ShouldReturnFileResult_WhenUidIsValid();

    [Fact]
    public abstract Task OnGetExportAsync_ShouldReturnNotFound_WhenUidIsInvalid();

    [Fact]
    public abstract Task OnGetExportAsync_ShouldSanitizeTrustName_WhenTrustNameContainsIllegalCharacters();
}
