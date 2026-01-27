namespace DfE.FindInformationAcademiesTrusts.Data.Repositories.ReportCards;

using DfE.FindInformationAcademiesTrusts.Data;

public interface IReportCardsRepository
{
    Task<(EstablishmentReportCard? LatestReportCard, EstablishmentReportCard? PreviousReportCard)> GetReportCardAsync(int urn);
}
