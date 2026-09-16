using DfE.FindInformationAcademiesTrusts.Application.WatchlistCommands.Commands;
using DfE.FindInformationAcademiesTrusts.Domain.ValueObjects;
using DfE.FindInformationAcademiesTrusts.HttpServices;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DfE.FindInformationAcademiesTrusts.Pages.Watchlist;

public class RemoveTrust(IMediator mediator,IGetTrustsTemp getTrusts) : ContentPageModel
{
    public Guid? Id;
    public string? Name;
    public string? Trn;
    public string? Region;
    public string? CompaniesHouseNumber;
    
    

    public Task<IActionResult> OnGetAsync(Guid trustIdToRemove,string referenceNumber, CancellationToken cancellationToken)
    {   var trust = getTrusts.GetTrustByReferenceNumber(referenceNumber).Result;
        
        Id = trustIdToRemove;
        Name = trust!.Name;
        Trn = trust.ReferenceNumber;
        Region = trust.Gor ?? "Not Applicable";
        CompaniesHouseNumber = trust.CompaniesHouseNumber;
        

        return Task.FromResult<IActionResult>(Page());
    }

    public async Task<IActionResult> OnPostAsync(Guid id,CancellationToken cancellationToken = default)
    {
        var request = new RemoveTrustFromWatchlistCommand(id);
        var result = await mediator.Send(request, cancellationToken);
        
        TempData["TrustRemoved"] = true;
        ViewData["ActiveWatchListTab"] = "Trusts";
        return RedirectToPage("/WatchList/Trusts");
    }
}
