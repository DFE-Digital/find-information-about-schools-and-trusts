namespace DfE.FindInformationAcademiesTrusts.Pages.Shared;

public interface IPageSearchFormModel
{
    string? KeyWords { get; set; }
    string PageSearchFormInputId { get; }
    string LabelText { get; }
    string HintText { get; }
    string PlaceholderText { get; }
}
