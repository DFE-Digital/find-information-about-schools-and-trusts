using System.Reflection;
using Dfe.FindInformationAcademiesTrusts.TagHelpers;
using Dfe.FindInformationAcademiesTrusts.ViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.TagHelpers;

public class TextInputTagHelperTests
{
    private readonly FakeHtmlHelper _fakeHtmlHelper = new();

    private TextInputTagHelper CreateSut(string? modelValue = "Test School", ModelStateDictionary? modelState = null) =>
        new(_fakeHtmlHelper.Helper)
        {
            Name = "SearchTerm",
            For = TagHelperTestHelpers.CreateModelExpression(modelValue, "SearchTerm"),
            ViewContext = TagHelperTestHelpers.CreateViewContext(modelState)
        };

    [Fact]
    public void TextInputTagHelper_ShouldHaveDefaultValues()
    {
        var sut = CreateSut();

        Assert.Equal(0, sut.Width);
        Assert.False(sut.HeadingLabel);
        Assert.Equal(string.Empty, sut.HeadingLabelClass);
    }

    [Fact]
    public void TextInputTagHelper_ShouldTargetTheTextInputElementWithoutAnEndTag()
    {
        var attribute = typeof(TextInputTagHelper).GetCustomAttribute<HtmlTargetElementAttribute>();

        Assert.NotNull(attribute);
        Assert.Equal("govuk-text-input", attribute.Tag);
        Assert.Equal(TagStructure.WithoutEndTag, attribute.TagStructure);
    }

    [Theory]
    [InlineData(nameof(TextInputTagHelper.Width), "width")]
    [InlineData(nameof(TextInputTagHelper.HeadingLabelClass), "heading-label-class")]
    public void TextInputTagHelper_ShouldMapToTheExpectedHtmlAttributeNames(string propertyName, string expectedAttributeName)
    {
        var property = typeof(TextInputTagHelper).GetProperty(propertyName);

        var attribute = property!.GetCustomAttribute<HtmlAttributeNameAttribute>();

        Assert.NotNull(attribute);
        Assert.Equal(expectedAttributeName, attribute.Name);
    }

    [Fact]
    public async Task ProcessAsync_ShouldRenderTheTextInputPartial()
    {
        var sut = CreateSut();

        var output = await TagHelperTestHelpers.ProcessAsync(sut);

        Assert.Equal("_TextInput", _fakeHtmlHelper.PartialName);
        Assert.Null(output.TagName);
        Assert.Equal(FakeHtmlHelper.RenderedHtml, output.PostContent.GetContent());
    }

    [Fact]
    public async Task ProcessAsync_ShouldMapTheTagHelperPropertiesOntoTheViewModel()
    {
        var sut = CreateSut("Oakwood Primary");
        sut.Id = "search-input";
        sut.Label = "Search for a school";
        sut.Width = 20;
        sut.Hint = "For example, Oakwood Primary";
        sut.HeadingLabel = true;
        sut.HeadingLabelClass = "govuk-label--l";

        await TagHelperTestHelpers.ProcessAsync(sut);

        var model = Assert.IsType<TextInputViewModel>(_fakeHtmlHelper.Model);
        Assert.Equal("search-input", model.Id);
        Assert.Equal("SearchTerm", model.Name);
        Assert.Equal("Search for a school", model.Label);
        Assert.Equal("Oakwood Primary", model.Value);
        Assert.Equal(20, model.Width);
        Assert.Equal("For example, Oakwood Primary", model.Hint);
        Assert.True(model.HeadingLabel);
        Assert.Equal("govuk-label--l", model.HeadingLabelClass);
    }

    [Fact]
    public async Task ProcessAsync_WhenNoIdIsSet_ShouldUseTheNameAsTheViewModelId()
    {
        var sut = CreateSut();

        await TagHelperTestHelpers.ProcessAsync(sut);

        var model = Assert.IsType<TextInputViewModel>(_fakeHtmlHelper.Model);
        Assert.Equal("SearchTerm", model.Id);
    }

    [Fact]
    public async Task ProcessAsync_WhenTheBoundModelIsNull_ShouldSetTheViewModelValueToNull()
    {
        var sut = CreateSut(null);

        await TagHelperTestHelpers.ProcessAsync(sut);

        var model = Assert.IsType<TextInputViewModel>(_fakeHtmlHelper.Model);
        Assert.Null(model.Value);
    }

    [Fact]
    public async Task ProcessAsync_WithNoModelStateErrors_ShouldLeaveTheErrorMessageNull()
    {
        var sut = CreateSut();

        await TagHelperTestHelpers.ProcessAsync(sut);

        var model = Assert.IsType<TextInputViewModel>(_fakeHtmlHelper.Model);
        Assert.Null(model.ErrorMessage);
    }

    [Fact]
    public async Task ProcessAsync_WithAModelStateErrorForTheName_ShouldSetTheErrorMessage()
    {
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("SearchTerm", "Enter a school name");

        var sut = CreateSut(modelState: modelState);

        await TagHelperTestHelpers.ProcessAsync(sut);

        var model = Assert.IsType<TextInputViewModel>(_fakeHtmlHelper.Model);
        Assert.Equal("Enter a school name", model.ErrorMessage);
    }

    [Fact]
    public async Task ProcessAsync_WithMultipleModelStateErrorsForTheName_ShouldUseTheFirstErrorMessage()
    {
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("SearchTerm", "First error");
        modelState.AddModelError("SearchTerm", "Second error");

        var sut = CreateSut(modelState: modelState);

        await TagHelperTestHelpers.ProcessAsync(sut);

        var model = Assert.IsType<TextInputViewModel>(_fakeHtmlHelper.Model);
        Assert.Equal("First error", model.ErrorMessage);
    }

    [Fact]
    public async Task ProcessAsync_WithAModelStateErrorForADifferentKey_ShouldLeaveTheErrorMessageNull()
    {
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("SomethingElse", "Not for this field");

        var sut = CreateSut(modelState: modelState);

        await TagHelperTestHelpers.ProcessAsync(sut);

        var model = Assert.IsType<TextInputViewModel>(_fakeHtmlHelper.Model);
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
