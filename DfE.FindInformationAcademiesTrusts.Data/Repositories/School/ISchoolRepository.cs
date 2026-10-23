namespace DfE.FindInformationAcademiesTrusts.Data.Repositories.School;

public interface ISchoolRepository
{
    Task<SchoolContact?> GetSchoolContactsAsync(int urn);
    
    Task<bool> IsPartOfFederationAsync(int urn);

    Task<FederationDetails> GetSchoolFederationDetailsAsync(int urn);
}
