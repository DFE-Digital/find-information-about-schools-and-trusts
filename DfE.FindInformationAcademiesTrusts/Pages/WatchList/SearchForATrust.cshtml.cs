using DfE.FindInformationAcademiesTrusts.Application.WatchlistCommands.Queries;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Services.Search;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.WatchList;

public class SearchForATrust(ISearchService searchService,IWatchlistQueryService watchlistQueryService) : ContentPageModel, IEstablishmentSearchFormModel
{
    public void OnGet()
    {
        
    }

    public async Task<IActionResult> OnGetPopulateAutocompleteAsync(CancellationToken cancellationToken)
    {
        var searchResults = await searchService
            .GetTrustSearchResultsForAutocompleteAsync(KeyWords);

        var currentUser = User.Identity?.Name;

        var trustWatchListItems = await watchlistQueryService
            .GetAllTrustsForUser(currentUser ?? string.Empty,cancellationToken);

        var trustWatchlistIds = trustWatchListItems.Value?
            .Select(t => t.TrustId)
            .ToHashSet() ?? [];

        var results = searchResults
            .Where(result => result.referencenumber != null && !trustWatchlistIds.Contains(result.referencenumber))
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
    public async Task<IActionResult> OnPostAsync(string referenceNumber, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(referenceNumber))
        {
            ModelState.AddModelError("SelectedTrustId", "Please select a trust from the list.");
            return Page();
        }

        var currentUser = User.Identity?.Name;

       

        return RedirectToPage("/WatchList/ConfirmTrust",new {referenceNumber});
    }
    

    public string PageSearchFormInputId => "trust-search";
    public string AutocompletePagePath => "/WatchList/SearchForATrust";
}
