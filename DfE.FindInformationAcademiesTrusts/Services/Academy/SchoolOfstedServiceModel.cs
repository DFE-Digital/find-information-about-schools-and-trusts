using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;

namespace DfE.FindInformationAcademiesTrusts.Services.Academy;

public record SchoolOfstedServiceModel(
    string Urn,
    string? EstablishmentName,
    DateTime? DateAcademyJoinedTrust,
    OfstedShortInspection ShortInspection,
    OfstedRating PreviousOfstedRating,
    OfstedRating CurrentOfstedRating
)
{
    public bool HasRecentShortInspection => ShortInspection.InspectionDate > CurrentOfstedRating.InspectionDate;

    public BeforeOrAfterJoining WhenDidCurrentInspectionHappen
    {
        get
        {
            if (DateAcademyJoinedTrust is null)
            {
                return BeforeOrAfterJoining.NotApplicable;
            }

            if (CurrentOfstedRating.InspectionDate is null)
            {
                return BeforeOrAfterJoining.NotYetInspected;
            }

            if (CurrentOfstedRating.InspectionDate >= DateAcademyJoinedTrust)
            {
                return BeforeOrAfterJoining.After;
            }

            // Must be CurrentOfstedRating.InspectionDate < DateAcademyJoinedTrust by process of elimination 

            return BeforeOrAfterJoining.Before;
        }
    }

    public BeforeOrAfterJoining WhenDidPreviousInspectionHappen
    {
        get
        {
            if (DateAcademyJoinedTrust is null)
            {
                return BeforeOrAfterJoining.NotApplicable;
            }

            if (PreviousOfstedRating.InspectionDate is null)
            {
                return BeforeOrAfterJoining.NotYetInspected;
            }

            if (PreviousOfstedRating.InspectionDate >= DateAcademyJoinedTrust)
            {
                return BeforeOrAfterJoining.After;
            }

            // Must be PreviousOfstedRating.InspectionDate < DateAcademyJoinedTrust by process of elimination 

            return BeforeOrAfterJoining.Before;
        }
    }
}
