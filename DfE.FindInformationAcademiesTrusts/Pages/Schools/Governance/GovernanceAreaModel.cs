using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Governance;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Schools.Governance;

public class GovernanceAreaModel(
    ISchoolService schoolService,
    ITrustService trustService,
    IDataSourceService dataSourceService,
    ISchoolNavMenu schoolNavMenu)
    : SchoolAreaModel(schoolService, trustService, schoolNavMenu)
{
    private readonly ISchoolService _schoolService = schoolService;

    public const string PageName = "Governance";

    public SchoolGovernanceServiceModel SchoolGovernance { get; set; } = null!;

    public override PageMetadata PageMetadata => base.PageMetadata with { PageName = PageName };

    public override async Task<IActionResult> OnGetAsync()
    {
        SchoolGovernance = await _schoolService.GetSchoolGovernanceAsync(Urn);

        var pageResult = await base.OnGetAsync();

        if (pageResult.GetType() == typeof(NotFoundResult)) return pageResult;

        // Add data sources  
        var giasDataSource = await dataSourceService.GetAsync(Source.Gias);

        DataSourcesPerPage.AddRange([
            new DataSourcePageListEntry(TrustLeadershipModel.SubPageName, [new DataSourceListEntry(giasDataSource)]),
            new DataSourcePageListEntry(TrusteesModel.SubPageName, [new DataSourceListEntry(giasDataSource)]),
            new DataSourcePageListEntry(MembersModel.SubPageName, [new DataSourceListEntry(giasDataSource)]),
            new DataSourcePageListEntry(HistoricMembersModel.SubPageName, [new DataSourceListEntry(giasDataSource)])
        ]);

        return pageResult;
    }
}
