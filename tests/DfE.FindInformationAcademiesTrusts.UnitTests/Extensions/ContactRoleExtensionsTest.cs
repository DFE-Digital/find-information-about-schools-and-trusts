using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Extensions;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Extensions;

public class ContactRoleExtensionsTest
{
    [Fact]
    public void MapRoleToViewString_for_TrustContactRole_throws_when_mapping_is_invalid()
    {
        var sut = (TrustContactRole)9999;
        Assert.Throws<ArgumentOutOfRangeException>(() => sut.MapRoleToViewString());
    }

    [Fact]
    public void MapRoleToViewString_for_SchoolContactRole_throws_when_mapping_is_invalid()
    {
        var sut = (SchoolContactRole)9999;
        Assert.Throws<ArgumentOutOfRangeException>(() => sut.MapRoleToViewString());
    }
}
