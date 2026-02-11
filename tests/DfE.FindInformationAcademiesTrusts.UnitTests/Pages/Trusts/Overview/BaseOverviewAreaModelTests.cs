using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Overview;
using DfE.FindInformationAcademiesTrusts.Services.Trust;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Trusts.Overview;

public abstract class BaseOverviewAreaModelTests<T> : BaseTrustPageTests<T>, ITestSubpages where T : OverviewAreaModel
{
    protected readonly TrustOverviewServiceModel BaseTrustOverviewServiceModel =
        new(TrustUid, "", "", "", TrustType.MultiAcademyTrust, "", "", null, null, 0, new Dictionary<string, int>(), 0,
            0, true);

    protected BaseOverviewAreaModelTests()
    {
        MockTrustService.GetTrustOverviewAsync(Arg.Any<string>())
            .Returns(callinfo => BaseTrustOverviewServiceModel with { Uid = callinfo.Arg<string>() });
    }

    protected void SetupTrustOverview(TrustOverviewServiceModel trustOverviewServiceModel)
    {
        MockTrustService.GetTrustOverviewAsync(trustOverviewServiceModel.Uid)
            .Returns(Task.FromResult(trustOverviewServiceModel));
    }

    [Fact]
    public override async Task OnGetAsync_sets_correct_data_source_list()
    {
        _ = await Sut.OnGetAsync();
        await MockDataSourceService.Received(1).GetAsync(Source.Gias);

        Sut.DataSourcesPerPage.Should().BeEquivalentTo([
            new DataSourcePageListEntry("Trust details", [new DataSourceListEntry(GiasDataSource)]),
            new DataSourcePageListEntry("Trust summary", [new DataSourceListEntry(CompareDataSource, "Pupil numbers"), new DataSourceListEntry(GiasDataSource, "All other information was")]),
            new DataSourcePageListEntry("Reference numbers", [new DataSourceListEntry(GiasDataSource)])
        ]);
    }

    [Fact]
    public override async Task OnGetAsync_should_configure_TrustPageMetadata_PageName()
    {
        _ = await Sut.OnGetAsync();

        Sut.PageMetadata.PageName.Should().Be("Overview");
    }

    [Fact]
    public abstract Task OnGetAsync_should_configure_TrustPageMetadata_SubPageName();

}
