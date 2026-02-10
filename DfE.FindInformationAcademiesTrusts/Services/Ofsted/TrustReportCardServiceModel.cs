namespace DfE.FindInformationAcademiesTrusts.Services.Ofsted
{
    public class TrustReportCardServiceModel
    {
        public ReportCardServiceModel? ReportCardDetails { get; set; } = null;

        public string SchoolName { get; set; } = string.Empty;
        public int Urn { get; set; }
    }
}
