using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Overview;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Schools.Overview;

public abstract class BaseOverviewAreaModelTests<T> : BaseSchoolPageTests<T> where T : OverviewAreaModel
{
    [Fact]
    public override async Task OnGetAsync_should_configure_PageMetadata_PageName()
    {
        await Sut.OnGetAsync();

        Sut.PageMetadata.PageName.Should().Be("Overview");
    }

    [Fact]
    public override async Task OnGetAsync_sets_correct_data_source_list()
    {
        await OnGetAsync_sets_correct_data_source_list_for_academy();

        MockDataSourceService.ClearReceivedCalls();

        await OnGetAsync_sets_correct_data_source_list_for_school();
    }

    private async Task OnGetAsync_sets_correct_data_source_list_for_academy()
    {
        Sut.Urn = AcademyUrn;

        _ = await Sut.OnGetAsync();
        await MockDataSourceService.Received(1).GetAsync(Source.Gias);

        Sut.DataSourcesPerPage.Should().BeEquivalentTo([
            new DataSourcePageListEntry("Academy details", [
                new DataSourceListEntry(Mocks.MockDataSourceService.Gias)
            ]),
            new DataSourcePageListEntry("Federation details",
                [new DataSourceListEntry(Mocks.MockDataSourceService.Gias)]),
            new DataSourcePageListEntry("Reference numbers",
                [new DataSourceListEntry(Mocks.MockDataSourceService.Gias)]),
            new DataSourcePageListEntry("SEN (special educational needs)",
                [new DataSourceListEntry(Mocks.MockDataSourceService.Gias)])
        ]);
    }

    private async Task OnGetAsync_sets_correct_data_source_list_for_school()
    {
        Sut.Urn = SchoolUrn;

        _ = await Sut.OnGetAsync();
        await MockDataSourceService.Received(1).GetAsync(Source.Gias);

        Sut.DataSourcesPerPage.Should().BeEquivalentTo([
            new DataSourcePageListEntry("School details", [
                new DataSourceListEntry(Mocks.MockDataSourceService.Gias)
            ]),
            new DataSourcePageListEntry("Federation details",
                [new DataSourceListEntry(Mocks.MockDataSourceService.Gias)]),
            new DataSourcePageListEntry("Reference numbers",
                [new DataSourceListEntry(Mocks.MockDataSourceService.Gias)]),
            new DataSourcePageListEntry("SEN (special educational needs)",
                [new DataSourceListEntry(Mocks.MockDataSourceService.Gias)])
        ]);
    }
}
