namespace DfE.FindInformationAcademiesTrusts.Data.Repositories.Trust;

public interface ITrustRepository
{
    Task<TrustSummary?> GetTrustSummaryAsync(string referenceNumber);
    
    Task<TrustSummary?> GetTrustSummaryByEstablishmentUrnAsync(int urn);
    Task<TrustOverview> GetTrustOverviewAsync(string uid);
    Task<TrustContacts> GetTrustContactsAsync(string uid, string? urn = null);
    Task<string> GetTrustReferenceNumberAsync(string uid);
}
