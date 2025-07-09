using DfE.FindInformationAcademiesTrusts.Pages.Shared;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Shared;

public class FormHelpersTests
{
    private readonly BasePageModel _mockPageModel = Substitute.For<BasePageModel>();

    [Fact]
    public void GetErrorClass_returns_the_class_string_when_validation_is_incorrect()
    {
        _mockPageModel.ModelState.AddModelError("Test", "Test");
        var result = _mockPageModel.GetErrorClass("Test");
        result.Should().Be("govuk-form-group--error");
    }

    [Theory]
    [InlineData("")]
    [InlineData("Name")]
    [InlineData("Email")]
    public void GetErrorClass_returns_empty_string_when_validation_is_correct(string key)
    {
        var result = _mockPageModel.GetErrorClass(key);
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void GenerateErrorAriaDescribedBy_returns_the_correct_string_when_validation_is_incorrect()
    {
        _mockPageModel.ModelState.AddModelError("Test", "Test");
        _mockPageModel.ModelState.AddModelError("Test", "Test1");
        var result = _mockPageModel.GenerateErrorAriaDescribedBy("Test");
        result.Should().Be("error-Test-0 error-Test-1");
    }

    [Fact]
    public void GenerateErrorAriaDescribedBy_returns_empty_when_validation_is_correct()
    {
        var result = _mockPageModel.GenerateErrorAriaDescribedBy("Test");
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void GetErrorList_returns_the_correct_list_when_validation_is_incorrect()
    {
        _mockPageModel.ModelState.AddModelError("Test", "Test");
        var result = _mockPageModel.GetErrorList("Test");
        result.Should().HaveCount(1);
        result.Should().BeEquivalentTo([(0, "Test")]);
    }

    [Fact]
    public void GetErrorList_returns_an_empty_list_when_validation_is_correct()
    {
        var result = _mockPageModel.GetErrorList("Test");
        result.Should().HaveCount(0);
        result.Should().BeEquivalentTo(Array.Empty<(int, string)>());
    }
}
