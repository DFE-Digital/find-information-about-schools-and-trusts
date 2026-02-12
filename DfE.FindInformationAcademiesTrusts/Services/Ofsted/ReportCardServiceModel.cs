namespace DfE.FindInformationAcademiesTrusts.Services.Ofsted
{
    public class ReportCardServiceModel: IOfstedInspection
    {
        public ReportCardDetails? LatestReportCard { get; set; }
        public ReportCardDetails? PreviousReportCard { get; set; }
        public DateOnly? DateJoinedTrust { get; set; }
    }

    public record ReportCardDetails(
        DateOnly InspectionDate,
        string? WebLink,
        string? CurriculumAndTeaching,
        string? AttendanceAndBehaviour,
        string? PersonalDevelopmentAndWellBeing,
        string? LeadershipAndGovernance,
        string? Inclusion,
        string? Achievement,
        string? EarlyYearsProvision,
        string? Safeguarding,
        string? Post16Provision, 
        string? CategoryOfConcern);
}
