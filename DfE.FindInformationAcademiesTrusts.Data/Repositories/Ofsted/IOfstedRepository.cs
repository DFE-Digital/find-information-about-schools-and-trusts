namespace DfE.FindInformationAcademiesTrusts.Data.Repositories.Ofsted;

public interface IOfstedRepository
{
    Task<SchoolOfsted[]> GetAcademiesInTrustOfstedAsync(string uid);
    Task<OfstedInspectionHistorySummary> GetOfstedInspectionHistorySummaryAsync(int urn);
    Task<OfstedShortInspection> GetOfstedShortInspectionAsync(int urn);
}
