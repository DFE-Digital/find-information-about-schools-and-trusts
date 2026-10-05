namespace DfE.FindInformationAcademiesTrusts.Data.Repositories.School;

public record FederationDetails(string? FederationName, string? FederationUid)
{
    public DateOnly? OpenedOnDate { get; set; }
    public Dictionary<string, string>? Schools { get; set; }
}
