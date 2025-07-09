using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Schools.Overview;

public abstract class OverviewAreaModel(
    ISchoolService schoolService,
    ITrustService trustService,
    IDataSourceService dataSourceService,
    ISchoolNavMenu schoolNavMenu) : SchoolAreaModel(schoolService, trustService, schoolNavMenu)
{
    public const string PageName = "Overview";
    public override PageMetadata PageMetadata => base.PageMetadata with { PageName = PageName };

    public override async Task<IActionResult> OnGetAsync()
    {
        var pageResult = await base.OnGetAsync();
        if (pageResult is NotFoundResult) return pageResult;

        // Add data sources
        var giasDataSource = await dataSourceService.GetAsync(Source.Gias);

        DataSourcesPerPage =
        [
            new DataSourcePageListEntry(DetailsModel.SubPageName(SchoolCategory),
                [new DataSourceListEntry(giasDataSource)]),
            new DataSourcePageListEntry(FederationModel.SubPageName,
                [new DataSourceListEntry(giasDataSource)]),
            new DataSourcePageListEntry(ReferenceNumbersModel.SubPageName,
                [new DataSourceListEntry(giasDataSource)]),
            new DataSourcePageListEntry(SenModel.SubPageName,
                [new DataSourceListEntry(giasDataSource)])
        ];

        return Page();
    }
}
