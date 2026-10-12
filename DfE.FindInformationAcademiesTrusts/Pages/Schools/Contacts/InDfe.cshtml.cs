using DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Models;
using DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Queries;
using DfE.FindInformationAcademiesTrusts.Configuration;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Schools.Contacts;

[FeatureGate(FeatureFlags.ContactsInDfeForSchools)]
public class InDfeModel(
    IMediator mediator,
    IDataSourceService dataSourceService,
    IVariantFeatureManager featureManager,
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
        if (pageResult is NotFoundResult) return pageResult;

        Contacts = await mediator.Send(new GetSchoolDfeContactsQuery(Urn, TrustUid));

        DataSourcesPerPage =
            await ContactsDataSources.GetAsync(dataSourceService, featureManager, SchoolCategory, Contacts);

        return pageResult;
    }

    private int? TrustUid => int.TryParse(TrustSummary?.Uid, out var uid) ? uid : null;
}
