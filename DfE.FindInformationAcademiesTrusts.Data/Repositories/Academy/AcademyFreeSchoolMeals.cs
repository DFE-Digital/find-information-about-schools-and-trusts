namespace DfE.FindInformationAcademiesTrusts.Data.Repositories.Academy;

public record AcademyFreeSchoolMeals(
    string Urn,
    string? EstablishmentName,
    double? PercentageFreeSchoolMeals,
    int? LocalAuthorityCode,
    string? TypeOfEstablishment,
    string? PhaseOfEducation
);
