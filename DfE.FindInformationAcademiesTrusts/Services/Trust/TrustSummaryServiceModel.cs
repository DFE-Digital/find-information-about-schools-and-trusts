namespace DfE.FindInformationAcademiesTrusts.Services.Trust;

public record TrustSummaryServiceModel(
    string Uid,
    string Ukprn,
    string Name,
    string Type,
    int NumberOfAcademies);
