using DfE.FindInformationAcademiesTrusts.Data.Enums;

namespace DfE.FindInformationAcademiesTrusts.Extensions;

public static class ContactRoleExtensions
{
    public static string MapRoleToViewString(this TrustContactRole role)
    {
        return role switch
        {
            TrustContactRole.TrustRelationshipManager => "Trust relationship manager",
            TrustContactRole.SfsoLead => "SFSO (Schools financial support and oversight) lead",
            _ => throw new ArgumentOutOfRangeException(nameof(role))
        };
    }

    public static string MapRoleToViewString(this SchoolContactRole role)
    {
        return role switch
        {
            SchoolContactRole.RegionsGroupLocalAuthorityLead => "Regions group local authority lead",
            _ => throw new ArgumentOutOfRangeException(nameof(role))
        };
    }
}
