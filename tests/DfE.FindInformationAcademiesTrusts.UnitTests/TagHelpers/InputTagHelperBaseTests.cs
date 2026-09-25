using System.Reflection;
using Dfe.FindInformationAcademiesTrusts.TagHelpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.TagHelpers;

public class InputTagHelperBaseTests
{
    private readonly FakeHtmlHelper _fakeHtmlHelper = new();

    private sealed class TestInputTagHelper(IHtmlHelper htmlHelper) : InputTagHelperBase(htmlHelper)
    {
        public int RenderCount { get; private set; }
        public string? IdSeenDuringRender { get; private set; }
        public string? NameSeenDuringRender { get; private set; }

        protected override Task<IHtmlContent> RenderContentAsync()
        {
            RenderCount++;
            IdSeenDuringRender = Id;
            NameSeenDuringRender = Name;
            return Task.FromResult<IHtmlContent>(new HtmlString("<b>content</b>"));
        }
    }

    private TestInputTagHelper CreateSut() =>
        new(_fakeHtmlHelper.Helper) { ViewContext = TagHelperTestHelpers.CreateViewContext() };

    [Fact]
    public void Properties_ShouldHaveDefaultValues()
    {
        var sut = CreateSut();

        Assert.Equal(string.Empty, sut.Id);
        Assert.Equal(string.Empty, sut.Name);
        Assert.Equal(string.Empty, sut.Label);
        Assert.Equal(string.Empty, sut.LabelHint);
        Assert.Equal(string.Empty, sut.SubLabel);
        Assert.Equal(string.Empty, sut.PreviousInformation);
        Assert.Equal(string.Empty, sut.AdditionalInformation);
        Assert.Equal(string.Empty, sut.Suffix);
        Assert.Equal(string.Empty, sut.Hint);
        Assert.Equal(string.Empty, sut.DetailsHeading);
        Assert.Equal(string.Empty, sut.DetailsBody);
        Assert.Equal(string.Empty, sut.Heading);
        Assert.Equal(string.Empty, sut.HeadingStyle);
        Assert.Equal(string.Empty, sut.HeadingType);
        Assert.Equal(string.Empty, sut.ErrorMessage);
        Assert.Equal(string.Empty, sut.Email);
        Assert.False(sut.Inline);
    }

    [Fact]
    public void Properties_ShouldSetAndGetValues()
    {
        var modelExpression = TagHelperTestHelpers.CreateModelExpression("value");

        var sut = CreateSut();
        sut.Id = "id";
        sut.Name = "name";
        sut.Label = "label";
        sut.LabelHint = "label hint";
        sut.SubLabel = "sub label";
        sut.PreviousInformation = "previous";
        sut.AdditionalInformation = "additional";
        sut.Suffix = "suffix";
        sut.For = modelExpression;
        sut.Hint = "hint";
        sut.DetailsHeading = "details heading";
        sut.DetailsBody = "details body";
        sut.Heading = "heading";
        sut.HeadingStyle = "govuk-label--l";
        sut.HeadingType = "h1";
        sut.ErrorMessage = "error";
        sut.Email = "user@test.gov.uk";
        sut.Inline = true;

        Assert.Equal("id", sut.Id);
        Assert.Equal("name", sut.Name);
        Assert.Equal("label", sut.Label);
        Assert.Equal("label hint", sut.LabelHint);
        Assert.Equal("sub label", sut.SubLabel);
        Assert.Equal("previous", sut.PreviousInformation);
        Assert.Equal("additional", sut.AdditionalInformation);
        Assert.Equal("suffix", sut.Suffix);
        Assert.Same(modelExpression, sut.For);
        Assert.Equal("hint", sut.Hint);
        Assert.Equal("details heading", sut.DetailsHeading);
        Assert.Equal("details body", sut.DetailsBody);
        Assert.Equal("heading", sut.Heading);
        Assert.Equal("govuk-label--l", sut.HeadingStyle);
        Assert.Equal("h1", sut.HeadingType);
        Assert.Equal("error", sut.ErrorMessage);
        Assert.Equal("user@test.gov.uk", sut.Email);
        Assert.True(sut.Inline);
    }

    [Theory]
    [InlineData(nameof(InputTagHelperBase.Id), "id")]
    [InlineData(nameof(InputTagHelperBase.Name), "name")]
    [InlineData(nameof(InputTagHelperBase.Label), "label")]
    [InlineData(nameof(InputTagHelperBase.LabelHint), "label-hint")]
    [InlineData(nameof(InputTagHelperBase.SubLabel), "sub-label")]
    [InlineData(nameof(InputTagHelperBase.PreviousInformation), "previous-information")]
    [InlineData(nameof(InputTagHelperBase.AdditionalInformation), "additional-information")]
    [InlineData(nameof(InputTagHelperBase.Suffix), "suffix")]
    [InlineData(nameof(InputTagHelperBase.For), "asp-for")]
    [InlineData(nameof(InputTagHelperBase.Hint), "hint")]
    [InlineData(nameof(InputTagHelperBase.DetailsHeading), "details-heading")]
    [InlineData(nameof(InputTagHelperBase.DetailsBody), "details-body")]
    [InlineData(nameof(InputTagHelperBase.Heading), "heading")]
    [InlineData(nameof(InputTagHelperBase.HeadingStyle), "heading-style")]
    [InlineData(nameof(InputTagHelperBase.HeadingType), "heading-type")]
    [InlineData(nameof(InputTagHelperBase.ErrorMessage), "error-message")]
    [InlineData(nameof(InputTagHelperBase.Email), "email")]
    [InlineData(nameof(InputTagHelperBase.Inline), "inline")]
    public void Properties_ShouldMapToTheExpectedHtmlAttributeNames(string propertyName, string expectedAttributeName)
    {
        var property = typeof(InputTagHelperBase).GetProperty(propertyName);

        var attribute = property!.GetCustomAttribute<HtmlAttributeNameAttribute>();

        Assert.NotNull(attribute);
        Assert.Equal(expectedAttributeName, attribute.Name);
    }

    [Fact]
    public async Task ProcessAsync_WhenIdIsEmpty_ShouldDefaultIdToName()
    {
        var sut = CreateSut();
        sut.Name = "OrganisationType";

        await TagHelperTestHelpers.ProcessAsync(sut);

        Assert.Equal("OrganisationType", sut.Id);
        Assert.Equal("OrganisationType", sut.Name);
    }

    [Fact]
    public async Task ProcessAsync_WhenNameIsEmpty_ShouldDefaultNameToId()
    {
        var sut = CreateSut();
        sut.Id = "search-input";

        await TagHelperTestHelpers.ProcessAsync(sut);

        Assert.Equal("search-input", sut.Id);
        Assert.Equal("search-input", sut.Name);
    }

    [Fact]
    public async Task ProcessAsync_WhenIdAndNameAreSet_ShouldLeaveThemUnchanged()
    {
        var sut = CreateSut();
        sut.Id = "the-id";
        sut.Name = "the-name";

        await TagHelperTestHelpers.ProcessAsync(sut);

        Assert.Equal("the-id", sut.Id);
        Assert.Equal("the-name", sut.Name);
    }

    [Fact]
    public async Task ProcessAsync_WhenIdIsWhitespace_ShouldDefaultIdToName()
    {
        var sut = CreateSut();
        sut.Id = "   ";
        sut.Name = "the-name";

        await TagHelperTestHelpers.ProcessAsync(sut);

        Assert.Equal("the-name", sut.Id);
    }

    [Fact]
    public async Task ProcessAsync_WhenIdAndNameAreEmpty_ShouldLeaveThemEmpty()
    {
        var sut = CreateSut();

        await TagHelperTestHelpers.ProcessAsync(sut);

        Assert.Equal(string.Empty, sut.Id);
        Assert.Equal(string.Empty, sut.Name);
    }

    [Fact]
    public async Task ProcessAsync_ShouldDefaultIdAndNameBeforeRendering()
    {
        var sut = CreateSut();
        sut.Name = "the-name";

        await TagHelperTestHelpers.ProcessAsync(sut);

        Assert.Equal("the-name", sut.IdSeenDuringRender);
        Assert.Equal("the-name", sut.NameSeenDuringRender);
    }

    [Fact]
    public async Task ProcessAsync_ShouldContextualizeTheHtmlHelperWithTheViewContext()
    {
        var sut = CreateSut();

        await TagHelperTestHelpers.ProcessAsync(sut);

        _fakeHtmlHelper.ViewContextAware.Received(1).Contextualize(sut.ViewContext);
    }

    [Fact]
    public async Task ProcessAsync_WhenHtmlHelperIsNotViewContextAware_ShouldStillRender()
    {
        var plainHtmlHelper = Substitute.For<IHtmlHelper>();
        var sut = new TestInputTagHelper(plainHtmlHelper) { ViewContext = TagHelperTestHelpers.CreateViewContext() };

        var output = await TagHelperTestHelpers.ProcessAsync(sut);

        Assert.Equal(1, sut.RenderCount);
        Assert.Equal("<b>content</b>", output.PostContent.GetContent());
    }

    [Fact]
    public async Task ProcessAsync_ShouldRemoveTheOuterTagAndAppendTheRenderedContent()
    {
        var sut = CreateSut();

        var output = await TagHelperTestHelpers.ProcessAsync(sut);

        Assert.Null(output.TagName);
        Assert.Equal("<b>content</b>", output.PostContent.GetContent());
        Assert.Equal(1, sut.RenderCount);
    }
}
