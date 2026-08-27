using DfE.FindInformationAcademiesTrusts.Pages.Shared;

namespace DfE.FindInformationAcademiesTrusts.Pages.WatchList;

public class Trusts : ContentPageModel
{
    public IReadOnlyList<WatchListEntry> Items { get; } = WatchListDummyData.Trusts;

    public string? CurrentUser { get; set; }

    
    public void OnGet()
    {
        CurrentUser = User.Identity?.Name;
    }
}
