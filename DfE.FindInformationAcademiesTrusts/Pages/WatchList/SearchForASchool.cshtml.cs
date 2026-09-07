using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DfE.FindInformationAcademiesTrusts.Pages.WatchList;

public class SearchForASchool : ContentPageModel,IEstablishmentSearchFormModel
{
    
    public void OnGet()
    {
        
    }

    public string PageSearchFormInputId => "school-search";
}
