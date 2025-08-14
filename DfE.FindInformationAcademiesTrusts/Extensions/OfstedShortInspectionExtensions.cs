using DfE.FindInformationAcademiesTrusts.Data;

namespace DfE.FindInformationAcademiesTrusts.Extensions;

public static class OfstedShortInspectionExtensions
{
    public static string ToOutcomeDisplayString(this OfstedShortInspection shortInspection) =>
        shortInspection.InspectionOutcome switch
        {
            null => "Not available",
            var o when o.Trim() == string.Empty => "Not available",
            var o when o.Contains('-') => o.Split('-')[0].Trim(),
            _ => shortInspection.InspectionOutcome
        };
}
