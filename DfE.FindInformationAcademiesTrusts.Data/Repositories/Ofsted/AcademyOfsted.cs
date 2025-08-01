namespace DfE.FindInformationAcademiesTrusts.Data.Repositories.Ofsted;

public record AcademyOfsted(
    string Urn,
    string? EstablishmentName,
    DateTime? DateAcademyJoinedTrust,
    OfstedShortInspection ShortInspection,
    OfstedRating PreviousOfstedRating,
    OfstedRating CurrentOfstedRating
);
