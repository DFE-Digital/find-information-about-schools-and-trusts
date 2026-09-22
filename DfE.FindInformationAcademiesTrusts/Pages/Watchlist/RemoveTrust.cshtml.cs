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

public class RemoveTrust(IMediator mediator,IGetTrustsTemp getTrusts, ErrorService errorService) : ContentPageModel
{
    public Guid? Id { get; set;}
    public string? Name { get; set;}
    public string? Trn { get; set;}
    public string? Region { get; set;}
    public string? CompaniesHouseNumber { get; set;}
    
    

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
        
        if (!result)
        {
            errorService.AddApiError();
            return Page();
        }
        
        TempData["TrustRemoved"] = true;
        ViewData["ActiveWatchListTab"] = "Trusts";
        return RedirectToPage(Links.Watchlist.TrustsWatchlist.Page);
    }
}
