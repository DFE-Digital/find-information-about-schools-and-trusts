using DfE.FindInformationAcademiesTrusts.Application.WatchlistCommands.Commands;
using DfE.FindInformationAcademiesTrusts.Domain.ValueObjects;
using DfE.FindInformationAcademiesTrusts.HttpServices;
using Dfe.FindInformationAcademiesTrusts.Models;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using Dfe.FindInformationAcademiesTrusts.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DfE.FindInformationAcademiesTrusts.Pages.Watchlist;

public class RemoveSchool(IMediator mediator,IGetEstablishmentsTemp getEstablishments,ErrorService errorService) : ContentPageModel
{

    public Guid? Id { get; set;}
    public string? Name { get; set;}
    public string? Urn { get; set;}
    public string? Trust { get; set;}
    public string? LocalAuthority { get; set;}

    public Task<IActionResult> OnGetAsync(Guid watchlistIdToRemove,string establishmentName,string establishmentUrn,string establishmentTrust,string establishmentLocalAuthority, CancellationToken cancellationToken)
    {
        var establishment = getEstablishments.GetEstablishment(int.Parse(establishmentUrn)).Result;
        
        Id = watchlistIdToRemove;
        Name = establishment.Name;
        Urn = establishment.Urn;
        Trust = establishment.TrustName ?? "Not applicable";
        LocalAuthority = establishment.LocalAuthorityName;
        
        return Task.FromResult<IActionResult>(Page());
    }

    public async Task<IActionResult> OnPostAsync(Guid id,CancellationToken cancellationToken = default)
    {
        var request = new RemoveEstablishmentFromWatchlistCommand(id);
        
        var result = await mediator.Send(request, cancellationToken);
        
        if (!result)
        {
            errorService.AddApiError();
            return Page();
        }
        
        
        TempData["SchoolRemoved"] = true;
        return RedirectToPage(Links.Watchlist.SchoolsWatchlist.Page);
    }
}
