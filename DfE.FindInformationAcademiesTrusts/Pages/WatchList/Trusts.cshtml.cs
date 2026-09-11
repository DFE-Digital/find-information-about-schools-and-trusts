using DfE.FindInformationAcademiesTrusts.Application.WatchlistCommands.Models;
using DfE.FindInformationAcademiesTrusts.Application.WatchlistCommands.Queries;
using DfE.FindInformationAcademiesTrusts.HttpServices;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using EstablishmentDto = GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Establishments.EstablishmentDto;
using TrustDto = GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Trusts.TrustDto;


namespace DfE.FindInformationAcademiesTrusts.Pages.WatchList;

public class Trusts(IWatchlistQueryService watchlistQueryService,IGetTrustsTemp getTrusts) : ContentPageModel
{
    public IEnumerable<TrustDto> Items { get; set; } = Array.Empty<TrustDto>();

    public string? CurrentUser { get; set; }

    public int TrustsCount => Items.Count();
    public int SchoolsCount { get; set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        CurrentUser = User.Identity?.Name;
        
        var trustsWatchlist = await watchlistQueryService.GetAllTrustsForUser(CurrentUser ?? string.Empty, cancellationToken);
        
        List<string> referenceNumbers = trustsWatchlist.Value?
            .Select(x => x.TrustId)
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Select(id => id!.Trim())
            .ToList() ?? [];
        
        var schools = await watchlistQueryService.GetAllEstablishmentsForUser(CurrentUser ?? string.Empty, cancellationToken);
        SchoolsCount = schools.Value?.Count() ?? 0;
        
        if(referenceNumbers.Count == 0)
        {
            Items = Array.Empty<TrustDto>();
            return;
        }
        
        var items = await getTrusts.GetTrustsByReferenceNumbers(referenceNumbers);

        Items = (IEnumerable<TrustDto>)items ?? Array.Empty<TrustDto>();
    }
}
