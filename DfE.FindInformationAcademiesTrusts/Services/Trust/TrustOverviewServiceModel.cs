using DfE.FindInformationAcademiesTrusts.Data.Enums;

namespace DfE.FindInformationAcademiesTrusts.Services.Trust;

public record TrustOverviewServiceModel(
    string Uid,
    string GroupId,
    string? Ukprn,
    string? CompaniesHouseNumber,
    TrustType Type,
    string Address,
    string RegionAndTerritory,
    string? SingleAcademyTrustAcademyUrn,
    DateTime? OpenedDate,
    int TotalAcademies,
    IReadOnlyDictionary<string, int> AcademiesByLocalAuthority,
    int TotalPupilNumbers,
    int TotalCapacity,
    bool HasIncompleteCapacityData)
{
    public int? PercentageFull =>
        !HasIncompleteCapacityData && TotalCapacity > 0
            ? (int)Math.Round((double)TotalPupilNumbers / TotalCapacity * 100)
            : null;
}
