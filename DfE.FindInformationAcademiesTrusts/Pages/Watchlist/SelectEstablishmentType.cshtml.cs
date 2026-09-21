using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using Dfe.FindInformationAcademiesTrusts.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Watchlist;

public class SelectEstablishmentType : ContentPageModel
{
    private IList<RadioButtonsLabelViewModel> _organisationTypeRadioButtons =
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

    public IList<RadioButtonsLabelViewModel> OrganisationTypeRadioButtons
    {
        get => _organisationTypeRadioButtons;
        set => _organisationTypeRadioButtons = value;
    }

    public string? OrganisationType { get; set; }
    public string? ErrorMessage { get; set; }
    public bool ShowError { get; set; }

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
