using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Services.Search;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.WatchList;

public class SearchForATrust(ISearchService searchService) : ContentPageModel, IEstablishmentSearchFormModel
{
    public void OnGet()
    {
        
    }

    public async Task<IActionResult> OnGetPopulateAutocompleteAsync()
    {
        var results = (await searchService.GetTrustSearchResultsForAutocompleteAsync(KeyWords))
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
