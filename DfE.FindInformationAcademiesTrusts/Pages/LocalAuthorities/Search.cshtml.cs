using DfE.FindInformationAcademiesTrusts.HttpServices;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.LocalAuthorities;

public class SearchModel : BasePageModel, IPageSearchFormModel
{
    private readonly IGetLocalAuthorities _getLocalAuthorities;

    public SearchModel(IGetLocalAuthorities getLocalAuthorities)
    {
        _getLocalAuthorities = getLocalAuthorities;
        ShowHeaderSearch = false;
    }

    [BindProperty] public string? LaCode { get; set; }

    public string PageSearchFormInputId => "la-search";
    public string LabelText => "Search for a local authority ";
    public string HintText => "Search by LA code or name. Include any punctuation in names.";
    public string PlaceholderText => "";

    public IActionResult OnPost()
    {
        return RedirectToPage("/LocalAuthorities/Results", new { KeyWords, LaCode });
    }

    public async Task<IActionResult> OnGetPopulateAutocompleteAsync(string? keywords)
    {
        if (string.IsNullOrWhiteSpace(keywords)) return new JsonResult(Array.Empty<object>());

        var matches = await _getLocalAuthorities.Find(keywords);
        return new JsonResult(matches.Take(5).Select(la => new { code = la.Code, name = la.Name }));
    }
}
