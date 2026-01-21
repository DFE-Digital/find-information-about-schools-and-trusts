namespace DfE.FindInformationAcademiesTrusts.Services.Ofsted;

public interface IReportCardsService
{
    Task<ReportCardServiceModel> GetReportCardsAsync(int urn);
}
