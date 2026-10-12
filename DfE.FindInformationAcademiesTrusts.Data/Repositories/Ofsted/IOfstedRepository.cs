namespace DfE.FindInformationAcademiesTrusts.Data.Repositories.Ofsted;

public interface IOfstedRepository
{
    Task<SchoolOfsted[]> GetAcademiesInTrustOfstedAsync(string trustReferenceNumber);
    Task<SchoolOfsted> GetSchoolOfstedRatingsAsync(int urn);
}
