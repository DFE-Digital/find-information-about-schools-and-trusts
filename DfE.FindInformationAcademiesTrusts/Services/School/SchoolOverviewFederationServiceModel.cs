namespace DfE.FindInformationAcademiesTrusts.Services.School;

public record SchoolOverviewFederationServiceModel(
    string? FederationName,
    string? FederationUid,
    DateOnly? OpenedOnDate,
    Dictionary<string, string>? Schools);
