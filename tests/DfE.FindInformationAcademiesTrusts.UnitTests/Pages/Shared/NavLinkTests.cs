using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.NavMenu;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Shared;

public class NavLinkTests
{
    [Theory]
    [InlineData(null, true)]
    [InlineData(SchoolCategory.Academy, false)]
    [InlineData(SchoolCategory.LaMaintainedSchool, true)]
    public void ShowNavLink_should_be_set_from_school_category(SchoolCategory? schoolCategory, bool expected)
    {
        var navLink = new NavLink(
            false,
            null,
            "Test",
            "/Test",
            "Test",
            new Dictionary<string, string>(),
            schoolCategory);

        navLink.ShowNavLink.Should().Be(expected);
    }
}
