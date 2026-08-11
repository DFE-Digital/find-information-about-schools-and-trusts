using System.ComponentModel.DataAnnotations;
using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using DfE.FindInformationAcademiesTrusts.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DfE.FindInformationAcademiesTrusts.Pages.Shared.Contact;

public abstract class EditContactFormModel : BasePageModel
{
    public abstract PageMetadata PageMetadata { get; }

    [BindProperty]
    [BindRequired]
    [MaxLength(500)]
    public string? Name { get; set; }

    [BindProperty]
    [BindRequired]
    [DfeEmailAddress]
    [MaxLength(320)]
    public string? Email { get; set; }

    public abstract string Id { get; }
    public abstract string IdName { get; }
    public abstract string CancelUrl { get; }
    public abstract string ContactUpdatedUrl { get; }

    /// <summary>
    /// Route values used when redirecting back to <see cref="ContactUpdatedUrl"/> after a successful save.
    /// Defaults to just the identifying route value (<see cref="IdName"/>/<see cref="Id"/>); override to
    /// carry additional context (e.g. ReferenceNumber) through the redirect.
    /// </summary>
    protected virtual RouteValueDictionary RedirectRouteValues => new() { { IdName, Id } };

    /// <summary>
    /// Route values used for the Cancel link back to <see cref="CancelUrl"/>. Bound to the anchor tag helper's
    /// asp-all-route-data, which requires string values (unlike RedirectToPage's RouteValueDictionary). Defaults
    /// to just the identifying route value (<see cref="IdName"/>/<see cref="Id"/>); override to carry additional
    /// context (e.g. ReferenceNumber) through the link.
    /// </summary>
    public virtual Dictionary<string, string> CancelRouteValues => new() { { IdName, Id } };

    [TempData] public string ContactUpdatedMessage { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync()
    {
        if (!await OrganisationExistsAsync())
        {
            return NotFound();
        }

        var contact = await GetContactAsync();

        Email = contact?.Email;
        Name = contact?.FullName;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return await OnGetAsync();
        }

        var result = await UpdateContactAsync();

        ContactUpdatedMessage = GetContactUpdatedMessage(result);

        return RedirectToPage(ContactUpdatedUrl, RedirectRouteValues);
    }

    protected abstract Task<bool> OrganisationExistsAsync();
    protected abstract Task<InternalContact?> GetContactAsync();
    protected abstract Task<InternalContactUpdatedServiceModel> UpdateContactAsync();
    protected abstract string GetContactUpdatedMessage(InternalContactUpdatedServiceModel result);
}
