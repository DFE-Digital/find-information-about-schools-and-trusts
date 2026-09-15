using DfE.FindInformationAcademiesTrusts.Application.WatchlistCommands.Queries;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Services.Search;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.WatchList;

public class SearchForATrust(ISearchService searchService,IWatchlistQueryService watchlistQueryService) : ContentPageModel, IEstablishmentSearchFormModel
{
    public bool ShowError { get; set; }

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
    public async Task<IActionResult> OnPostAsync(string? referenceNumber, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(referenceNumber))
        {
            var keywords = Request.Form["keywords"].ToString();
            if (string.IsNullOrEmpty(keywords))
            {

                ModelState.AddModelError(PageSearchFormInputId, "Enter the trust name or TRN");
                ShowError = true;
                return Page();
            }
            
            ModelState.AddModelError(PageSearchFormInputId, "We could not find any trusts matching your search criteria");
            ShowError = true;
            return Page();
        }
            
        
        
        return RedirectToPage("/WatchList/ConfirmTrust",new {referenceNumber});
    }
    

    public string PageSearchFormInputId => "trust-search";
    public string AutocompletePagePath => "/WatchList/SearchForATrust";
}
