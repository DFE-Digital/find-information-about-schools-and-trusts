using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Domain.Entities;
using DfE.FindInformationAcademiesTrusts.Domain.Enums;
using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;

namespace DfE.FindInformationAcademiesTrusts.Pages.Trusts.Contacts;

public static class ContactsDataSources
{
    public static async Task<List<DataSourcePageListEntry>> GetAsync(IDataSourceService dataSourceService,
        TrustContact? trustRelationshipManager, TrustContact? sfsoLead)
    {
        var giasDataSource = await dataSourceService.GetAsync(Source.Gias);
        var mstrDataSource = await dataSourceService.GetAsync(Source.Mstr);

        return
        [
            new DataSourcePageListEntry(InDfeModel.SubPageName, [
                new DataSourceListEntry(FiatDbDataSource(trustRelationshipManager),
                    TrustContactRole.TrustRelationshipManager.MapRoleToViewString()),
                new DataSourceListEntry(FiatDbDataSource(sfsoLead), TrustContactRole.SfsoLead.MapRoleToViewString())
            ]),
            new DataSourcePageListEntry(InTrustModel.SubPageName, [
                new DataSourceListEntry(giasDataSource, "Accounting officer name"),
                new DataSourceListEntry(giasDataSource, "Chief financial officer name"),
                new DataSourceListEntry(giasDataSource, "Chair of trustees name"),
                new DataSourceListEntry(mstrDataSource, "Accounting officer email"),
                new DataSourceListEntry(mstrDataSource, "Chief financial officer email"),
                new DataSourceListEntry(mstrDataSource, "Chair of trustees email")
            ])
        ];
    }

    private static DataSourceServiceModel FiatDbDataSource(TrustContact? contact) =>
        new(Source.FiatDb, contact?.LastModifiedAtTime, null, contact?.LastModifiedByEmail);
}
