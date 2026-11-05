using DfE.FindInformationAcademiesTrusts.Pages.LocalAuthorities;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.LocalAuthorities;

public class SearchModelTests
{
    [Fact]
    public void KeyWords_property_is_empty_by_default()
    {
        var sut = new SearchModel();
        sut.KeyWords.Should().Be("");
    }

    [Fact]
    public void InputId_should_have_a_fixed_value()
    {
        var sut = new SearchModel();

        sut.PageSearchFormInputId.Should().Be("la-search");
    }
}
