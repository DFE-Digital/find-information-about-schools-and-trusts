using DfE.FindInformationAcademiesTrusts.Data.Repositories.PupilCensus;

namespace DfE.FindInformationAcademiesTrusts.Services.Trust;

public interface ITrustPupilService
{
    Task<int> GetTotalPupilCountForTrustAsync(string uid);
    Task<TrustStatistics<Statistic<int>>> GetPupilCountsForSchoolsInTrustAsync(string uid);
}

public class TrustPupilService(IPupilCensusRepository pupilCensusRepository) : ITrustPupilService
{
    public async Task<int> GetTotalPupilCountForTrustAsync(string uid)
    {
        var statistics = await pupilCensusRepository.GetMostRecentPopulationStatisticsForTrustAsync(uid);

        return statistics.Values.Sum(sp => sp.PupilsOnRole.TryGetValue(out var value) ? value : 0);
    }

    public async Task<TrustStatistics<Statistic<int>>> GetPupilCountsForSchoolsInTrustAsync(string uid)
    {
        var statistics = await pupilCensusRepository.GetMostRecentPopulationStatisticsForTrustAsync(uid);

        var result = new TrustStatistics<Statistic<int>>();
        foreach (var (urn, sp) in statistics)
        {
            result.Add(urn, sp.PupilsOnRole);
        }

        return result;
    }
}
