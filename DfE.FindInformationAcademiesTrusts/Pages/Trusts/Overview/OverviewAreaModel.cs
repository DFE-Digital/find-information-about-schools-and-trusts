using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Trusts.Overview;

public class OverviewAreaModel(
    IDataSourceService dataSourceService,
    ITrustService trustService)
    : TrustsAreaModel(dataSourceService, trustService)
{
    public const string PageName = "Overview";

    public override PageMetadata PageMetadata => base.PageMetadata with { PageName = PageName };

    public TrustOverviewServiceModel TrustOverview { get; set; } = default!;

    public override async Task<IActionResult> OnGetAsync()
    {
        var pageResult = await base.OnGetAsync();
        if (pageResult is NotFoundResult) return pageResult;

        // Fetch the trust overview data
        TrustOverview = await TrustService.GetTrustOverviewAsync(Uid);

        // Add data sources
        var giasDataSource = await DataSourceService.GetAsync(Source.Gias);
        var compareDataSource = await DataSourceService.GetAsync(Source.CompareSchoolCollegePerformanceEnglandPopulation);
        DataSourcesPerPage.AddRange([
            new DataSourcePageListEntry(TrustDetailsModel.SubPageName, [new DataSourceListEntry(giasDataSource)]),
            new DataSourcePageListEntry(TrustSummaryModel.SubPageName, [new DataSourceListEntry(compareDataSource, "Pupil numbers"), new DataSourceListEntry(giasDataSource, "All other information was")]),
            new DataSourcePageListEntry(ReferenceNumbersModel.SubPageName, [new DataSourceListEntry(giasDataSource)])
        ]);

        return Page();
    }
}
