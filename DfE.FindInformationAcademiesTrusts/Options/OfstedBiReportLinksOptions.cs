namespace DfE.FindInformationAcademiesTrusts.Options
{
    public class OfstedBiReportLinksOptions
    {
        public const string ConfigurationSection = "OfstedBiReportLinks";

        public string? ReportCardsBaseUrl { get; init; }

        public string? OfstedPublishedBaseUrl { get; init; }
    }
}
