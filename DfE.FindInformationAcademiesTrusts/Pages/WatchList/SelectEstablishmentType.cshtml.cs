
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using Dfe.FindInformationAcademiesTrusts.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DfE.FindInformationAcademiesTrusts.Pages.WatchList;

public class SelectEstablishmentType : ContentPageModel
{
    public string? OrganisationType { get; set; }
    public string? ErrorMessage { get; set; }
    
    public bool ShowError { get; set; }
    

    public IList<RadioButtonsLabelViewModel> OrganisationTypeRadioButtons =
    [
        new()
        {
            Name = "School",
            Id = "school",
            Value = "school"
        },
        new()
        {
            Name = "Trust",
            Id = "trust",
            Value = "trust"
        }
    ];

    public void OnGet()
    {
    }

    public IActionResult OnPost(string? organisationType)
    {
        if (string.IsNullOrEmpty(organisationType))
        {
            const string errorMessage = "You must choose an establishment type";

            ModelState.AddModelError("option-selection-error", errorMessage);
            ErrorMessage = errorMessage;
            ShowError = true;

            return Page();
        }

        return organisationType == "school"
            ? RedirectToPage("SearchForASchool")
            : RedirectToPage("SearchForATrust");
    }

}
