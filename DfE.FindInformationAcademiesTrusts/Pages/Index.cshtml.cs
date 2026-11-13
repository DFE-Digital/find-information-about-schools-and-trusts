using DfE.FindInformationAcademiesTrusts.Pages.Shared;

namespace DfE.FindInformationAcademiesTrusts.Pages;

public class IndexModel : BasePageModel, IPageSearchFormModel
{
    public IndexModel()
    {
        ShowHeaderSearch = false;
    }

    public string PageSearchFormInputId => "home-search";
    
    public string LabelText => "Search for a trust or school ";
    public string HintText => "Search by URN (unique reference number), TRN (trust reference number) or name. Include any punctuation in names.";
    public string PlaceholderText => "e.g. st mary's primary school";

}
