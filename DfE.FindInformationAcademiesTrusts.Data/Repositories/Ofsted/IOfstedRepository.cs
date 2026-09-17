namespace DfE.FindInformationAcademiesTrusts.Data.Repositories.Ofsted;

public interface IOfstedRepository
{
    Task<SchoolOfsted[]> GetAcademiesInTrustOfstedAsync(string trustReferenceNumber);
    Task<OfstedInspectionHistorySummary> GetOfstedInspectionHistorySummaryAsync(int urn);
    Task<OfstedShortInspection> GetOfstedShortInspectionAsync(int urn);
    Task<SchoolOfsted> GetSchoolOfstedRatingsAsync(int urn);
}
