using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Extensions;

namespace DfE.FindInformationAcademiesTrusts.Services.Academy;

public record SchoolOfstedServiceModel(
    string Urn,
    string? EstablishmentName,
    DateTime? DateEstablishmentJoinedGroup,
    OfstedShortInspection ShortInspection,
    OfstedRating? PreviousOfstedRating,
    OfstedRating? CurrentOfstedRating,
    bool IsFurtherEducationalEstablishment
)
{
    public bool HasRecentShortInspection => ShortInspection.InspectionDate > CurrentOfstedRating?.InspectionDate;

    public BeforeOrAfterJoining WhenDidCurrentInspectionHappen => DateEstablishmentJoinedGroup.GetBeforeOrAfterJoiningTrust(CurrentOfstedRating?.InspectionDate);

    public BeforeOrAfterJoining WhenDidPreviousInspectionHappen => DateEstablishmentJoinedGroup.GetBeforeOrAfterJoiningTrust(PreviousOfstedRating?.InspectionDate);
}