using DfE.FindInformationAcademiesTrusts.Data;

namespace DfE.FindInformationAcademiesTrusts.Services.School;

    public record OlderSchoolOfstedServiceModel(string Urn,
        string? EstablishmentName,
        DateTime? DateAcademyJoinedTrust,
        OfstedShortInspection ShortInspection,
        List<OfstedRating> RatingsWithSingleHeadlineGrade,
        List<OfstedRating> RatingsWithoutSingleHeadlineGrade,
        bool IsFurtherEducationalEstablishment);

