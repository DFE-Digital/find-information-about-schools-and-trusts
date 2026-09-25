using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Contexts;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Extensions;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Models.Edperf_Mstr;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Academy;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.AcademiesDbServices;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.PupilCensus;

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
                GetFsmPercentage(GetCensusValue(e.Urn, census, ef => ef.CensusNumfsm), GetCensusValue(e.Urn, census, ef => ef.CensusNor)),
                e.LocalAuthorityCode.ParseAsNullableInt(),
                e.EstablishmentType.Name,
                e.PhaseOfEducation.Name))
            .ToArray();
    }

    private static double? GetFsmPercentage(string? freeSchoolMealsNumber, string? pupilsOnRollNumber)
    {
        var pupilsEligibleForFreeSchoolMeals = ParseIntStatistic(freeSchoolMealsNumber);
        var pupilsOnRoll = ParseIntStatistic(pupilsOnRollNumber);
        var pupilsEligibleForFreeSchoolMealsPercentage = pupilsEligibleForFreeSchoolMeals.Compute(
            pupilsOnRoll,
            (fsm, por) => por == 0 ? 0m : Math.Round(100.0m * fsm / por, 1));

        return pupilsEligibleForFreeSchoolMealsPercentage is Statistic<decimal>.WithValue { Value: var percentage }
            ? (double)percentage
            : null;
    }
    
    private static Statistic<int> ParseIntStatistic(string? input)
    {
        return input switch
        {
            "SUPP" => Statistic<int>.Suppressed,
            "NP" => Statistic<int>.NotPublished,
            "NA" => Statistic<int>.NotApplicable,
            _ when int.TryParse(input, out var value) => new Statistic<int>.WithValue(value),
            _ => Statistic<int>.NotAvailable
        };
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
    {
        if (urn.ParseAsNullableInt() is not { } parsedUrn || !edPerfFiats.TryGetValue(parsedUrn, out var census))
        {
            return default;
        }

        return selector(census);
    }
}
