using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.HttpServices;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using GovUK.Dfe.AcademiesApi.Client.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.LocalAuthorities;

public class ResultsModel : ContentPageModel, IPageSearchFormModel, IPaginationModel
{
    private const int PageSize = 10;
    private readonly IGetLocalAuthorities _getLocalAuthorities;

    public ResultsModel(IGetLocalAuthorities getLocalAuthorities)
    {
        _getLocalAuthorities = getLocalAuthorities;
        ShowHeaderSearch = false;
    }

    [BindProperty(SupportsGet = true)] public string? LaCode { get; set; }
    [BindProperty(SupportsGet = true)] public int PageNumber { get; set; } = 1;

    public IPaginatedList<NameAndCodeDto> SearchResults { get; private set; } = PaginatedList<NameAndCodeDto>.Empty();

    public string PageName => "LocalAuthorities/Results";
    public IPageStatus PageStatus => SearchResults.PageStatus;
    public Dictionary<string, string> PaginationRouteData { get; private set; } = new();

    public string PageSearchFormInputId => "la-search";
    public string LabelText => "Search for a local authority ";
    public string HintText => "Search by LA Code or name. Include any punctuation in names.";
    public string PlaceholderText => "";

    public string Title => PageStatus.TotalPages > 1
        ? $"Search (page {PageStatus.PageIndex} of {PageStatus.TotalPages}) - {KeyWords}"
        : $"Search - {KeyWords}";

    public IActionResult OnPost()
    {
        return RedirectToPage(new { KeyWords, LaCode });
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!string.IsNullOrWhiteSpace(LaCode))
        {
            return RedirectToPage("/LocalAuthorities/Details", new { LaCode });
        }

        if (PageNumber <= 0) return NotFound();
        if (string.IsNullOrWhiteSpace(KeyWords)) return Page();

        var matches = await _getLocalAuthorities.Find(KeyWords);

        if (matches.Count == 1)
        {
            return RedirectToPage("/LocalAuthorities/Details", new { LaCode = matches[0].Code });
        }

        var pageCount = (int)Math.Ceiling(matches.Count / (double)PageSize);
        if (matches.Count > 0 && PageNumber > pageCount) return NotFound();

        SearchResults = new PaginatedList<NameAndCodeDto>(
            matches.Skip((PageNumber - 1) * PageSize).Take(PageSize), matches.Count, PageNumber, PageSize);
        PaginationRouteData = new Dictionary<string, string> { { "KeyWords", KeyWords } };
        return Page();
    }
}
