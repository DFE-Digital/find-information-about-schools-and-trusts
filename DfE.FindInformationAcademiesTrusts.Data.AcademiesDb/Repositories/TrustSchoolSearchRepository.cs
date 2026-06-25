using System.Linq.Expressions;
using Dfe.AcademiesApi.Client.Contracts;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Contexts;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Extensions;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Models.Gias;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Search;
using Microsoft.EntityFrameworkCore;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Repositories;

public class TrustSchoolSearchRepository(
    IEstablishmentsV4Client establishmentsClient,
    ITrustsV4Client trustsClient,
    IStringFormattingUtilities stringFormattingUtilities)
    : ITrustSchoolSearchRepository
{
    public async Task<(SearchResult[] Results, SearchResultCount NumberOfResults)>
        GetSearchResultsAsync(string? text, int pageSize, int page = 1)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return ([], new SearchResultCount(0, 0, 0));
        }

        // Run API calls in parallel
        var establishmentsTask =
            establishmentsClient.SearchEstablishments2Async(text, null, text, true, true);

        var trustsTask =
            trustsClient.SearchTrusts3Async(text, null, null, 1, 10, null);

        
        var trustByTrn = trustsClient.GetTrustByTrustReferenceNumberAsync(text).Result;
        
        await Task.WhenAll(establishmentsTask, trustsTask);

        var establishments = establishmentsTask.Result;
        var trusts = trustsTask.Result?.Data ?? [];
        trusts.Add(trustByTrn);

        // Filter (STARTS WITH ONLY)
        var filteredEstablishments = establishments
                .OrderBy(x => x.Name!.StartsWith(text, StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                .ToList();
        

        var filteredTrusts = trusts
            .OrderBy(t =>
                !string.IsNullOrWhiteSpace(t.Name) &&
                t.Name.StartsWith(text, StringComparison.OrdinalIgnoreCase))
            .ToList();

        // Map to SearchResult
        var schoolResults = filteredEstablishments.Select(MapSchool);
        var trustResults = filteredTrusts.Select(MapTrust);

        var allResults = schoolResults
            .Concat(trustResults)
            .OrderBy(x => x.Name)
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
                t.Ukprn!.ToString(),
                t.Name!,
                t.Type!.Name!,
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