using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Extensions;

namespace DfE.FindInformationAcademiesTrusts.Services.Academy;

public record SchoolOfstedServiceModel(
    string Urn,
    string? EstablishmentName,
    DateTime? DateAcademyJoinedTrust,
    OfstedShortInspection ShortInspection,
    OfstedRating? PreviousOfstedRating,
    OfstedRating? CurrentOfstedRating,
    bool IsFurtherEducationalEstablishment
)
{
    public bool HasRecentShortInspection => ShortInspection.InspectionDate > CurrentOfstedRating?.InspectionDate;

    public BeforeOrAfterJoining WhenDidCurrentInspectionHappen => DateAcademyJoinedTrust.GetBeforeOrAfterJoiningTrust(CurrentOfstedRating?.InspectionDate);

    public BeforeOrAfterJoining WhenDidPreviousInspectionHappen => DateAcademyJoinedTrust.GetBeforeOrAfterJoiningTrust(PreviousOfstedRating?.InspectionDate);
}
