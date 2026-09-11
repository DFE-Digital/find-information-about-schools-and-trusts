using DfE.FindInformationAcademiesTrusts.Application.WatchlistCommands.Models;
using DfE.FindInformationAcademiesTrusts.Application.WatchlistCommands.Queries;
using DfE.FindInformationAcademiesTrusts.HttpServices;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using Dfe.FindInformationAcademiesTrusts.ViewModels;
using EstablishmentDto = GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Establishments.EstablishmentDto;
using TrustDto = GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Trusts.TrustDto;


namespace DfE.FindInformationAcademiesTrusts.Pages.WatchList;

public class Trusts(IWatchlistQueryService watchlistQueryService,IGetTrustsTemp getTrusts) : ContentPageModel
{
    public IEnumerable<TrustWatchlistViewModel> Items { get; set; } = Array.Empty<TrustWatchlistViewModel>();

    public string? CurrentUser { get; set; }

    public int TrustsCount => Items.Count();
    public int SchoolsCount { get; set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        CurrentUser = User.Identity?.Name;
        
        var trustsWatchlist = await watchlistQueryService.GetAllTrustsForUser(CurrentUser ?? string.Empty, cancellationToken);
        
        if (trustsWatchlist.Value != null && trustsWatchlist.Value.Any())
        {
            foreach (var trustWatchlist in trustsWatchlist.Value)
            {
                var result = await getTrusts.GetTrustByReferenceNumber(trustWatchlist.TrustId!);

                if (result != null)
                {
                    var trust = result;
                    Items = Items.Append(new TrustWatchlistViewModel
                    {
                         WatchlistId  = trustWatchlist.Id.Value,
                         ReferenceNumber = trust.ReferenceNumber,
                         GroupUid = trust.GroupUid,
                         Region = trust.Gor,
                         Name = trust.Name,
                         CompaniesHouseNumber = trust.CompaniesHouseNumber,
                         CreatedOn = trustWatchlist.CreatedOn
                    });
                }
            }
            
            Items = Items.OrderBy(i => i.CreatedOn).ToList();
        }
     
        
        var schools = await watchlistQueryService.GetAllEstablishmentsForUser(CurrentUser ?? string.Empty, cancellationToken);
        SchoolsCount = schools.Value?.Count() ?? 0;
    }
}

