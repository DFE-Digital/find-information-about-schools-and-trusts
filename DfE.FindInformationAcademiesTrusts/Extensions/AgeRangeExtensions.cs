using DfE.FindInformationAcademiesTrusts.Data;

namespace DfE.FindInformationAcademiesTrusts.Extensions;

public static class AgeRangeExtensions
{
    public static string ToFullDisplayString(this AgeRange ageRange)
    {
        if(ageRange.Minimum is null || ageRange.Maximum is null)
        {
            return "Not Available";
        }

        return $"{ageRange.Minimum} to {ageRange.Maximum}";
    }

    public static string ToTabularDisplayString(this AgeRange ageRange)
    {
        if (ageRange.Minimum is null || ageRange.Maximum is null)
        {
            return "Not Available";
        }

        return $"{ageRange.Minimum}-{ageRange.Maximum}";
    }

    public static string ToDataSortValue(this AgeRange? ageRange)
    {
        return ageRange is null ? "-1" : $"{ageRange.Minimum:D2}{ageRange.Maximum:D2}";
    }
}
