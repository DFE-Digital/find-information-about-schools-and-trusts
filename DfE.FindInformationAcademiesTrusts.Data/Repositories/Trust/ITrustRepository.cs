namespace DfE.FindInformationAcademiesTrusts.Data.Repositories.Trust;

public interface ITrustRepository
{ 
    Task<TrustOverview> GetTrustOverviewAsync(string trustReferenceNumber);
    Task<TrustContacts> GetTrustContactsAsync(string uid, string? urn = null);
    
}
