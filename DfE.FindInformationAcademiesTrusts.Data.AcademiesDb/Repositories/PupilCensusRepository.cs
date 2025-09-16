using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Contexts;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Models.Edperf_Mstr;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.PupilCensus;
using Microsoft.EntityFrameworkCore;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Repositories;

public class PupilCensusRepository(IAcademiesDbContext dbContext) : IPupilCensusRepository
{
    public async Task<AnnualStatistics<SchoolPopulation>> GetSchoolPopulationStatisticsAsync(int urn)
    {
        var results = await GetCensusDataForUrn(urn);

        var annualStatistics = new AnnualStatistics<SchoolPopulation>();

        foreach (var result in results)
        {
            var year = ParseCensusYearFromAcademicYear(result.DownloadYear, Census.Spring);

            var statistics = ConvertEdperfFiatToSchoolPopulation(result);

            annualStatistics.Add(year, statistics);
        }
        
        return annualStatistics;
    }

    public async Task<TrustStatistics<SchoolPopulation>> GetMostRecentPopulationStatisticsForTrustAsync(string uid)
    {
        var results = await dbContext.GiasGroupLinks
            .Where(gl => gl.GroupUid == uid)
            .Join(dbContext.EdperfFiats, gl => gl.Urn, ef => ef.Urn.ToString(), (gl, ef) => ef)
            .GroupBy(ef => ef.Urn)
            .Select(grp => grp.OrderByDescending(ef => ef.DownloadYear).First())
            .ToListAsync();

        var trustStatistics = new TrustStatistics<SchoolPopulation>();

        foreach (var result in results)
        {
            trustStatistics.Add(result.Urn, ConvertEdperfFiatToSchoolPopulation(result));
        }
        
        return trustStatistics;
    }

    private static SchoolPopulation ConvertEdperfFiatToSchoolPopulation(EdperfFiat edperfFiat)
    {
        var pupilsOnRole = ParseIntStatistic(edperfFiat.CensusNor);
        var pupilsEligibleForFreeSchoolMeals = ParseIntStatistic(edperfFiat.CensusNumfsm);
        var pupilsEligibleForFreeSchoolMealsPercentage = pupilsEligibleForFreeSchoolMeals.Compute(
            pupilsOnRole,
            (fsm, por) => Math.Round(100.0m * fsm / por, 1)
        );

        return new SchoolPopulation(
            pupilsOnRole,
            ParseIntStatistic(edperfFiat.CensusTsenelse),
            ParseDecimalStatistic(edperfFiat.CensusPsenelse),
            ParseIntStatistic(edperfFiat.CensusTsenelk),
            ParseDecimalStatistic(edperfFiat.CensusPsenelk),
            ParseIntStatistic(edperfFiat.CensusNumeal),
            ParseDecimalStatistic(edperfFiat.CensusPnumeal),
                pupilsEligibleForFreeSchoolMeals,
                pupilsEligibleForFreeSchoolMealsPercentage
            );

            
    }

    public async Task<AnnualStatistics<Attendance>> GetAttendanceStatisticsAsync(int urn)
    {
        var results = await GetCensusDataForUrn(urn);

        var annualStatistics = new AnnualStatistics<Attendance>();

        foreach (var result in results)
        {
            var year = ParseCensusYearFromAcademicYear(result.DownloadYear, Census.Autumn);

            var statistics = new Attendance(
                ParseDecimalStatistic(result.AbsencePerctot),
                ParseDecimalStatistic(result.AbsencePpersabs10)
            );

            annualStatistics.Add(year, statistics);
        }

        return annualStatistics;
    }

    private async Task<List<EdperfFiat>> GetCensusDataForUrn(int urn)
    {
        return await dbContext
            .EdperfFiats
            .Where(edperfFiat => edperfFiat.Urn == urn)
            .ToListAsync();
    }

    private static CensusYear ParseCensusYearFromAcademicYear(string academicYearText, Census census)
    {
        // The academic year is from September until July and is stored in the database in the format '2021-2022'.
        // For the above example, the autumn census would be conducted in 2021, and the spring census in 2022.
        // Therefore, for the autumn census, the first year should be used, and for the spring census, the second year.
        var index = census == Census.Autumn ? 0 : 1;

        CensusYear year = int.Parse(academicYearText.Split('-')[index]);
        return year;
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

    private static Statistic<decimal> ParseDecimalStatistic(string? input)
    {
        return input switch
        {
            null => Statistic<decimal>.NotAvailable,
            "SUPP" => Statistic<decimal>.Suppressed,
            "NP" => Statistic<decimal>.NotPublished,
            "NA" => Statistic<decimal>.NotApplicable,
            _ when input.EndsWith('%') => ParseDecimalStatistic(input[..^1]),
            _ when decimal.TryParse(input, out var value) => new Statistic<decimal>.WithValue(value),
            _ => Statistic<decimal>.NotAvailable
        };
    }
}
