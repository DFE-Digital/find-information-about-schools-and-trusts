namespace DfE.FindInformationAcademiesTrusts.Services.School;

public record SchoolOverviewFederationServiceModel(
    string? FederationName,
    string? FederationUid,
    DateTime? OpenedOnDate,
    Dictionary<string, string>? Schools);
