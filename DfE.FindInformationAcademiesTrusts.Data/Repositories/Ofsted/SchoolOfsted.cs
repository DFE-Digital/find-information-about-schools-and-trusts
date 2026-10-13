namespace DfE.FindInformationAcademiesTrusts.Data.Repositories.Ofsted;

public record SchoolOfsted(
    string Urn,
    string? EstablishmentName,
    DateTime? DateEstablishmentJoinedGroup,
    OfstedShortInspection ShortInspection,
    OfstedRating PreviousOfstedRating,
    OfstedRating CurrentOfstedRating,
    bool IsFurtherEducationalEstablishment
);
