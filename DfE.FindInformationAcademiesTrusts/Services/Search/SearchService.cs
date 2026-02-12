using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Search;
using Microsoft.Extensions.Logging;

namespace DfE.FindInformationAcademiesTrusts.Services.Search;

public interface ISearchService
{
    Task<SearchResultServiceModel[]> GetSearchResultsForAutocompleteAsync(string? keyWords);
    Task<PagedSearchResults> GetSearchResultsForPageAsync(string? keyWords, int pageNumber);
}

public class SearchService(ITrustSchoolSearchRepository trustSchoolSearchRepository) : ISearchService
{
    private const int PageSize = 20;

    public async Task<SearchResultServiceModel[]> GetSearchResultsForAutocompleteAsync(string? keyWords)
    {
        //try
        //{
            if (string.IsNullOrWhiteSpace(keyWords))
            {
                return [];
            }
            
            var results = await trustSchoolSearchRepository.GetAutoCompleteSearchResultsAsync(keyWords);

            return BuildResults(results);
        //}
        //catch (Exception ex)
        //{
        //    logger.LogError(ex, "Error getting autocomplete search results");
        //    return [];
        //}
    }

    public async Task<PagedSearchResults> GetSearchResultsForPageAsync(string? keyWords, int pageNumber)
    {
        //try
        //{
            if (string.IsNullOrWhiteSpace(keyWords))
            {
                return new PagedSearchResults(PaginatedList<SearchResultServiceModel>.Empty(), new SearchResultsOverview());
            }

            var searchResults = await trustSchoolSearchRepository.GetSearchResultsAsync(keyWords, PageSize, pageNumber);

            var results = BuildResults(searchResults.Results);

            return new PagedSearchResults(new PaginatedList<SearchResultServiceModel>(results,
                    searchResults.NumberOfResults.TotalRecords, pageNumber,
                    PageSize),
                new SearchResultsOverview(searchResults.NumberOfResults.NumberOfTrusts,
                    searchResults.NumberOfResults.NumberOfSchools));
        //}
        //catch (Exception ex)
        //{
        //    logger.LogError(ex, "Error getting search results for page");
        //    return new PagedSearchResults(PaginatedList<SearchResultServiceModel>.Empty(), new SearchResultsOverview());
        //}
    }

    private static SearchResultServiceModel[] BuildResults(SearchResult[] results)
    {
        return results.Select(x =>
                new SearchResultServiceModel(x.Id, x.Name, x.Address, x.TrustReferenceNumber, x.Type,
                    x.IsTrust ? ResultType.Trust : ResultType.School))
            .ToArray();
    }
}
