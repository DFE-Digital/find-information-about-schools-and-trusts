using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.Contact;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Trusts.Contacts;

public abstract class EditTrustContactFormModel(ITrustService trustService, TrustContactRole role)
    : EditContactFormModel, ITrustsAreaModel
{
    public TrustSummaryServiceModel TrustSummary { get; set; } = null!;
    public List<DataSourcePageListEntry> DataSourcesPerPage { get; } = [];

    [BindProperty(SupportsGet = true)] public string Uid { get; set; } = "";

    public override PageMetadata PageMetadata => new(TrustSummary.Name, ModelState.IsValid, ContactsAreaModel.PageName,
        $"Edit {role.MapRoleToViewString()} details");

    public override string Id => Uid;

    public override string IdName => "Uid";
    public override string ContactUpdatedUrl => "/Trusts/Contacts/InDfe";
    public override string CancelUrl => "/Trusts/Contacts/InDfe";

    protected abstract InternalContact? GetContactFromServiceModel(TrustContactsServiceModel contacts);

    protected override async Task<bool> OrganisationExistsAsync()
    {
        var summary = await trustService.GetTrustSummaryAsync(Id);

        if (summary is null)
        {
            return false;
        }

        TrustSummary = summary;
        return true;
    }

    protected override async Task<InternalContact?> GetContactAsync()
    {
        var contacts = await trustService.GetTrustContactsAsync(Id);
        return GetContactFromServiceModel(contacts);
    }

    protected override async Task<InternalContactUpdatedServiceModel> UpdateContactAsync()
    {
        return await trustService.UpdateContactAsync(int.Parse(Id), Name, Email, role);
    }

    protected override string GetContactUpdatedMessage(InternalContactUpdatedServiceModel result)
    {
        return result.ToContactUpdatedMessage(role.MapRoleToViewString());
    }
}
