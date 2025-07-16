namespace DfE.FindInformationAcademiesTrusts.Data.Repositories.Ofsted;

public interface IOfstedRepository
{
    Task<AcademyOfsted[]> GetAcademiesInTrustOfstedAsync(string uid);
    Task<OfstedShortInspection> GetOfstedShortInspectionAsync(int urn);
}
