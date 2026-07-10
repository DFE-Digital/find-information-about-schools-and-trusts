namespace DfE.FindInformationAcademiesTrusts.Data.Repositories.Search;

public record SearchResult(
    string Id,
    string ukprn,
    string Name,
    string Type,
    string Address,
    bool IsTrust,
    string? TrustReferenceNumber);
