using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Trusts.Governance;

public class GovernanceAreaModel(
    IDataSourceService dataSourceService,
    ITrustService trustService)
    : TrustsAreaModel(dataSourceService, trustService)
{
    public const string PageName = "Governance";

    public override PageMetadata PageMetadata => base.PageMetadata with { PageName = PageName };

    public TrustGovernanceServiceModel TrustGovernance { get; set; } = default!;

    public override async Task<IActionResult> OnGetAsync()
    {
        var pageResult = await base.OnGetAsync();

        if (pageResult.GetType() == typeof(NotFoundResult)) return pageResult;

        // TrustGovernance = await TrustService.GetTrustGovernanceAsync(Uid);
        TrustGovernance = await TrustService.GetTrustGovernancePersonsApiAsync(TrustReferenceNumber);

        // Add data sources
        var giasDataSource = await DataSourceService.GetAsync(Source.Gias);

        DataSourcesPerPage.AddRange([
            new DataSourcePageListEntry(TrustLeadershipModel.SubPageName, [new DataSourceListEntry(giasDataSource)]),
            new DataSourcePageListEntry(TrusteesModel.SubPageName, [new DataSourceListEntry(giasDataSource)]),
            new DataSourcePageListEntry(MembersModel.SubPageName, [new DataSourceListEntry(giasDataSource)]),
            new DataSourcePageListEntry(HistoricMembersModel.SubPageName, [new DataSourceListEntry(giasDataSource)])
        ]);

        return pageResult;
    }
}
