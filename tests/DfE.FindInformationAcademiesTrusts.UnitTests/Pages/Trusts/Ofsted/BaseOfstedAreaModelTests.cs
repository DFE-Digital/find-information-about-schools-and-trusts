using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Ofsted;
using DfE.FindInformationAcademiesTrusts.Services.Ofsted;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Trusts.Ofsted;

public abstract class BaseOfstedAreaModelTests<T> : BaseTrustPageTests<T>, ITestSubpages
    where T : OfstedAreaModel
{

    protected readonly IOfstedService MockOfstedService = Substitute.For<IOfstedService>();

    protected readonly IPowerBiLinkBuilderService MockPowerBiLinkBuilderService = Substitute.For<IPowerBiLinkBuilderService>();

    [Fact]
    public override async Task OnGetAsync_sets_correct_data_source_list()
    {
        await Sut.OnGetAsync();

        await MockDataSourceService.Received(1).GetAsync(Source.Gias);
        await MockDataSourceService.Received(1).GetAsync(Source.Mis);

        Sut.DataSourcesPerPage.Should().BeEquivalentTo([
            new DataSourcePageListEntry("Overview", [
                    new DataSourceListEntry(GiasDataSource, "Date joined trust"),
                    new DataSourceListEntry(MisDataSource, "All inspection types"),
                    new DataSourceListEntry(MisDataSource, "All inspection dates")
                ]
            ),
            new DataSourcePageListEntry("Report cards", [
                    new DataSourceListEntry(MisDataSource, "Current report card ratings"),
                    new DataSourceListEntry(MisDataSource, "Previous report card ratings")
                ]
            ),
            new DataSourcePageListEntry("Older inspections (before November 2025)", [
                    new DataSourceListEntry(MisDataSource, "Inspection ratings after September 24"),
                    new DataSourceListEntry(MisDataSource, "Inspection ratings before September 24")
                ]
            ),
            new DataSourcePageListEntry(SafeguardingAndConcernsModel.SubPageName, [
                    new DataSourceListEntry(MisDataSource, "Effective safeguarding and category of concern")
                ]
            )
        ]);
    }


    [Fact]
    public override async Task OnGetAsync_should_configure_TrustPageMetadata_PageName()
    {
        _ = await Sut.OnGetAsync();

        Sut.PageMetadata.PageName.Should().Be("Ofsted");
    }

    [Fact]
    public abstract Task OnGetAsync_should_configure_TrustPageMetadata_SubPageName();


    [Fact]
    public abstract Task OnGetAsync_ShouldSetPowerBiLinkUrl();
}
