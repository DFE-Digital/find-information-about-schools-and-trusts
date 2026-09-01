using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Dfe.FindInformationAcademiesTrusts.ViewModels;

public class RadioButtonViewModel
{
    public string? Heading { get; set; } = null;
    public string? HeadingStyle { get; set; } = null;
    public string? Hint { get; set; } = null;
    public string? ErrorMessage { get; set; } = null;
    public string? Name { get; set; } = null;
    public IList<RadioButtonsLabelViewModel> RadioButtons { get; set; } = [];
    public string? Value { get; set; } = null;
    public bool Inline { get; set; } = false;
}
public class RadioButtonsLabelViewModel
{
    public required string Name { get; set; }
    public required string Id { get; set; }
    public string? Value { get; set; }

    public string? Hint { get; set; }
    public TextFieldInputViewModel? Input { get; set; }
    public bool? DisplayAsOr { get; set; } = false;
}
public class TextFieldInputViewModel
{
    public required string Id { get; set; }
    public bool IsValid { get; set; } = true;
    public required string ValidationMessage { get; set; }
    public required string Paragraph { get; set; }
    [HtmlAttributeName("Value")]
    public required string? Value { get; set; }
    public bool IsTextArea { get; set; } = true;
}
