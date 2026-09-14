using DfE.FindInformationAcademiesTrusts.Application.WatchlistCommands.Commands;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.AcademiesDbServices;
using DfE.FindInformationAcademiesTrusts.Domain.ValueObjects;
using DfE.FindInformationAcademiesTrusts.HttpServices;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DfE.FindInformationAcademiesTrusts.Pages.WatchList;

public class ConfirmSchool(IGetEstablishmentsTemp getEstablishments,IMediator mediator) : ContentPageModel
{
    public string? Id;
    
    public string? Name;
    
    public string? Trust;
    
    public string? LocalAuthority;
    public string? CurrentUser { get; set; }
    
    public async Task OnGet(string id)
    {
        var establishment =
            await getEstablishments.GetEstablishment(int.Parse(id));

        Id = establishment.Urn;
        Name = establishment.Name;
        Trust = establishment.TrustName ?? "Not applicable";
        LocalAuthority = establishment.LocalAuthorityName;
    }

    public async Task<IActionResult> OnPostAsync(string establishmentId,CancellationToken cancellationToken = default)
    {
        CurrentUser = User.Identity?.Name;
        
        var request = new AddEstablishmentToWatchlistCommand(establishmentId, CurrentUser!);
        
        var result = await mediator.Send(request, cancellationToken);
        
        var action = Request.Form["action"].ToString();
        
        if (action == "add-another")
        {
            return RedirectToPage("/Watchlist/SearchForASchool");
        }
        
        TempData["SchoolAdded"] = true;
        return RedirectToPage("/WatchList/Index");
    }
}
