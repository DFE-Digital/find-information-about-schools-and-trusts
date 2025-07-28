namespace DfE.FindInformationAcademiesTrusts.Data;

public record OfstedFullInspectionSummary(DateTime? InspectionDate, OfstedRatingScore InspectionOutcome)
{
    public static readonly OfstedFullInspectionSummary Unknown = new(null, OfstedRatingScore.Unknown);
}
