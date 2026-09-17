using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Dfe.FindInformationAcademiesTrusts.TagHelpers;

public abstract class InputTagHelperBase(IHtmlHelper htmlHelper) : TagHelper
{
    protected readonly IHtmlHelper _htmlHelper = htmlHelper;

    [HtmlAttributeName("id")]
    public string Id { get; set; } = string.Empty;

    [HtmlAttributeName("name")]
    public string Name { get; set; } = string.Empty;

    [HtmlAttributeName("label")]
    public string Label { get; set; } = string.Empty;

    [HtmlAttributeName("label-hint")]
    public string LabelHint { get; set; } = string.Empty;

    [HtmlAttributeName("sub-label")]
    public string SubLabel { get; set; } = string.Empty;

    [HtmlAttributeName("previous-information")]
    public string PreviousInformation { get; set; } = string.Empty;

    [HtmlAttributeName("additional-information")]
    public string AdditionalInformation { get; set; } = string.Empty;

    [HtmlAttributeName("suffix")]
    public string Suffix { get; set; } = string.Empty;

    [HtmlAttributeName("asp-for")]
    public ModelExpression For { get; set; } = null!;

    [HtmlAttributeName("hint")]
    public string Hint { get; set; } = string.Empty;

    [HtmlAttributeName("details-heading")]
    public string DetailsHeading { get; set; } = string.Empty;
    [HtmlAttributeName("details-body")]
    public string DetailsBody { get; set; } = string.Empty;

    [HtmlAttributeName("heading")]
    public string Heading { get; set; } = string.Empty;

    [HtmlAttributeName("heading-style")]
    public string HeadingStyle { get; set; } = string.Empty;
    [HtmlAttributeName("error-message")]
    public string ErrorMessage { get; set; } = string.Empty;
    
    [HtmlAttributeName("email")]
    public string Email { get; set; } = string.Empty;
    
    [HtmlAttributeName("inline")]
    public bool Inline { get; set; } = false;

    [ViewContext]
    public ViewContext ViewContext { get; set; } = null!;

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        if (_htmlHelper is IViewContextAware viewContextAware)
        {
            viewContextAware.Contextualize(ViewContext);
        }

        if (string.IsNullOrWhiteSpace(Id))
        {
            Id = Name;
        }

        if (string.IsNullOrWhiteSpace(Name))
        {
            Name = Id;
        }

        IHtmlContent content = await RenderContentAsync();
        output.TagName = null;
        output.PostContent.AppendHtml(content);
    }

    protected abstract Task<IHtmlContent> RenderContentAsync();
}
