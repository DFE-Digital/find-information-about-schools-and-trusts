using DfE.FindInformationAcademiesTrusts.Pages.Shared;

namespace DfE.FindInformationAcademiesTrusts.Pages.LocalAuthorities;

public class SearchModel : BasePageModel, IPageSearchFormModel
{
    public SearchModel()
    {
        ShowHeaderSearch = false;
    }

    public string PageSearchFormInputId => "la-search";
    
    public string LabelText => "Search for a local authority ";
    public string HintText => "Search by LA code or name. Include any punctuation in names.";
    public string PlaceholderText => "";
}
