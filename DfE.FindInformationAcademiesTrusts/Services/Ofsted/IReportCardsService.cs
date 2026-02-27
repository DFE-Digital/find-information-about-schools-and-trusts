namespace DfE.FindInformationAcademiesTrusts.Services.Ofsted;

using System.Threading.Tasks;

public interface IReportCardsService
{
    Task<ReportCardServiceModel> GetReportCardsAsync(int urn);
    Task<List<ReportCardServiceModel>> GetReportCardsAsync(List<string> urns);
}
