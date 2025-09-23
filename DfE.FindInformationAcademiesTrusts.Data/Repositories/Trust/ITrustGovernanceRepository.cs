namespace DfE.FindInformationAcademiesTrusts.Data.Repositories.Trust;

public interface ITrustGovernanceRepository
{
    Task<List<Governor>> GetTrustGovernanceAsync(string uidOrUrn);
}
