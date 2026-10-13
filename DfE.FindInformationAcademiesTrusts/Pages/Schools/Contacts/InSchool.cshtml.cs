using DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Queries;
using DfE.FindInformationAcademiesTrusts.Configuration;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.School;
using DfE.FindInformationAcademiesTrusts.Domain.Enums;
using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;

namespace DfE.FindInformationAcademiesTrusts.Pages.Schools.Contacts;

public class InSchoolModel(
    IMediator mediator,
    ISchoolRepository schoolRepository,
    IDataSourceService dataSourceService,
    IVariantFeatureManager featureManager,
    ISchoolService schoolService,
    ITrustService trustService,
    ISchoolNavMenu schoolNavMenu)
    : SchoolAreaModel(schoolService, trustService, schoolNavMenu)
{
    public override PageMetadata PageMetadata => base.PageMetadata with
    {
        PageName = InDfeModel.PageName,
        SubPageName = FullSubPageName(SchoolCategory)
    };

    public static string FullSubPageName(SchoolCategory schoolCategory)
    {
        return schoolCategory switch
        {
            SchoolCategory.LaMaintainedSchool => "Contacts in this school",
            SchoolCategory.Academy => "Contacts in this academy",
            _ => throw new ArgumentOutOfRangeException(nameof(schoolCategory))
        };
    }

    public static string SubPageName(SchoolCategory schoolCategory)
    {
        return schoolCategory switch
        {
            SchoolCategory.LaMaintainedSchool => "In this school",
            SchoolCategory.Academy => "In this academy",
            _ => throw new ArgumentOutOfRangeException(nameof(schoolCategory))
        };
    }

    public SchoolContact HeadTeacher { get; private set; } = null!;

    public override async Task<IActionResult> OnGetAsync()
    {
        var pageResult = await base.OnGetAsync();
        if (pageResult is NotFoundResult) return pageResult;

        var headTeacher = await schoolRepository.GetSchoolContactsAsync(Urn);
        if (headTeacher is null) return NotFound();

        HeadTeacher = headTeacher;

        var giasDataSource = await dataSourceService.GetAsync(Source.Gias);
        var inSchoolDataSources = new DataSourcePageListEntry(SubPageName(SchoolCategory),
            [new DataSourceListEntry(giasDataSource, "Head teacher name")]);

        if (!await featureManager.IsEnabledAsync(FeatureFlags.ContactsInDfeForSchools))
        {
            DataSourcesPerPage = [inSchoolDataSources];
            return pageResult;
        }

        var trustUid = int.TryParse(TrustSummary?.Uid, out var uid) ? uid : (int?)null;
        var dfeContacts = await mediator.Send(new GetSchoolDfeContactsQuery(Urn, trustUid));

        List<DataSourceListEntry> inDfeDataSources = SchoolCategory == SchoolCategory.Academy
            ?
            [
                new DataSourceListEntry(
                    new DataSourceServiceModel(Source.FiatDb, dfeContacts.TrustRelationshipManager?.LastModifiedAtTime,
                        null, dfeContacts.TrustRelationshipManager?.LastModifiedByEmail),
                    TrustContactRole.TrustRelationshipManager.MapRoleToViewString()),
                new DataSourceListEntry(
                    new DataSourceServiceModel(Source.FiatDb, dfeContacts.SfsoLead?.LastModifiedAtTime, null,
                        dfeContacts.SfsoLead?.LastModifiedByEmail),
                    TrustContactRole.SfsoLead.MapRoleToViewString())
            ]
            :
            [
                new DataSourceListEntry(
                    new DataSourceServiceModel(Source.FiatDb,
                        dfeContacts.RegionsGroupLocalAuthorityLead?.LastModifiedAtTime, null,
                        dfeContacts.RegionsGroupLocalAuthorityLead?.LastModifiedByEmail),
                    "Regions group LA lead")
            ];

        DataSourcesPerPage =
        [
            new DataSourcePageListEntry(InDfeModel.SubPageName, inDfeDataSources),
            inSchoolDataSources
        ];

        return pageResult;
    }
}
