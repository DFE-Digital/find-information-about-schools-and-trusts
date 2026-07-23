namespace DfE.FindInformationAcademiesTrusts.Options;

public class AcademiesApiOptions
{
    public const string ConfigurationSection = "AcademiesApi";

    public string? Key { get; init; }
    public string? Url { get; init; }
}
