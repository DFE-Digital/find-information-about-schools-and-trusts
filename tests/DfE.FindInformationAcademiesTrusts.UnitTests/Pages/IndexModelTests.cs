using DfE.FindInformationAcademiesTrusts.Pages;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages;

public class IndexModelTests
{
    [Fact]
    public void KeyWords_property_is_empty_by_default()
    {
        var sut = new IndexModel();
        sut.KeyWords.Should().Be("");
    }

    [Fact]
    public void InputId_LabelText_HintText_and_PlaceholderText_should_have_fixed_values()
    {
        var sut = new IndexModel();

        sut.PageSearchFormInputId.Should().Be("home-search");
        sut.LabelText.Should().Be("Search for a trust or school ");
        sut.HintText.Should().Be("Search by URN (unique reference number), TRN (trust reference number) or name. Include any punctuation in names.");
        sut.PlaceholderText.Should().Be("e.g. st mary's primary school");
    }
}
