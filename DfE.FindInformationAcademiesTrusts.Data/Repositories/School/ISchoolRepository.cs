namespace DfE.FindInformationAcademiesTrusts.Data.Repositories.School;

public interface ISchoolRepository
{
    Task<SchoolContact?> GetSchoolContactsAsync(int urn);

    Task<SenProvision> GetSchoolSenProvisionAsync(int urn);

    Task<bool> IsPartOfFederationAsync(int urn);

    Task<FederationDetails> GetSchoolFederationDetailsAsync(int urn);

    Task<SchoolReferenceNumbers?> GetReferenceNumbersAsync(int urn);

    Task<List<Governor>> GetGovernanceAsync(int urn);

    Task<ReligiousCharacteristics> GetReligiousCharacteristicsAsync(int urn);
}
