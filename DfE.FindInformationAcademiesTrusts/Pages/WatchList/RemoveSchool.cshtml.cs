using DfE.FindInformationAcademiesTrusts.Application.WatchlistCommands.Commands;
using DfE.FindInformationAcademiesTrusts.Domain.ValueObjects;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DfE.FindInformationAcademiesTrusts.Pages.WatchList;

public class RemoveSchool(IMediator mediator) : ContentPageModel
{

    public Guid? Id;

    public Task<IActionResult> OnGetAsync(Guid watchlistIdToRemove, CancellationToken cancellationToken)
    {
        Id = watchlistIdToRemove;

        return Task.FromResult<IActionResult>(Page());
    }

    public async Task<IActionResult> OnPostAsync(Guid id,CancellationToken cancellationToken = default)
    {
        var request = new RemoveEstablishmentFromWatchlistCommand(id);
        var result = await mediator.Send(request, cancellationToken);
        return RedirectToPage("/WatchList/Index");
    }
}
