using DfE.FindInformationAcademiesTrusts.Data;

namespace DfE.FindInformationAcademiesTrusts.Services.School;

public record SchoolOverviewServiceModel(
    string Name,
    string Address,
    string Region,
    string LocalAuthority,
    string PhaseOfEducationName,
    AgeRange AgeRange,
    NurseryProvision NurseryProvision,
    string? TrustName,
    DateTime? DateJoinedTrust)
{
}
