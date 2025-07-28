namespace DfE.FindInformationAcademiesTrusts.Data;

public record OfstedInspectionHistorySummary(
    OfstedFullInspectionSummary CurrentInspection,
    OfstedFullInspectionSummary PreviousInspection)
{
    public static readonly OfstedInspectionHistorySummary Unknown = new(
        OfstedFullInspectionSummary.Unknown,
        OfstedFullInspectionSummary.Unknown);
}
