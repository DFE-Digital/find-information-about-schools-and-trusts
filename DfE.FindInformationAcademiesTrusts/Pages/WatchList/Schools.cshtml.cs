using DfE.FindInformationAcademiesTrusts.Application.Watchlist.Models;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;

namespace DfE.FindInformationAcademiesTrusts.Pages.WatchList;

public class Schools : ContentPageModel
{
    public IReadOnlyList<EstablishmentWatchlistDto> Items { get; } = WatchListDummyData.Schools;

    public void OnGet()
    {

    }
}
