using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Contexts;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Models.Edperf_Mstr;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.PupilCensus;
using Microsoft.EntityFrameworkCore;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Repositories;

public class PupilCensusRepository(IAcademiesDbContext dbContext) : IPupilCensusRepository
{
    public async Task<AnnualStatistics<SchoolPopulation>> GetSchoolPopulationStatisticsAsync(int urn)
    {
        var results = await dbContext
            .EdperfFiats
            .Where(edperfFiat => edperfFiat.Urn == urn)
            .ToListAsync();

        var annualStatistics = new AnnualStatistics<SchoolPopulation>();

        foreach (var result in results)
        {
            // The year is stored in the database in the format '2021-2022'.
            // The academic year starts in the autumn, but the census is produced in the spring,
            // so the second of the two years should be used.
            CensusYear year = int.Parse(result.DownloadYear.Split('-')[1]);
            
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

    private static Statistic<int> ParseIntStatistic(string? input) =>
        input switch
        {
            "SUPP" => Statistic<int>.Suppressed,
            "NP" => Statistic<int>.NotPublished,
            "NA" => Statistic<int>.NotApplicable,
            _ when int.TryParse(input, out var value) => new Statistic<int>.WithValue(value),
            _ => Statistic<int>.NotAvailable
        };

    private static Statistic<decimal> ParseDecimalStatistic(string? input) =>
        input switch
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
