namespace DfE.FindInformationAcademiesTrusts.Data.Repositories.Trust;

public interface ITrustRepository
{
    Task<TrustSummary?> GetTrustSummaryAsync(string referenceNumber);
    
    Task<TrustSummary?> GetTrustSummaryByEstablishmentUrnAsync(int urn);
    
    Task<TrustOverview?> GetTrustOverviewByTrnAsync(string referenceNumber);
    Task<TrustContacts> GetTrustContactsAsync(string uid, string? urn = null);
    
}
