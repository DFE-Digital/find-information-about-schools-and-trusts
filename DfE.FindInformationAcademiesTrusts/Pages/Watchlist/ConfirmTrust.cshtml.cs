using DfE.FindInformationAcademiesTrusts.Application.WatchlistCommands.Commands;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.AcademiesDbServices;
using Dfe.FindInformationAcademiesTrusts.Models;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using Dfe.FindInformationAcademiesTrusts.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DfE.FindInformationAcademiesTrusts.Pages.Watchlist;

public class ConfirmTrust(IGetTrusts getTrusts,IMediator mediator,ErrorService errorService) : ContentPageModel
{
    public string? ReferenceNumber { get; set; }
    
    public string? Name { get; set; }
    
    public string? Region{  get; set; } 
    
    public string? CompaniesHouseNumber{ get; set; }
    public string? CurrentUser { get; set; }
    
    public async Task OnGet(string referenceNumber)
    {
        var trust =
            await getTrusts.GetTrustByReferenceNumber(referenceNumber);

        ReferenceNumber = trust!.ReferenceNumber;
        Name = trust.Name;
        Region = trust.Gor ?? "Not applicable";
        CompaniesHouseNumber = trust.CompaniesHouseNumber;
    }

    public async Task<IActionResult> OnPostAsync(string referenceNumber,CancellationToken cancellationToken = default)
    {
        CurrentUser = User.Identity?.Name;
        
        var request = new AddTrustToWatchlistCommand(referenceNumber, CurrentUser!);
        
        var result = await mediator.Send(request, cancellationToken);
        
        
        if (!result)
        {
            errorService.AddApiError();
            return Page();
        }

        var action = Request.Form["action"].ToString();
        
        if (action == "add-another")
        {
            return RedirectToPage(Links.Watchlist.SearchForATrust.Page);
        }
        
        TempData["TrustAdded"] = true;
        ViewData["ActiveWatchListTab"] = "Trusts";
        return RedirectToPage(Links.Watchlist.TrustsWatchlist.Page);
    }
}
