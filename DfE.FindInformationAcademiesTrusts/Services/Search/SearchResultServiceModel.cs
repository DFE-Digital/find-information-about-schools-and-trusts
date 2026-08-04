namespace DfE.FindInformationAcademiesTrusts.Services.Search;

public record SearchResultServiceModel(string Id,string ukprn, string Name, string Address, string? TrustGroupId, string Type, ResultType ResultType);

public record SearchResultsOverview(int NumberOfTrusts = 0, int NumberOfSchools = 0);