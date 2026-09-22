using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.TagHelpers;

public sealed class FakeHtmlHelper
{
    public const string RenderedHtml = "<p>rendered</p>";

    public FakeHtmlHelper()
    {
        Helper = Substitute.For<IHtmlHelper, IViewContextAware>();
        Helper.PartialAsync(Arg.Any<string>(), Arg.Any<object?>(), Arg.Any<ViewDataDictionary?>())
            .Returns(callInfo =>
            {
                PartialName = callInfo.ArgAt<string>(0);
                Model = callInfo.ArgAt<object?>(1);
                return Task.FromResult<IHtmlContent>(new HtmlString(RenderedHtml));
            });
    }

    public IHtmlHelper Helper { get; }
    public string? PartialName { get; private set; }
    public object? Model { get; private set; }

    public IViewContextAware ViewContextAware => (IViewContextAware)Helper;
}

public static class TagHelperTestHelpers
{
    public static ViewContext CreateViewContext(ModelStateDictionary? modelState = null)
    {
        modelState ??= new ModelStateDictionary();

        var actionContext = new ActionContext(
            new DefaultHttpContext(),
            new RouteData(),
            new ActionDescriptor(),
            modelState);

        return new ViewContext(
            actionContext,
            Substitute.For<IView>(),
            new ViewDataDictionary(new EmptyModelMetadataProvider(), modelState),
            Substitute.For<ITempDataDictionary>(),
            TextWriter.Null,
            new HtmlHelperOptions());
    }

    public static ModelExpression CreateModelExpression(object? model, string name = "Property")
    {
        var provider = new EmptyModelMetadataProvider();
        var explorer = provider.GetModelExplorerForType(model?.GetType() ?? typeof(string), model);
        return new ModelExpression(name, explorer);
    }

    public static async Task<TagHelperOutput> ProcessAsync(TagHelper tagHelper)
    {
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N"));

        var output = new TagHelperOutput(
            "govuk-input",
            new TagHelperAttributeList(),
            (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        await tagHelper.ProcessAsync(context, output);

        return output;
    }
}
