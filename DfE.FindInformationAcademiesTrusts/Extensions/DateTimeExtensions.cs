using DfE.FindInformationAcademiesTrusts.Pages;

namespace DfE.FindInformationAcademiesTrusts.Extensions;

public static class DateTimeExtensions
{
    public static string ShowDateStringOrReplaceWithText(this DateTime? date,
        string replacementText = ViewConstants.NoDataText)
    {
        if (date.HasValue)
        {
            return date.Value.ToString(StringFormatConstants.DisplayDateFormat);
        }

        return replacementText;
    }

    public static string ShowFullDateStringOrReplaceWithText(this DateTime? date,
        string replacementText = ViewConstants.NoDataText)
    {
        if (date.HasValue)
        {
            return date.Value.ToString(StringFormatConstants.DisplayFullDateFormat);
        }

        return replacementText;
    }

    public static string ToDataSortValue(this DateTime date)
    {
        return date.ToString(StringFormatConstants.SortableDateFormat);
    }

    public static string ToDataSortValue(this DateTime? date)
    {
        return date?.ToDataSortValue() ?? string.Empty;
    }
}
