namespace DfE.FindInformationAcademiesTrusts.Data.Repositories.Ofsted;

public record SchoolOfsted(
    string Urn,
    string? EstablishmentName,
    DateTime? DateAcademyJoinedTrust,
    OfstedShortInspection ShortInspection,
    OfstedRating PreviousOfstedRating,
    OfstedRating CurrentOfstedRating,
    bool IsFurtherEducationalEstablishment
);
