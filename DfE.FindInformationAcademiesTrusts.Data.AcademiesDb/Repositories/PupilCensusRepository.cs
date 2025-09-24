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

            var pupilsOnRole = ParseIntStatistic(result.CensusNor);
            var pupilsEligibleForFreeSchoolMeals = ParseIntStatistic(result.CensusNumfsm);
            var pupilsEligibleForFreeSchoolMealsPercentage = pupilsEligibleForFreeSchoolMeals.Compute(
                pupilsOnRole,
                (fsm, por) => Math.Round(100.0m * fsm / por, 1)
            );

            var statistics = new SchoolPopulation(
                pupilsOnRole,
                ParseIntStatistic(result.CensusTsenelse),
                ParseDecimalStatistic(result.CensusPsenelse),
                ParseIntStatistic(result.CensusTsenelk),
                ParseDecimalStatistic(result.CensusPsenelk),
                ParseIntStatistic(result.CensusNumeal),
                ParseDecimalStatistic(result.CensusPnumeal),
                pupilsEligibleForFreeSchoolMeals,
                pupilsEligibleForFreeSchoolMealsPercentage
            );

            annualStatistics.Add(year, statistics);
        }

        return annualStatistics;
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
