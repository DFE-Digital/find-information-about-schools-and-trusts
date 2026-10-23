namespace DfE.FindInformationAcademiesTrusts.Data.Repositories.Trust;

public interface ITrustRepository
{ 
    Task<TrustContacts> GetTrustContactsAsync(string uid, string? urn = null);
}
