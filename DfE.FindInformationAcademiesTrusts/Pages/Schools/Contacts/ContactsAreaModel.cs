using DfE.FindInformationAcademiesTrusts.Configuration;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;

namespace DfE.FindInformationAcademiesTrusts.Pages.Schools.Contacts;

public class ContactsAreaModel(
    ISchoolService schoolService,
    ITrustService trustService,
    IDataSourceService dataSourceService,
    ISchoolNavMenu schoolNavMenu,
    IVariantFeatureManager featureManager)
    : SchoolAreaModel(schoolService, trustService, schoolNavMenu)
{
    private readonly ITrustService _trustService = trustService;

    public const string PageName = "Contacts";

    public override PageMetadata PageMetadata => base.PageMetadata with { PageName = PageName };

    public override async Task<IActionResult> OnGetAsync()
    {
        var pageResult = await base.OnGetAsync();

        if (pageResult.GetType() == typeof(NotFoundResult)) return pageResult;

        var giasDataSource = await dataSourceService.GetAsync(Source.Gias);

        DataSourcesPerPage =
        [
            ..await GetInDfeDataSources(),
            new DataSourcePageListEntry(InSchoolModel.SubPageName(SchoolCategory),
                [new DataSourceListEntry(giasDataSource, "Head teacher name")])
        ];

        return Page();
    }

    private async Task<DataSourcePageListEntry[]> GetInDfeDataSources()
    {
        if (!await ContactsInDfeForSchoolsEnabled()) return [];

        return SchoolSummary.Category switch
        {
            SchoolCategory.LaMaintainedSchool => await GetInDfeDataSourcesForLocalAuthorityMaintainedSchool(),
            SchoolCategory.Academy => await GetInDfeDataSourcesForAcademy(),
            _ => throw new InvalidOperationException($"School category {SchoolSummary.Category} is not supported.")
        };
    }

    private async Task<DataSourcePageListEntry[]> GetInDfeDataSourcesForLocalAuthorityMaintainedSchool()
    {
        var dataSource = await dataSourceService.GetSchoolContactDataSourceAsync(Urn, SchoolContactRole.RegionsGroupLocalAuthorityLead);
        return
        [
            new DataSourcePageListEntry(InDfeModel.SubPageName,
                [new DataSourceListEntry(dataSource, "Regions group LA lead")])
        ];
    }

    private async Task<DataSourcePageListEntry[]> GetInDfeDataSourcesForAcademy()
    {
        var uid = (await _trustService.GetTrustSummaryAsync(Urn))?.Uid;

        if (uid is null || !int.TryParse(uid, out var uidAsInt)) return [];

        var trustRelationshipManagerDataSource =
            await dataSourceService.GetTrustContactDataSourceAsync(uidAsInt, TrustContactRole.TrustRelationshipManager);
        var sfsoLeadDataSource = await dataSourceService.GetTrustContactDataSourceAsync(uidAsInt, TrustContactRole.SfsoLead);

        return
        [
            new DataSourcePageListEntry(InDfeModel.SubPageName,
            [
                new DataSourceListEntry(trustRelationshipManagerDataSource,
                    TrustContactRole.TrustRelationshipManager.MapRoleToViewString()),
                new DataSourceListEntry(sfsoLeadDataSource, TrustContactRole.SfsoLead.MapRoleToViewString())
            ])
        ];
    }

    private async Task<bool> ContactsInDfeForSchoolsEnabled()
    {
        return await featureManager.IsEnabledAsync(FeatureFlags.ContactsInDfeForSchools);
    }
}
