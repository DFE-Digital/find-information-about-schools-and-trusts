using DfE.FindInformationAcademiesTrusts.Domain.Interfaces.Repositories;
using DfE.FindInformationAcademiesTrusts.Domain.ValueObjects;
using MediatR;

namespace DfE.FindInformationAcademiesTrusts.Application.WatchlistCommands.Commands;

public record RemoveTrustFromWatchlistCommand(
    Guid WatchlistId
): IRequest<bool>;

public class RemoveTrustFromWatchlist
{
    public class RemoveTrustFromWatchlistCommandHandler(IWatchlistRepository watchlistRepository)
        : IRequestHandler<RemoveTrustFromWatchlistCommand, bool>
    {
        public async Task<bool> Handle(RemoveTrustFromWatchlistCommand request, CancellationToken cancellationToken)
        {
            var id = new WatchlistId(request.WatchlistId);
            var watchlistItem = await watchlistRepository.GetAsync(id, cancellationToken);
            
            await watchlistRepository.RemoveAsync(watchlistItem, cancellationToken);

            return true;
        }
    }
}