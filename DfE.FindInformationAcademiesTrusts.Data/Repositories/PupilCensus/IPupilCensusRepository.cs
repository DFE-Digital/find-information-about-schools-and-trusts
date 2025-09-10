namespace DfE.FindInformationAcademiesTrusts.Data.Repositories.PupilCensus;

public interface IPupilCensusRepository
{
    public Task<AnnualStatistics<SchoolPopulation>> GetSchoolPopulationStatisticsAsync(int urn);
}
