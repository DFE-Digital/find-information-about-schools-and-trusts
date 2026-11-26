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
    public void InputId_LabelText_HintText_and_PlaceholderText_should_have_fixed_values()
    {
        var sut = new SearchModel();

        sut.PageSearchFormInputId.Should().Be("la-search");
        sut.LabelText.Should().Be("Search for a local authority ");
        sut.HintText.Should().Be("Search by LA code or name. Include any punctuation in names.");
        sut.PlaceholderText.Should().Be("");
    }
}
