using Dfe.AcademiesApi.Client.Contracts;
using DfE.FindInformationAcademiesTrusts.Application.Watchlist.Models;
using DfE.FindInformationAcademiesTrusts.Application.Watchlist.Queries;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;

namespace DfE.FindInformationAcademiesTrusts.Pages.WatchList;

public class Index(IWatchlistQueryService watchlistQueryService) : ContentPageModel
{
    public IEnumerable<EstablishmentWatchlistDto> Items { get; set; } = Array.Empty<EstablishmentWatchlistDto>();
    
    public string? CurrentUser { get; set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        CurrentUser = User.Identity?.Name;
        var items = await watchlistQueryService.GetAllEstablishmentsForUser(CurrentUser ?? string.Empty, cancellationToken);

        Items = items.Value ?? Array.Empty<EstablishmentWatchlistDto>();
    }
}
