namespace DfE.FindInformationAcademiesTrusts.Data.Repositories.School;

public record FederationDetails(string? FederationName, string? FederationUid)
{
    public DateTime? OpenedOnDate { get; set; }
    public Dictionary<string, string>? Schools { get; set; }
}
