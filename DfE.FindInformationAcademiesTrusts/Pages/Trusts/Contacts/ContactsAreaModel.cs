using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Trusts.Contacts;

public class ContactsAreaModel(
    IDataSourceService dataSourceService,
    ITrustService trustService
)
    : TrustsAreaModel(dataSourceService, trustService)
{
    public const string PageName = "Contacts";

    public override PageMetadata PageMetadata => base.PageMetadata with { PageName = PageName };

    public Person? ChairOfTrustees { get; set; }
    public Person? AccountingOfficer { get; set; }
    public Person? ChiefFinancialOfficer { get; set; }
    public InternalContact? SfsoLead { get; set; }
    public InternalContact? TrustRelationshipManager { get; set; }

    public override async Task<IActionResult> OnGetAsync()
    {
        var pageResult = await base.OnGetAsync();

        if (pageResult.GetType() == typeof(NotFoundResult)) return pageResult;

        (TrustRelationshipManager, SfsoLead, AccountingOfficer, ChairOfTrustees, ChiefFinancialOfficer) =
            await TrustService.GetTrustContactsAsync(Uid, TrustReferenceNumber);

        // Add data sources
        var giasDataSource = await DataSourceService.GetAsync(Source.Gias);
        var mstrDataSource = await DataSourceService.GetAsync(Source.Mstr);

        DataSourcesPerPage.AddRange([
            new DataSourcePageListEntry(InDfeModel.SubPageName, [
                    new DataSourceListEntry(new DataSourceServiceModel(Source.FiatDb,
                            TrustRelationshipManager?.LastModifiedAtTime, null,
                            TrustRelationshipManager?.LastModifiedByEmail),
                        TrustContactRole.TrustRelationshipManager.MapRoleToViewString()),
                    new DataSourceListEntry(new DataSourceServiceModel(Source.FiatDb, SfsoLead?.LastModifiedAtTime,
                        null,
                        SfsoLead?.LastModifiedByEmail), TrustContactRole.SfsoLead.MapRoleToViewString())
                ]
            ),
            new DataSourcePageListEntry(InTrustModel.SubPageName, [
                    new DataSourceListEntry(giasDataSource, "Accounting officer name"),
                    new DataSourceListEntry(giasDataSource, "Chief financial officer name"),
                    new DataSourceListEntry(giasDataSource, "Chair of trustees name"),
                    new DataSourceListEntry(mstrDataSource, "Accounting officer email"),
                    new DataSourceListEntry(mstrDataSource, "Chief financial officer email"),
                    new DataSourceListEntry(mstrDataSource, "Chair of trustees email")
                ]
            )
        ]);

        return pageResult;
    }
}
