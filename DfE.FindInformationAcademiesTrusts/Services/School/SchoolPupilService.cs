using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.PupilCensus;

namespace DfE.FindInformationAcademiesTrusts.Services.School;

public interface ISchoolPupilService
{
    public Task<AnnualStatistics<SchoolPopulation>> GetSchoolPopulationStatisticsAsync(int urn, CensusYear from,
        CensusYear to);
}

public class SchoolPupilService(
    IDateTimeProvider dateTimeProvider,
    IPupilCensusRepository pupilCensusRepository
) : ISchoolPupilService
{
    public async Task<AnnualStatistics<SchoolPopulation>> GetSchoolPopulationStatisticsAsync(int urn, CensusYear from, CensusYear to)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(to.Value, from.Value);
        Console.WriteLine(dateTimeProvider);
        
        var allAvailableStatistics = await pupilCensusRepository.GetSchoolPopulationStatisticsAsync(urn);
        var result = new AnnualStatistics<SchoolPopulation>();

        foreach (var year in Enumerable.Range(from.Value, to.Value - from.Value + 1))
        {
            if (year > CensusYear.Current(dateTimeProvider).Value)
            {
                result.Add(year, SchoolPopulation.NotYetSubmitted);
            }
            else
            {
                var schoolPopulation = allAvailableStatistics.TryGetValue(year, out var value)
                    ? value
                    : SchoolPopulation.Unknown;
        
                result.Add(year, schoolPopulation);
            }
        }

        return result;
    }
}
