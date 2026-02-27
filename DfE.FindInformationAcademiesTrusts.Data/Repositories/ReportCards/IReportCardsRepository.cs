namespace DfE.FindInformationAcademiesTrusts.Data.Repositories.ReportCards;

public interface IReportCardsRepository
{
    Task<ReportCardData> GetReportCardAsync(int urn);
    Task<List<ReportCardData>> GetReportCardsAsync(List<int> urns);
}
