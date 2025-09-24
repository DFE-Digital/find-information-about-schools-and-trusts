namespace DfE.FindInformationAcademiesTrusts.Data.Repositories.PupilCensus;

public record Attendance(
    Statistic<decimal> OverallAbsencePercentage,
    Statistic<decimal> EnrolmentsWhoArePersistentAbsenteesPercentage
)
{
    public static readonly Attendance Unknown = new(
        Statistic<decimal>.NotAvailable,
        Statistic<decimal>.NotAvailable
    );

    public static readonly Attendance NotYetSubmitted = new(
        Statistic<decimal>.NotYetSubmitted,
        Statistic<decimal>.NotYetSubmitted
    );
}
