namespace DfE.FindInformationAcademiesTrusts.Services.Trust;

public record TrustSummaryServiceModel(
    string Uid,
    string ReferenceNumber,
    string Name,
    string Type,
    int NumberOfAcademies);
