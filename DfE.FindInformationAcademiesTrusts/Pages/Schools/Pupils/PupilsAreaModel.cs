using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;
using Source = DfE.FindInformationAcademiesTrusts.Data.Enums.Source;

namespace DfE.FindInformationAcademiesTrusts.Pages.Schools.Pupils;

public class PupilsAreaModel(
    IDataSourceService dataSourceService,
    ISchoolService schoolService,
    ITrustService trustService,
    ISchoolNavMenu schoolNavMenu
) : SchoolAreaModel(schoolService, trustService, schoolNavMenu)
{
    public const string PageName = "Pupils";
    public override PageMetadata PageMetadata => base.PageMetadata with { PageName = PageName };

    public override async Task<IActionResult> OnGetAsync()
    {
        var pageResult = await base.OnGetAsync();
        if (pageResult is NotFoundResult) return pageResult;

        var compareDataSource = await dataSourceService.GetAsync(Source.CompareSchoolCollegePerformanceEngland);

        DataSourcesPerPage.AddRange([
            new DataSourcePageListEntry(PopulationModel.SubPageName, [new DataSourceListEntry(compareDataSource)]),
        ]);

        return Page();
    }
}
