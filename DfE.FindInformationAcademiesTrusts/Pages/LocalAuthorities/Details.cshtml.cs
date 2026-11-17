using DfE.FindInformationAcademiesTrusts.HttpServices;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using GovUK.Dfe.AcademiesApi.Client.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.LocalAuthorities;

public class DetailsModel(IGetLocalAuthorities getLocalAuthorities) : BasePageModel
{
    [BindProperty(SupportsGet = true)] public string? LaCode { get; set; }

    public NameAndCodeDto LocalAuthority { get; private set; } = null!;

    public async Task<IActionResult> OnGetAsync()
    {
        if (string.IsNullOrWhiteSpace(LaCode)) return NotFound();

        var matches = await getLocalAuthorities.SearchLocalAuthorities(null, LaCode);
        var match = matches.FirstOrDefault(la => la.Code == LaCode) ?? matches.FirstOrDefault();
        if (match is null) return NotFound();

        LocalAuthority = match;
        return Page();
    }
}
