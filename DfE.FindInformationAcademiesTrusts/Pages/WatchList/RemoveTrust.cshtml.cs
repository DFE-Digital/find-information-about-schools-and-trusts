using DfE.FindInformationAcademiesTrusts.Application.WatchlistCommands.Commands;
using DfE.FindInformationAcademiesTrusts.Domain.ValueObjects;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DfE.FindInformationAcademiesTrusts.Pages.WatchList;

public class RemoveTrust(IMediator mediator) : ContentPageModel
{

    public Guid? Id;

    public Task<IActionResult> OnGetAsync(Guid trustIdToRemove, CancellationToken cancellationToken)
    {
        Id = trustIdToRemove;

        return Task.FromResult<IActionResult>(Page());
    }

    public async Task<IActionResult> OnPostAsync(Guid id,CancellationToken cancellationToken = default)
    {
        var request = new RemoveTrustFromWatchlistCommand(id);
        var result = await mediator.Send(request, cancellationToken);
        return RedirectToPage("/WatchList/Index");
    }
}
