using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.AcademiesDbServices;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Search;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Establishments;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Trusts;


namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Repositories;

public class TrustSchoolSearchRepository(
    IGetEstablishments getEstablishments,
    IGetTrusts getTrusts,
    IStringFormattingUtilities stringFormattingUtilities)
    : ITrustSchoolSearchRepository
{
    private static readonly HashSet<int> AllowedEstablishmentGroupTypeCodes =
        Enum.GetValues<NameAndCodeEnums.AllowedEstablishmentGroupTypeCodes>()
            .Select(static code => (int)code)
            .ToHashSet();

    public async Task<(SearchResult[] Results, SearchResultCount NumberOfResults)>
        GetSearchResultsAsync(string? text, int pageSize, int page = 1)
    {
        if (string.IsNullOrWhiteSpace(text) || (int.TryParse(text, out int number) && number < 100000))
        {
            return ([], new SearchResultCount(0, 0, 0));
        }
        

        
        // Run API calls in parallel
        var trustsTask = getTrusts.SearchTrusts(text);

        var establishmentsTask = getEstablishments.SearchEstablishments(text);
        
        await Task.WhenAll(establishmentsTask, trustsTask);
        
        var establishments = establishmentsTask.Result;
        
        var trusts = trustsTask.Result?.Data?.ToList() ?? [];

        if (establishments.Count == 0)
        {
            var trustByTrn = getTrusts.GetTrustByReferenceNumber(text);
            if (trustByTrn.Result != null)
            { 
                trusts.Add(trustByTrn.Result);
            }
        }

        var filteredEstablishments = establishments
            .Where(x => int.TryParse(x.EstablishmentGroupType?.Code, out var code) &&
                        AllowedEstablishmentGroupTypeCodes.Contains(code))
            .OrderBy(x => x.Name!.StartsWith(text, StringComparison.OrdinalIgnoreCase) ? 0 : 1)
            .ToList();

        var filteredTrusts = trusts
            .OrderBy(t =>
                !string.IsNullOrWhiteSpace(t.Name) &&
                t.Name.StartsWith(text, StringComparison.OrdinalIgnoreCase))
            .ToList();
        
        var trustResults = filteredTrusts.Select(MapTrust);
        var establishmentsResults = filteredEstablishments.Select(MapSchool);

        var allResults = establishmentsResults
            .Concat(trustResults)
            .OrderBy(x => x.Name.StartsWith(text, StringComparison.OrdinalIgnoreCase) ? 0 : 1)
            .ToArray();

        // Paging (RESTORED)
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
        
        SearchResult MapSchool(EstablishmentDto e)
        {
            return new SearchResult(
                e.Urn!.ToString(),
               null ,
                e.Name!,
                e.EstablishmentType!.Name!,
                stringFormattingUtilities.BuildAddressString(
                    e.Address!.Street,
                    e.Address.Locality,
                    e.Address.Town,
                    e.Address.Postcode),
                false,
                e.EstablishmentNumber
            );
        }

        SearchResult MapTrust(TrustDto t)
        {
            return new SearchResult(
                t.GroupUid!.ToString(),
                t.ReferenceNumber.ToString(),
                t.Name!,
                t.Type!.Name,
                stringFormattingUtilities.BuildAddressString(
                    t.Address!.Street,
                    t.Address.Locality,
                    t.Address.Town,
                    t.Address.Postcode),
                true,
                t.ReferenceNumber
            );
        }
    }

    public async Task<SearchResult[]> GetAutoCompleteSearchResultsAsync(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return [];
        }

        var (results, _) = await GetSearchResultsAsync(text, 5, 1);

        return results;
    }
}