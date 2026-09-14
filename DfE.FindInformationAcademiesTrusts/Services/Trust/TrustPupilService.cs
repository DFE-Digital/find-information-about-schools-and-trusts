using DfE.FindInformationAcademiesTrusts.Data.Repositories.PupilCensus;

namespace DfE.FindInformationAcademiesTrusts.Services.Trust;

public interface ITrustPupilService
{
    Task<int> GetTotalPupilCountForTrustAsync(string trustReferenceNumber);
    Task<TrustStatistics<Statistic<int>>> GetPupilCountsForSchoolsInTrustAsync(string trustReferenceNumber);
}

public class TrustPupilService(IPupilCensusRepository pupilCensusRepository) : ITrustPupilService
{
    public async Task<int> GetTotalPupilCountForTrustAsync(string trustReferenceNumber)
    {
        var statistics = await pupilCensusRepository.GetMostRecentPopulationStatisticsForTrustAsync(trustReferenceNumber);

        return statistics.Values.Sum(sp => sp.PupilsOnRoll.TryGetValue(out var value) ? value : 0);
    }

    public async Task<TrustStatistics<Statistic<int>>> GetPupilCountsForSchoolsInTrustAsync(string trustReferenceNumber)
    {
        var statistics = await pupilCensusRepository.GetMostRecentPopulationStatisticsForTrustAsync(trustReferenceNumber);

        var result = new TrustStatistics<Statistic<int>>();
        foreach (var (urn, sp) in statistics)
        {
            result.Add(urn, sp.PupilsOnRoll);
        }

        return result;
    }
}
