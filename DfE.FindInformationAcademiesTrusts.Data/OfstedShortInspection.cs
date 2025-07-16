namespace DfE.FindInformationAcademiesTrusts.Data;

public record OfstedShortInspection(DateTime? InspectionDate, string? InspectionOutcome)
{
    public static readonly OfstedShortInspection Unknown = new(null, null);
}
