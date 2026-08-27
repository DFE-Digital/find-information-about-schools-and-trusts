using Dfe.AcademiesApi.Client.Contracts;
using DfE.FindInformationAcademiesTrusts.Application.Watchlist.Models;
using DfE.FindInformationAcademiesTrusts.Application.Watchlist.Queries;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;

namespace DfE.FindInformationAcademiesTrusts.Pages.WatchList;

public class Index(IWatchlistQueryService watchlistQueryService) : ContentPageModel
{
    public IReadOnlyList<EstablishmentWatchlistDto> Items { get; } = WatchListDummyData.Schools;
    
    public string? CurrentUser { get; set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        CurrentUser = User.Identity?.Name;
        var cat = await watchlistQueryService.GetAllEstablishmentsForUser(CurrentUser ?? string.Empty, cancellationToken);
    }
}
