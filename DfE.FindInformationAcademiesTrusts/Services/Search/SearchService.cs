using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Search;

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
        if (string.IsNullOrWhiteSpace(keyWords))
        {
            return [];
        }

        var results = await trustSchoolSearchRepository.GetAutoCompleteSearchResultsAsync(keyWords);

        return BuildResults(results);
    }

    public async Task<PagedSearchResults> GetSearchResultsForPageAsync(string? keyWords, int pageNumber)
    {
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
    }

    private static SearchResultServiceModel[] BuildResults(SearchResult[] results)
    {
        return results.Select(x =>
                new SearchResultServiceModel(x.Id, x.ukprn,x.Name, x.Address, x.TrustReferenceNumber, x.Type,
                    x.IsTrust ? ResultType.Trust : ResultType.School))
            .ToArray();
    }
}
