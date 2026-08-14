namespace DfE.FindInformationAcademiesTrusts.Data.Repositories.School;

public record SchoolDetails(
    string Name,
    string Address,
    string Region,
    string LocalAuthority,
    string PhaseOfEducationName,
    AgeRange AgeRange,
    string? NurseryProvision,
    string? TrustName,
    string? DateJoinedTrust);
