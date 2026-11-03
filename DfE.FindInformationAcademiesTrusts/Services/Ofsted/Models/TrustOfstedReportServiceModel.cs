namespace DfE.FindInformationAcademiesTrusts.Services.Ofsted
{
    public class TrustOfstedReportServiceModel<T> where T: IOfstedInspection
    {
        public required T ReportDetails { get; set; }

        public required string SchoolName { get; set; }
        public int Urn { get; set; }
    }
}
