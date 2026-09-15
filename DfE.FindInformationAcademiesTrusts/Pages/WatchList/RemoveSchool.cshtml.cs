using DfE.FindInformationAcademiesTrusts.Application.WatchlistCommands.Commands;
using DfE.FindInformationAcademiesTrusts.Domain.ValueObjects;
using DfE.FindInformationAcademiesTrusts.HttpServices;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DfE.FindInformationAcademiesTrusts.Pages.WatchList;

public class RemoveSchool(IMediator mediator,IGetEstablishmentsTemp getEstablishments) : ContentPageModel
{

    public Guid? Id;
    public string? Name;
    public string? Urn;
    public string? Trust;
    public string? LocalAuthority;

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
        
        TempData["SchoolRemoved"] = true;
        return RedirectToPage("/WatchList/Index");
    }
}
