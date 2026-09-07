using DfE.FindInformationAcademiesTrusts.Application.Watchlist.Queries;
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
            .Select(t => t.TrustReferenceNumber)
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

    public string PageSearchFormInputId => "trust-search";
    public string AutocompletePagePath => "/WatchList/SearchForATrust";
}
