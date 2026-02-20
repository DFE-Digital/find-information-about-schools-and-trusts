namespace DfE.FindInformationAcademiesTrusts.Data.Repositories.ReportCards
{
    public class ReportCardData
    {
        public int Urn { get; set; }

        public EstablishmentReportCard? LatestReportCard { get; set; }

        public EstablishmentReportCard? PreviousReportCard { get; set; }
    }
}
