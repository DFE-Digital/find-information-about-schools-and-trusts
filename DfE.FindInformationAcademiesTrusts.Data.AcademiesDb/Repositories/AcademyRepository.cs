using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Contexts;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Extensions;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Models.Edperf_Mstr;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Academy;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.AcademiesDbServices;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Repositories;

public class AcademyRepository(IAcademiesDbContext academiesDbContext, IGetEstablishments getEstablishments)
    : IAcademyRepository
{
    public async Task<AcademyDetails[]> GetAcademiesInTrustDetailsAsync(string uid)
    {
        return await academiesDbContext.GiasGroupLinks
            .Where(gl => gl.GroupUid == uid)
            .Join(academiesDbContext.GiasEstablishments,
                gl => gl.Urn!, e => e.Urn.ToString(),
                (gl, e) =>
                    new AcademyDetails(e.Urn.ToString(),
                        e.EstablishmentName,
                        e.TypeOfEstablishmentName,
                        e.LaName,
                        e.UrbanRuralName,
                        DateOnly.ParseExact(gl.JoinedDate!, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None)))
            .ToArrayAsync();
    }
    
    public async Task<AcademyPupilNumbers[]> GetAcademiesInTrustPupilNumbersByTrnAsync(string referenceNumber)
    {
        var details = await getEstablishments.GetEstablishmentsByTrustReferenceNumber(referenceNumber);
        var censusNumbers = await GetMostRecentCensusByUrnAsync(details.Select(e => e.Urn));

        return details
            .Select(e => new AcademyPupilNumbers(
                e.Urn.ToString(),
                e.Name,
                e.PhaseOfEducation.Name,
                new AgeRange(e.StatutoryLowAge, e.StatutoryHighAge),
                GetCensusValue(e.Urn, censusNumbers, ef => ef.CensusNor.ParseAsNullableInt()),
                e.SchoolCapacity.ParseAsNullableInt()))
            .ToArray();
    }

    public async Task<AcademyFreeSchoolMeals[]> GetAcademiesInTrustFreeSchoolMealsAsync(string referenceNumber)
    {
        var result = await getEstablishments.GetEstablishmentsByTrustReferenceNumber(referenceNumber);
        var census = await GetMostRecentCensusByUrnAsync(result.Select(e => e.Urn));

        return result
            .Select(e => new AcademyFreeSchoolMeals(
                e.Urn.ToString(),
                e.Name,
                GetCensusValue(e.Urn, census, ef => ef.CensusPnumeal?.TrimEnd('%').ParseAsNullableDouble()),
                e.LocalAuthorityCode.ParseAsNullableInt(),
                e.EstablishmentType.Name,
                e.PhaseOfEducation.Name))
            .ToArray();
    }

    public async Task<int> GetNumberOfAcademiesInTrustAsync(string referenceNumber)
    {
        var result =  await getEstablishments.GetEstablishmentsByTrustReferenceNumber(referenceNumber);
        return result.Length;
    }

    public async Task<string?> GetSingleAcademyTrustAcademyUrnAsync(string referenceNumber)
    {
        var result = await getEstablishments.GetEstablishmentsByTrustReferenceNumber(referenceNumber);

        return result[0].Urn;
    }

    public async Task<AcademyOverview[]> GetOverviewOfAcademiesInTrustAsync(string referenceNumber)
    {
        var result = await getEstablishments.GetEstablishmentsByTrustReferenceNumber(referenceNumber);
        var census = await GetMostRecentCensusByUrnAsync(result.Select(e => e.Urn));

        return result.Select(e => new AcademyOverview(
                e.Urn.ToString(),
                e.LocalAuthorityName,
                GetCensusValue(e.Urn, census, ef => ef.CensusNor.ParseAsNullableInt()),
                e.SchoolCapacity.ParseAsNullableInt()
            )).ToArray();
    }

    private async Task<Dictionary<int, EdperfFiat>> GetMostRecentCensusByUrnAsync(IEnumerable<string?> urnStrings)
    {
        var urns = urnStrings
            .Select(urn => urn.ParseAsNullableInt())
            .Where(urn => urn is not null)
            .Select(urn => urn!.Value)
            .Distinct()
            .ToList();

        var results = await academiesDbContext.EdperfFiats
            .Where(ef => urns.Contains(ef.Urn))
            .GroupBy(ef => ef.Urn)
            .Select(grp => grp.OrderByDescending(ef => ef.DownloadYear).First())
            .ToListAsync();

        return results.ToDictionary(ef => ef.Urn);
    }

    private static TValue? GetCensusValue<TValue>(
        string urn,
        Dictionary<int, EdperfFiat> edPerfFiats,
        Func<EdperfFiat, TValue?> selector)
        where TValue : struct
    {
        if (urn.ParseAsNullableInt() is not { } parsedUrn || !edPerfFiats.TryGetValue(parsedUrn, out var census))
        {
            return null;
        }

        return selector(census);
    }
}
