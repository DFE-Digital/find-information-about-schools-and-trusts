using System.Reflection;
using Dfe.FindInformationAcademiesTrusts.TagHelpers;
using Dfe.FindInformationAcademiesTrusts.ViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.TagHelpers;

public class RadioButtonsInputTagHelperTests
{
    private readonly FakeHtmlHelper _fakeHtmlHelper = new();

    private RadioButtonsInputTagHelper CreateSut(string? modelValue = "Establishment") =>
        new(_fakeHtmlHelper.Helper)
        {
            Name = "OrganisationType",
            For = TagHelperTestHelpers.CreateModelExpression(modelValue, "OrganisationType"),
            ViewContext = TagHelperTestHelpers.CreateViewContext()
        };

    [Fact]
    public void RadioButtonsInputTagHelper_ShouldHaveDefaultValues()
    {
        var sut = CreateSut();

        Assert.Empty(sut.RadioButtons);
        Assert.False(sut.HasError);
        Assert.Null(sut.ErrorMessage);
    }

    [Fact]
    public void RadioButtonsInputTagHelper_ShouldTargetTheRadioButtonsElementWithoutAnEndTag()
    {
        var attribute = typeof(RadioButtonsInputTagHelper).GetCustomAttribute<HtmlTargetElementAttribute>();

        Assert.NotNull(attribute);
        Assert.Equal("govuk-radiobuttons-input", attribute.Tag);
        Assert.Equal(TagStructure.WithoutEndTag, attribute.TagStructure);
    }

    [Fact]
    public async Task ProcessAsync_ShouldRenderTheRadioButtonsPartial()
    {
        var sut = CreateSut();

        var output = await TagHelperTestHelpers.ProcessAsync(sut);

        Assert.Equal("_RadioButtons", _fakeHtmlHelper.PartialName);
        Assert.Null(output.TagName);
        Assert.Equal(FakeHtmlHelper.RenderedHtml, output.PostContent.GetContent());
    }

    [Fact]
    public async Task ProcessAsync_ShouldMapTheTagHelperPropertiesOntoTheViewModel()
    {
        var radioButtons = new List<RadioButtonsLabelViewModel>
        {
            new() { Name = "School", Id = "school", Value = "School" },
            new() { Name = "Trust", Id = "trust", Value = "Trust" }
        };

        var sut = CreateSut("Trust");
        sut.Heading = "Select the type of establishment to add";
        sut.HeadingStyle = "govuk-label govuk-label--l";
        sut.Hint = "Select one option";
        sut.ErrorMessage = "Select an option";
        sut.Inline = true;
        sut.RadioButtons = radioButtons;

        await TagHelperTestHelpers.ProcessAsync(sut);

        var model = Assert.IsType<RadioButtonViewModel>(_fakeHtmlHelper.Model);
        Assert.Equal("OrganisationType", model.Name);
        Assert.Equal("Select the type of establishment to add", model.Heading);
        Assert.Equal("govuk-label govuk-label--l", model.HeadingStyle);
        Assert.Equal("Select one option", model.Hint);
        Assert.Equal("Select an option", model.ErrorMessage);
        Assert.True(model.Inline);
        Assert.Equal("Trust", model.Value);
        Assert.Same(radioButtons, model.RadioButtons);
    }

    [Fact]
    public async Task ProcessAsync_WhenNoNameIsSet_ShouldUseTheIdAsTheViewModelName()
    {
        var sut = CreateSut();
        sut.Name = string.Empty;
        sut.Id = "radio-group";

        await TagHelperTestHelpers.ProcessAsync(sut);

        var model = Assert.IsType<RadioButtonViewModel>(_fakeHtmlHelper.Model);
        Assert.Equal("radio-group", model.Name);
    }

    [Fact]
    public async Task ProcessAsync_WhenTheBoundModelIsNull_ShouldSetTheViewModelValueToNull()
    {
        var sut = CreateSut(null);

        await TagHelperTestHelpers.ProcessAsync(sut);

        var model = Assert.IsType<RadioButtonViewModel>(_fakeHtmlHelper.Model);
        Assert.Null(model.Value);
    }

    [Fact]
    public async Task ProcessAsync_WithNoErrorMessage_ShouldLeaveTheViewModelErrorMessageNull()
    {
        var sut = CreateSut();

        await TagHelperTestHelpers.ProcessAsync(sut);

        var model = Assert.IsType<RadioButtonViewModel>(_fakeHtmlHelper.Model);
        Assert.Null(model.ErrorMessage);
    }

    [Fact]
    public async Task ProcessAsync_ShouldContextualizeTheHtmlHelper()
    {
        var sut = CreateSut();

        await TagHelperTestHelpers.ProcessAsync(sut);

        _fakeHtmlHelper.ViewContextAware.Received(1).Contextualize(sut.ViewContext);
    }
}
