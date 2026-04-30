using System.Linq.Expressions;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Contexts;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Extensions;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Models.Gias;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Search;
using Microsoft.EntityFrameworkCore;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Repositories;

public class TrustSchoolSearchRepository(
    IAcademiesDbContext academiesDbContext,
    IStringFormattingUtilities stringFormattingUtilities) : ITrustSchoolSearchRepository
{
    public async Task<(SearchResult[] Results, SearchResultCount NumberOfResults)> GetSearchResultsAsync(string? text,
        int pageSize, int page = 1)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return ([], new SearchResultCount(0, 0, 0));
        }

        //Count is done without the select for speed efficiency
        var numberOfTrusts = await CreateTrustSearchQuery(text).CountAsync();
        var numberOfSchools = await CreateSchoolSearchQuery(text).CountAsync();
        var totalCount = numberOfTrusts + numberOfSchools;

        //If there's no results then don't go back to the db
        if (totalCount == 0)
            return ([], new SearchResultCount(0, 0, 0));

        //Now get all the results
        var results = await BuildOrderedSearchResultQuery(text)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync();

        return (results, new SearchResultCount(totalCount, numberOfTrusts, numberOfSchools));
    }

    public async Task<SearchResult[]> GetAutoCompleteSearchResultsAsync(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return [];
        }

        var results = await BuildOrderedSearchResultQuery(text)
            .Take(5)
            .ToArrayAsync();

        return results;
    }

    private IQueryable<GiasGroup> CreateTrustSearchQuery(string searchTerm)
    {
        return academiesDbContext.Groups.Trusts()
            .Where(g =>
                    g.GroupId!.Contains(searchTerm) // GroupId cannot be null for a trust
                    || g.GroupName!.Contains(searchTerm) // Enforced by global EF query filter
            );
    }

    private IQueryable<GiasEstablishment> CreateSchoolSearchQuery(string searchTerm)
    {
        return academiesDbContext.GiasEstablishments
            .Where(x =>
                x.EstablishmentName!.Contains(searchTerm) // Enforced by global EF query filter
                || x.Urn.ToString().Contains(searchTerm));
    }


    [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S3776:Cognitive Complexity of methods should not be too high", Justification = "In order for this to work the query needs to be inside the OrderBy statement")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3358:Ternary operators should not be nested", Justification = "Need to have the :2 on the end for other results")]
    private IQueryable<SearchResult> BuildOrderedSearchResultQuery(string text)
    {
        IQueryable<TempSearchResult> searchResults = SelectTrusts(CreateTrustSearchQuery(text));
        searchResults = searchResults.Union(SelectSchools(CreateSchoolSearchQuery(text)));

        return searchResults
            .OrderBy(x =>
                x.Name == text ? 0 :
                x.Id == text ? 0 :
                x.Name.StartsWith(text) ? 1 :
                x.Name.EndsWith(text) ? 2 :
                x.Name.Contains(text) ? 2 : 3)
            .ThenBy(x => x.Id)
            .Select(BuildAutoCompleteResult());
    }

    private Expression<Func<TempSearchResult, SearchResult>> BuildAutoCompleteResult()
    {
        return x => new SearchResult(x.Id, x.Name, x.Type, stringFormattingUtilities.BuildAddressString(
            x.Street,
            x.Locality,
            x.Town,
            x.PostCode), x.IsTrust, x.TrustGroupId);
    }


    private static IQueryable<TempSearchResult> SelectTrusts(IQueryable<GiasGroup> trustsBaseQuery)
    {
        var query = trustsBaseQuery
            .Select(g => new TempSearchResult
            {
                Street = g.GroupContactStreet,
                Locality = g.GroupContactLocality,
                Town = g.GroupContactTown,
                PostCode = g.GroupContactPostcode,
                Name = g.GroupName!,
                Id = g.GroupUid!.ToString(),
                TrustGroupId = g.GroupId!.ToString(),
                Type = g.GroupType!,
                IsTrust = true
            });
        return query;
    }

    private IQueryable<TempSearchResult> SelectSchools(IQueryable<GiasEstablishment> schoolsBaseQuery)
    {
        var query = schoolsBaseQuery
            .Select(e => new TempSearchResult
            {
                Street = e.Street,
                Locality = e.Locality,
                Town = e.Town,
                PostCode = e.Postcode,
                TrustGroupId = null,
                Name = e.EstablishmentName!,
                Id = e.Urn.ToString(),
                Type = e.TypeOfEstablishmentName!,
                IsTrust = false
            });
        return query;
    }

    private sealed class TempSearchResult
    {
        public string Id { get; init; } = null!;
        public string Name { get; init; } = null!;
        public string? Street { get; init; }
        public string? Locality { get; init; }
        public string? Town { get; init; }
        public string? PostCode { get; init; }
        public string? TrustGroupId { get; init; }
        public string Type { get; init; } = null!;
        public bool IsTrust { get; init; }
    }
}
