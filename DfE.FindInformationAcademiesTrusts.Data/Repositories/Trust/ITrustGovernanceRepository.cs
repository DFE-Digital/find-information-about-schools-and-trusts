namespace DfE.FindInformationAcademiesTrusts.Data.Repositories.Trust;

public interface ITrustGovernanceRepository
{
    Task<List<Governor>> GetTrustGovernanceAsync(string trn);
    Task<List<Governor>> GetSatGovernanceAsync(int urn);
}
