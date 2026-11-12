using DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Models;
using DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Queries;
using DfE.FindInformationAcademiesTrusts.Configuration;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Domain.Enums;
using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.HttpServices;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Schools.Contacts;

[FeatureGate(FeatureFlags.ContactsInDfeForSchools)]
public class InDfeModel(
    IMediator mediator,
    IDataSourceService dataSourceService,
    IGetEstablishmentsTemp getEstablishments,
    ISchoolService schoolService,
    ITrustService trustService,
    ISchoolNavMenu schoolNavMenu)
    : SchoolAreaModel(schoolService, trustService, schoolNavMenu)
{
    public const string PageName = "Contacts";
    public const string SubPageName = "In DfE";

    public override PageMetadata PageMetadata =>
        base.PageMetadata with { PageName = PageName, SubPageName = SubPageName };

    public SchoolDfeContacts Contacts { get; set; } = new(null, null, null);

    public override async Task<IActionResult> OnGetAsync()
    {
        var pageResult = await base.OnGetAsync();
        var establishment = await getEstablishments.GetEstablishment(Urn);
        LaCode = int.TryParse(establishment.LocalAuthorityCode, out var laCode) ? laCode : 0;
        if (pageResult is NotFoundResult) return pageResult;

        var trustUid = int.TryParse(TrustSummary?.Uid, out var uid) ? uid : (int?)null;
        Contacts = await mediator.Send(new GetSchoolDfeContactsQuery(LaCode, trustUid));

        List<DataSourceListEntry> inDfeDataSources = SchoolCategory == SchoolCategory.Academy
            ?
            [
                new DataSourceListEntry(
                    new DataSourceServiceModel(Source.FiatDb, Contacts.TrustRelationshipManager?.LastModifiedAtTime,
                        null, Contacts.TrustRelationshipManager?.LastModifiedByEmail),
                    TrustContactRole.TrustRelationshipManager.MapRoleToViewString()),
                new DataSourceListEntry(
                    new DataSourceServiceModel(Source.FiatDb, Contacts.SfsoLead?.LastModifiedAtTime, null,
                        Contacts.SfsoLead?.LastModifiedByEmail),
                    TrustContactRole.SfsoLead.MapRoleToViewString())
            ]
            :
            [
                new DataSourceListEntry(
                    new DataSourceServiceModel(Source.FiatDb,
                        Contacts.RegionsGroupLocalAuthorityLead?.LastModifiedAtTime, null,
                        Contacts.RegionsGroupLocalAuthorityLead?.LastModifiedByEmail),
                    "Regions group LA lead")
            ];

        var giasDataSource = await dataSourceService.GetAsync(Source.Gias);

        DataSourcesPerPage =
        [
            new DataSourcePageListEntry(SubPageName, inDfeDataSources),
            new DataSourcePageListEntry(InSchoolModel.SubPageName(SchoolCategory),
                [new DataSourceListEntry(giasDataSource, "Head teacher name")])
        ];

        return pageResult;
    }
}
