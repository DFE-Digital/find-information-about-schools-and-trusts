namespace DfE.FindInformationAcademiesTrusts.Services.Ofsted
{
    public class TrustOfstedReportServiceModel<T> where T: IOfstedInspection
    {
        public required T ReportDetails { get; set; }

        public string SchoolName { get; set; } = string.Empty;
        public int Urn { get; set; }
    }
}
