namespace DfE.FindInformationAcademiesTrusts.Data.Repositories.Search;

public interface ITrustSchoolSearchRepository
{
    Task<SearchResult[]> GetAutoCompleteSearchResultsAsync(string text);
    Task<SearchResult[]> GetSchoolAutoCompleteSearchResultsAsync(string text);
    Task<SearchResult[]> GetTrustAutoCompleteSearchResultsAsync(string text);

    Task<(SearchResult[] Results, SearchResultCount NumberOfResults)> GetSearchResultsAsync(string text, int pageSize, int page = 1);
}
