using DfE.FindInformationAcademiesTrusts.Application.Watchlist.Queries;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Services.Search;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.WatchList;

public class SearchForASchool(ISearchService searchService,IWatchlistQueryService watchlistQueryService) : ContentPageModel, IEstablishmentSearchFormModel
{
    
    public void OnGet()
    {
        
    }

    public async Task<IActionResult> OnGetPopulateAutocompleteAsync(CancellationToken cancellationToken)
    {
        var searchResults = await searchService.GetSchoolSearchResultsForAutocompleteAsync(KeyWords);

        var currentUser = User.Identity?.Name;

        var schoolWatchListItems = await watchlistQueryService
            .GetAllEstablishmentsForUser(currentUser ?? string.Empty, cancellationToken);

        var schoolWatchlistUrns = schoolWatchListItems.Value?
            .Select(s => s.Urn)
            .ToHashSet() ?? [];

        var results = searchResults
            .Where(result => !schoolWatchlistUrns.Contains(result.Id))
            .Select(result => new
            {
                id = result.Id,
                referenceNumber = result.referencenumber,
                address = result.Address,
                name = result.Name,
                resultType = result.ResultType
            });

        return new JsonResult(results);
    }
    
    public async Task<IActionResult> OnPostAsync(string id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            ModelState.AddModelError("SelectedSchoolId", "Please select a school from the list.");
            return Page();
        }

        var currentUser = User.Identity?.Name;

       

        return RedirectToPage("/WatchList/ConfirmSchool",new {id});
    }

    public string PageSearchFormInputId => "school-search";
    public string AutocompletePagePath => "/WatchList/SearchForASchool";
}
