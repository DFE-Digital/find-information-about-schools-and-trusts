using DfE.FindInformationAcademiesTrusts.Data;

namespace DfE.FindInformationAcademiesTrusts.Extensions;

public static class SafeguardingScoreExtensions
{
    public static string ToDataSortValue(this SafeguardingScore rating)
    {
        return rating.ToDisplayString().ToLowerInvariant().Trim();
    }

    public static string ToDisplayString(this SafeguardingScore score)
    {
        return score switch
        {
            SafeguardingScore.NotInspected => "Not inspected",
            SafeguardingScore.Yes => "Yes",
            SafeguardingScore.No => "No",
            SafeguardingScore.NotRecorded => "Not recorded",
            _ => "Unknown"
        };
    }
}
