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

        var routeValues = new RouteValueDictionary { { IdName, Id } };
        return RedirectToPage(ContactUpdatedUrl, routeValues);
    }

    protected abstract Task<bool> OrganisationExistsAsync();
    protected abstract Task<InternalContact?> GetContactAsync();
    protected abstract Task<InternalContactUpdatedServiceModel> UpdateContactAsync();
    protected abstract string GetContactUpdatedMessage(InternalContactUpdatedServiceModel result);
}
