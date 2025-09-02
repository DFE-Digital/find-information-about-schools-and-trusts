using DfE.FindInformationAcademiesTrusts.Data.Repositories.PupilCensus;

namespace DfE.FindInformationAcademiesTrusts.Extensions;

public static class StatisticDisplayExtensions
{
    public static string SortValue<T>(this Statistic<T> statistic)
    {
        return statistic.Stringify(
            "suppressed",
            "not-published",
            "not-applicable",
            "not-available",
            "not-yet-submitted"
        );
    }

    public static string DisplayValue<T>(this Statistic<T> statistic)
    {
        return statistic.Stringify(
            "Suppressed",
            "Not published",
            "Not applicable",
            "Not available",
            "Not yet submitted"
        );
    }

    public static string DisplayValueWithPercentage(this Statistic<int> absolute, Statistic<decimal> percentage) =>
        absolute.Compute(percentage, (count, percent) => $"{count} ({percent}%)").DisplayValue();

    private static string Stringify<T>(this Statistic<T> statistic, string suppressed, string notPublished,
        string notApplicable, string notAvailable, string notYetSubmitted)
    {
        if (statistic is Statistic<T>.WithValue v)
        {
            return v.Value?.ToString() ?? string.Empty;
        }

        return statistic.Kind switch
        {
            StatisticKind.Suppressed => suppressed,
            StatisticKind.NotPublished => notPublished,
            StatisticKind.NotApplicable => notApplicable,
            StatisticKind.NotAvailable => notAvailable,
            StatisticKind.NotYetSubmitted => notYetSubmitted,
            _ => throw new ArgumentException($"Unknown statistic kind '{statistic.Kind}'.")
        };
    }
}
