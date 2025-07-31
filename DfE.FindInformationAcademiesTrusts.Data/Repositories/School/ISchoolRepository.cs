namespace DfE.FindInformationAcademiesTrusts.Data.Repositories.School;

public interface ISchoolRepository
{
    Task<SchoolSummary?> GetSchoolSummaryAsync(int urn);

    Task<SchoolDetails> GetSchoolDetailsAsync(int urn);

    Task<DateOnly?> GetDateJoinedTrustAsync(int urn);

    Task<SchoolContact?> GetSchoolContactsAsync(int urn);

    Task<SenProvision> GetSchoolSenProvisionAsync(int urn);

    Task<bool> IsPartOfFederationAsync(int urn);

    Task<FederationDetails> GetSchoolFederationDetailsAsync(int urn);

    Task<SchoolReferenceNumbers?> GetReferenceNumbersAsync(int urn);

    Task<Governor[]> GetGovernanceAsync(int urn);
}
