using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.PupilCensus;

namespace DfE.FindInformationAcademiesTrusts.Services.School;

public interface ISchoolPupilService
{
    public Task<AnnualStatistics<SchoolPopulation>> GetSchoolPopulationStatisticsAsync(int urn, CensusYear from,
        CensusYear to);

    public Task<AnnualStatistics<Attendance>> GetAttendanceStatisticsAsync(int urn, CensusYear from, CensusYear to);
}

public class SchoolPupilService(
    IDateTimeProvider dateTimeProvider,
    IPupilCensusRepository pupilCensusRepository
) : ISchoolPupilService
{
    public async Task<AnnualStatistics<SchoolPopulation>> GetSchoolPopulationStatisticsAsync(int urn, CensusYear from,
        CensusYear to)
    {
        var allAvailableStatistics = await pupilCensusRepository.GetSchoolPopulationStatisticsAsync(urn);
        return GetCompleteStatisticsBetweenYears(
            Census.Spring,
            from,
            to,
            allAvailableStatistics,
            SchoolPopulation.Unknown,
            SchoolPopulation.NotYetSubmitted
        );
    }

    public async Task<AnnualStatistics<Attendance>> GetAttendanceStatisticsAsync(int urn, CensusYear from, CensusYear to)
    {
        var allAvailableStatistics = await pupilCensusRepository.GetAttendanceStatisticsAsync(urn);
        return GetCompleteStatisticsBetweenYears(
            Census.Autumn,
            from,
            to,
            allAvailableStatistics,
            Attendance.Unknown,
            Attendance.NotYetSubmitted
        );
    }

    private AnnualStatistics<T> GetCompleteStatisticsBetweenYears<T>(Census census, CensusYear from, CensusYear to,
        AnnualStatistics<T> allAvailableStatistics, T unknownValue, T notYetSubmittedValue)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(to.Value, from.Value);

        var result = new AnnualStatistics<T>();
        foreach (var year in Enumerable.Range(from.Value, to.Value - from.Value + 1))
        {
            if (year > CensusYear.Current(dateTimeProvider, census).Value)
            {
                result.Add(year, notYetSubmittedValue);
            }
            else
            {
                var statistic = allAvailableStatistics.TryGetValue(year, out var value)
                    ? value
                    : unknownValue;

                result.Add(year, statistic);
            }
        }

        return result;
    }
}
