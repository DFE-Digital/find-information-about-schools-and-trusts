namespace DfE.FindInformationAcademiesTrusts.Data
{
    public record EstablishmentReportCard(
        DateOnly InspectionDate,
        string? WebLink,
        string? CurriculumAndTeaching,
        string? AttendanceAndBehaviour,
        string? PersonalDevelopmentAndWellBeing,
        string? LeadershipAndGovernance,
        string? Inclusion,
        string? Achievement,
        string? EarlyYearsProvision,
        string? Safeguarding);
}
