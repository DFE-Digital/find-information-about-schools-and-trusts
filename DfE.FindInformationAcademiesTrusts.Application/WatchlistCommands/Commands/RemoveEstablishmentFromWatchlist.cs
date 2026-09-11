using DfE.FindInformationAcademiesTrusts.Domain.Interfaces.Repositories;
using DfE.FindInformationAcademiesTrusts.Domain.ValueObjects;
using MediatR;

namespace DfE.FindInformationAcademiesTrusts.Application.WatchlistCommands.Commands;

public record RemoveEstablishmentFromWatchlistCommand(
    Guid WatchlistId
): IRequest<bool>;

public class RemoveSchoolFromWatchlist
{
    public class RemoveEstablishmentFromWatchlistCommandHandler(IWatchlistRepository watchlistRepository)
        : IRequestHandler<RemoveEstablishmentFromWatchlistCommand, bool>
    {
        public async Task<bool> Handle(RemoveEstablishmentFromWatchlistCommand request, CancellationToken cancellationToken)
        {
            var id = new WatchlistId(request.WatchlistId);
            var watchlistItem = await watchlistRepository.GetAsync(id, cancellationToken);
            
            await watchlistRepository.RemoveAsync(watchlistItem, cancellationToken);

            return true;
        }
    }
}