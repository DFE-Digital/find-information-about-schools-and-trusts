using DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Models;
using DfE.FindInformationAcademiesTrusts.Configuration;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Domain.Common;
using DfE.FindInformationAcademiesTrusts.Domain.Enums;
using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using Microsoft.FeatureManagement;

namespace DfE.FindInformationAcademiesTrusts.Pages.Schools.Contacts;

public static class ContactsDataSources
{
    public static async Task<List<DataSourcePageListEntry>> GetAsync(IDataSourceService dataSourceService,
        IVariantFeatureManager featureManager, SchoolCategory schoolCategory, SchoolDfeContacts contacts)
    {
        var giasDataSource = await dataSourceService.GetAsync(Source.Gias);
        var inSchool = new DataSourcePageListEntry(InSchoolModel.SubPageName(schoolCategory),
            [new DataSourceListEntry(giasDataSource, "Head teacher name")]);

        if (!await featureManager.IsEnabledAsync(FeatureFlags.ContactsInDfeForSchools))
        {
            return [inSchool];
        }

        List<DataSourceListEntry> inDfe = schoolCategory switch
        {
            SchoolCategory.LaMaintainedSchool =>
            [
                new DataSourceListEntry(FiatDbDataSource(contacts.RegionsGroupLocalAuthorityLead),
                    "Regions group LA lead")
            ],
            SchoolCategory.Academy =>
            [
                new DataSourceListEntry(FiatDbDataSource(contacts.TrustRelationshipManager),
                    TrustContactRole.TrustRelationshipManager.MapRoleToViewString()),
                new DataSourceListEntry(FiatDbDataSource(contacts.SfsoLead),
                    TrustContactRole.SfsoLead.MapRoleToViewString())
            ],
            _ => throw new InvalidOperationException($"School category {schoolCategory} is not supported.")
        };

        return [new DataSourcePageListEntry(InDfeModel.SubPageName, inDfe), inSchool];
    }

    private static DataSourceServiceModel FiatDbDataSource(BaseEntity? contact) =>
        new(Source.FiatDb, contact?.LastModifiedAtTime, null, contact?.LastModifiedByEmail);
}
