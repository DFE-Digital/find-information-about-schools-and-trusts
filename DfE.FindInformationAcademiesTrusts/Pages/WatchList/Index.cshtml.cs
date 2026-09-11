using Dfe.AcademiesApi.Client.Contracts;
using DfE.FindInformationAcademiesTrusts.Application.WatchlistCommands.Models;
using DfE.FindInformationAcademiesTrusts.Application.WatchlistCommands.Queries;
using DfE.FindInformationAcademiesTrusts.HttpServices;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using Dfe.FindInformationAcademiesTrusts.ViewModels;
using EstablishmentDto = GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Establishments.EstablishmentDto;

namespace DfE.FindInformationAcademiesTrusts.Pages.WatchList;

public class Index(IWatchlistQueryService watchlistQueryService,IGetEstablishmentsTemp getEstablishments) : ContentPageModel
{
    public IEnumerable<SchoolWatchlistViewModel> Items { get; set; } = Array.Empty<SchoolWatchlistViewModel>();

    public string? CurrentUser { get; set; }

    public int SchoolsCount => Items.Count();
    public int TrustsCount; 

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        CurrentUser = User.Identity?.Name;

        var establishmentsWatchlist = await watchlistQueryService.GetAllEstablishmentsForUser(CurrentUser ?? string.Empty, cancellationToken);

        if (establishmentsWatchlist.Value != null && establishmentsWatchlist.Value.Any())
        {
            foreach (var establishmentWatchlist in establishmentsWatchlist.Value)
            {
                int establishmentId = int.Parse(establishmentWatchlist.EstablishmentId!);
                var result = await getEstablishments.GetEstablishment(establishmentId);

                if (result != null)
                {
                    var establishment = result;
                    Items = Items.Append(new SchoolWatchlistViewModel
                    {
                        WatchlistId  = establishmentWatchlist.Id.Value,
                        CreatedOn = establishmentWatchlist .CreatedOn,
                        Urn = establishment.Urn,
                        Name = establishment.Name,
                        TrustName = establishment.TrustName,
                        LocalAuthority = establishment.LocalAuthorityName
                    });
                }
            }
            
            Items = Items.OrderBy(i => i.CreatedOn).ToList();
        }
        

        var trustsWatchlist = await watchlistQueryService.GetAllTrustsForUser(CurrentUser ?? string.Empty, cancellationToken);
        
        TrustsCount = trustsWatchlist.Value?.Count() ?? 0;
        
    }
}
