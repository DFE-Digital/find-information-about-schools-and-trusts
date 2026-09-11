using DfE.FindInformationAcademiesTrusts.Application.WatchlistCommands.Commands;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.AcademiesDbServices;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DfE.FindInformationAcademiesTrusts.Pages.WatchList;

public class ConfirmTrust(IGetTrusts getTrusts,IMediator mediator) : ContentPageModel
{
    public string? ReferenceNumber;
    
    public string? Name;
    public string? CurrentUser { get; set; }
    
    public async Task OnGet(string referenceNumber)
    {
        var trust =
            await getTrusts.GetTrustByReferenceNumber(referenceNumber);

        ReferenceNumber = trust!.ReferenceNumber;
        Name = trust.Name;
    }

    public async Task<IActionResult> OnPostAsync(string referenceNumber,CancellationToken cancellationToken = default)
    {
        CurrentUser = User.Identity?.Name;
        
        var request = new AddTrustToWatchlistCommand(referenceNumber, CurrentUser!);
        
        var result = await mediator.Send(request, cancellationToken);
        
        return RedirectToPage("/WatchList/Index");
    }
}
