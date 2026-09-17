using Dfe.AcademiesApi.Client.Contracts;
using DfE.FindInformationAcademiesTrusts.Application.WatchlistCommands.Models;
using DfE.FindInformationAcademiesTrusts.Application.WatchlistCommands.Queries;
using DfE.FindInformationAcademiesTrusts.HttpServices;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using EstablishmentDto = GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Establishments.EstablishmentDto;

namespace DfE.FindInformationAcademiesTrusts.Pages.WatchList;

public class Index(IWatchlistQueryService watchlistQueryService,IGetEstablishmentsTemp getEstablishments) : ContentPageModel
{
    public IEnumerable<EstablishmentDto> Items { get; set; } = Array.Empty<EstablishmentDto>();

    public string? CurrentUser { get; set; }

    public int SchoolsCount => Items.Count();
    public int TrustsCount; 

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        CurrentUser = User.Identity?.Name;

        var establishmentsWatchlist = await watchlistQueryService.GetAllEstablishmentsForUser(CurrentUser ?? string.Empty, cancellationToken);

        var urns = establishmentsWatchlist.Value?
            .Select(e => e.EstablishmentId)
            .Where(id => !string.IsNullOrWhiteSpace(id) && int.TryParse(id, out _))
            .Select(id => int.Parse(id!))
            .ToList() ?? [];
        
        
        
        
        
        var trustsWatchlist = await watchlistQueryService.GetAllTrustsForUser(CurrentUser ?? string.Empty, cancellationToken);
        
        TrustsCount = trustsWatchlist.Value?.Count() ?? 0;

        if (urns.Count == 0)
        {
            Items = Array.Empty<EstablishmentDto>();
            return;
        }

        var items = await getEstablishments.GetEstablishmentsByUrns(urns);
        
        Items = (IEnumerable<EstablishmentDto>)items ?? Array.Empty<EstablishmentDto>();
    }
}
