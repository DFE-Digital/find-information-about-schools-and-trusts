using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.Contact;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.NavMenu;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Schools.Contacts;

public abstract class EditSchoolContactFormModel(
    ISchoolService schoolService,
    ISchoolContactsService schoolContactsService,
    SchoolContactRole role) : EditContactFormModel, ISchoolAreaModel
{
    [BindProperty(SupportsGet = true)] public int Urn { get; set; }
    public SchoolCategory SchoolCategory => default;
    public List<DataSourcePageListEntry> DataSourcesPerPage => [];

    public override PageMetadata PageMetadata => new(SchoolName, ModelState.IsValid, ContactsAreaModel.PageName,
        $"Edit {role.MapRoleToViewString()} details");

    public string SchoolName { get; set; } = null!;
    public string SchoolType => null!;
    public TrustSummaryServiceModel? TrustSummary => null;
    public bool IsPartOfAFederation => false;
    public NavLink[] ServiceNavLinks { get; } = [];
    public NavLink[] SubNavLinks { get; } = [];

    public override string Id => Urn.ToString();
    public override string IdName => nameof(Urn);
    public override string CancelUrl => "/Schools/Contacts/InDfe";
    public override string ContactUpdatedUrl => "/Schools/Contacts/InDfe";

    protected override async Task<bool> OrganisationExistsAsync()
    {
        var summary = await schoolService.GetSchoolSummaryAsync(Urn);

        if (summary is null)
        {
            return false;
        }

        SchoolName = summary.Name;
        return true;
    }

    protected override async Task<InternalContact?> GetContactAsync()
    {
        var contacts = await schoolContactsService.GetInternalContactsAsync(int.Parse(Id));
        return GetContactFromServiceModel(contacts);
    }

    protected abstract InternalContact? GetContactFromServiceModel(SchoolInternalContactsServiceModel contacts);

    protected override async Task<InternalContactUpdatedServiceModel> UpdateContactAsync()
    {
        return await schoolContactsService.UpdateContactAsync(int.Parse(Id), Name, Email, role);
    }

    protected override string GetContactUpdatedMessage(InternalContactUpdatedServiceModel result)
    {
        return result.ToContactUpdatedMessage(role.MapRoleToViewString());
    }
}
