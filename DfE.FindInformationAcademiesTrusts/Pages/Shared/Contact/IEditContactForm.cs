using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DfE.FindInformationAcademiesTrusts.Pages.Shared.Contact;

/// <summary>
/// What the shared _EditContactForm partial needs from a trust or school edit contact page
/// </summary>
public interface IEditContactForm
{
    string? Name { get; }
    string? Email { get; }
    PageMetadata PageMetadata { get; }
    ModelStateDictionary ModelState { get; }
    string CancelPage { get; }
    Dictionary<string, string> CancelRouteValues { get; }
}
