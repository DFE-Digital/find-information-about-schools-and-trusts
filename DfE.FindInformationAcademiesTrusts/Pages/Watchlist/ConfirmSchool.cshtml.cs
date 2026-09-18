using DfE.FindInformationAcademiesTrusts.Application.WatchlistCommands.Commands;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.AcademiesDbServices;
using DfE.FindInformationAcademiesTrusts.Domain.ValueObjects;
using DfE.FindInformationAcademiesTrusts.HttpServices;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using Dfe.FindInformationAcademiesTrusts.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DfE.FindInformationAcademiesTrusts.Pages.Watchlist;

public class ConfirmSchool(IGetEstablishmentsTemp getEstablishments,IMediator mediator,ErrorService errorService) : ContentPageModel
{
    public string? Id { get; set; }
    
    public string? Name  { get; set; }
    
    public string? Trust  { get; set; }
    
    public string? LocalAuthority { get; set; }
    
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
        
        if (!result)
        {
            errorService.AddApiError();
            return Page();
        }
        
        var action = Request.Form["action"].ToString();
        
        if (action == "add-another")
        {
            return RedirectToPage("/Watchlist/SearchForASchool");
        }
        
        TempData["SchoolAdded"] = true;
        return RedirectToPage("/WatchList/Index");
    }
}
