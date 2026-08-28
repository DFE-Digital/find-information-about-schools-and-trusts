using DfE.FindInformationAcademiesTrusts.Application.Watchlist.Models;
using DfE.FindInformationAcademiesTrusts.Application.Watchlist.Queries;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;

namespace DfE.FindInformationAcademiesTrusts.Pages.WatchList;

public class Trusts(IWatchlistQueryService watchlistQueryService) : ContentPageModel
{
    public IEnumerable<TrustWatchlistDto> Items { get; set; } = Array.Empty<TrustWatchlistDto>();

    public string? CurrentUser { get; set; }

    public int TrustsCount => Items.Count();
    public int SchoolsCount { get; set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        CurrentUser = User.Identity?.Name;
        
        var items = await watchlistQueryService.GetAllTrustsForUser(CurrentUser ?? string.Empty, cancellationToken);
        Items = items.Value ?? Array.Empty<TrustWatchlistDto>();

        var schools = await watchlistQueryService.GetAllEstablishmentsForUser(CurrentUser ?? string.Empty, cancellationToken);
        SchoolsCount = schools.Value?.Count() ?? 0;
    }
}
