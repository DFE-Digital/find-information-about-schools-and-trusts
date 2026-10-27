using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Search;
using DfE.FindInformationAcademiesTrusts.HttpServices;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Establishments;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Trusts;

namespace DfE.FindInformationAcademiesTrusts.Services.Search;

public interface ISearchService
{
    Task<SearchResultServiceModel[]> GetSearchResultsForAutocompleteAsync(string? keyWords);
    Task<SearchResultServiceModel[]> GetSchoolSearchResultsForAutocompleteAsync(string? keyWords);
    Task<SearchResultServiceModel[]> GetTrustSearchResultsForAutocompleteAsync(string? keyWords);
    Task<PagedSearchResults> GetSearchResultsForPageAsync(string? keyWords, int pageNumber);
}

public class SearchService(
    IGetEstablishmentsTemp getEstablishments,
    IGetTrustsTemp getTrusts,
    IStringFormattingUtilities stringFormattingUtilities) : ISearchService
{
    private const int PageSize = 20;
    private const int AutocompleteSize = 5;

    private static readonly HashSet<int> AllowedEstablishmentGroupTypeCodes =
        Enum.GetValues<NameAndCodeEnumsTemp.AllowedEstablishmentGroupTypeCodes>()
            .Select(static code => (int)code)
            .ToHashSet();

    public async Task<SearchResultServiceModel[]> GetSearchResultsForAutocompleteAsync(string? keyWords)
    {
        if (string.IsNullOrWhiteSpace(keyWords))
        {
            return [];
        }

        var (results, _) = await GetSearchResultsAsync(keyWords, AutocompleteSize, 1);

        return BuildResults(results);
    }

    public async Task<SearchResultServiceModel[]> GetSchoolSearchResultsForAutocompleteAsync(string? keyWords)
    {
        if (IsInvalidSearchText(keyWords))
        {
            return [];
        }

        var establishments = await getEstablishments.SearchEstablishments(keyWords!);

        var results = establishments
            .Where(IsAllowedEstablishmentGroupType)
            .OrderBy(x => x.Name.StartsWith(keyWords!, StringComparison.OrdinalIgnoreCase) ? 0 : 1)
            .Take(AutocompleteSize)
            .Select(MapSchool)
            .ToArray();

        return BuildResults(results);
    }

    public async Task<SearchResultServiceModel[]> GetTrustSearchResultsForAutocompleteAsync(string? keyWords)
    {
        if (IsInvalidSearchText(keyWords))
        {
            return [];
        }

        var trustSearchResults = await getTrusts.SearchTrusts(keyWords!);
        var trusts = trustSearchResults.Data?.ToList() ?? [];

        if (trusts.Count == 0)
        {
            var trustByTrn = await getTrusts.GetTrustByReferenceNumber(keyWords!);
            if (trustByTrn != null)
            {
                trusts.Add(trustByTrn);
            }
        }

        var results = trusts
            .OrderBy(t =>
                !string.IsNullOrWhiteSpace(t.Name) &&
                t.Name.StartsWith(keyWords!, StringComparison.OrdinalIgnoreCase))
            .Take(AutocompleteSize)
            .Select(t => new SearchResult(
                t.GroupUid?.ToString() ?? string.Empty,
                t.ReferenceNumber?.ToString(),
                t.Name ?? string.Empty,
                t.Type.Name,
                BuildAddress(t.Address.Street, t.Address.Locality, t.Address.Town, t.Address.Postcode),
                true,
                t.ReferenceNumber))
            .ToArray();

        return BuildResults(results);
    }

    public async Task<PagedSearchResults> GetSearchResultsForPageAsync(string? keyWords, int pageNumber)
    {
        if (string.IsNullOrWhiteSpace(keyWords))
        {
            return new PagedSearchResults(PaginatedList<SearchResultServiceModel>.Empty(), new SearchResultsOverview());
        }

        var searchResults = await GetSearchResultsAsync(keyWords, PageSize, pageNumber);

        var results = BuildResults(searchResults.Results);

        return new PagedSearchResults(new PaginatedList<SearchResultServiceModel>(results,
                searchResults.NumberOfResults.TotalRecords, pageNumber,
                PageSize),
            new SearchResultsOverview(searchResults.NumberOfResults.NumberOfTrusts,
                searchResults.NumberOfResults.NumberOfSchools));
    }

    private async Task<(SearchResult[] Results, SearchResultCount NumberOfResults)>
        GetSearchResultsAsync(string text, int pageSize, int page)
    {
        if (IsInvalidSearchText(text))
        {
            return ([], new SearchResultCount(0, 0, 0));
        }

        // Run API calls in parallel
        var trustsTask = getTrusts.SearchTrusts(text);
        var establishmentsTask = getEstablishments.SearchEstablishments(text);

        await Task.WhenAll(establishmentsTask, trustsTask);

        var establishments = await establishmentsTask;
        var trusts = (await trustsTask)?.Data?.ToList() ?? [];

        if (establishments.Count == 0)
        {
            var trustByTrn = await getTrusts.GetTrustByReferenceNumber(text);
            if (trustByTrn != null)
            {
                trusts.Add(trustByTrn);
            }
        }

        var filteredEstablishments = establishments
            .Where(IsAllowedEstablishmentGroupType)
            .OrderBy(x => x.Name!.StartsWith(text, StringComparison.OrdinalIgnoreCase) ? 0 : 1)
            .ToList();

        var filteredTrusts = trusts
            .OrderBy(t =>
                !string.IsNullOrWhiteSpace(t.Name) &&
                t.Name.StartsWith(text, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var allResults = filteredEstablishments.Select(MapSchool)
            .Concat(filteredTrusts.Select(MapTrust))
            .OrderBy(x => x.Name.StartsWith(text, StringComparison.OrdinalIgnoreCase) ? 0 : 1)
            .ToArray();

        var paged = allResults
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArray();

        return (
            paged,
            new SearchResultCount(
                allResults.Length,
                filteredTrusts.Count,
                filteredEstablishments.Count
            )
        );
    }

    private static bool IsInvalidSearchText(string? text)
    {
        return string.IsNullOrWhiteSpace(text) || (int.TryParse(text, out var number) && number < 100000);
    }

    private static bool IsAllowedEstablishmentGroupType(EstablishmentDto establishment)
    {
        return int.TryParse(establishment.EstablishmentGroupType?.Code, out var code) &&
               AllowedEstablishmentGroupTypeCodes.Contains(code);
    }

    private SearchResult MapSchool(EstablishmentDto e)
    {
        return new SearchResult(
            e.Urn!.ToString(),
            null,
            e.Name!,
            e.EstablishmentType!.Name!,
            BuildAddress(e.Address!.Street, e.Address.Locality, e.Address.Town, e.Address.Postcode),
            false,
            e.EstablishmentNumber);
    }

    private SearchResult MapTrust(TrustDto t)
    {
        return new SearchResult(
            t.GroupUid!.ToString(),
            t.ReferenceNumber.ToString(),
            t.Name!,
            t.Type.Name,
            BuildAddress(t.Address!.Street, t.Address.Locality, t.Address.Town, t.Address.Postcode),
            true,
            t.ReferenceNumber);
    }

    private string BuildAddress(string? street, string? locality, string? town, string? postcode)
    {
        return stringFormattingUtilities.BuildAddressString(street, locality, town, postcode);
    }

    private static SearchResultServiceModel[] BuildResults(SearchResult[] results)
    {
        return results.Select(x =>
                new SearchResultServiceModel(x.Id, x.ReferenceNumber, x.Name, x.Address, x.TrustReferenceNumber, x.Type,
                    x.IsTrust ? ResultType.Trust : ResultType.School))
            .ToArray();
    }
}
