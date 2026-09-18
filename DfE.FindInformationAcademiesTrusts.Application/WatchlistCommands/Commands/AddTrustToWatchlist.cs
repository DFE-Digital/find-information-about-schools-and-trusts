using DfE.FindInformationAcademiesTrusts.Domain.Interfaces.Repositories;
using DfE.FindInformationAcademiesTrusts.Domain.ValueObjects;
using MediatR;

namespace DfE.FindInformationAcademiesTrusts.Application.WatchlistCommands.Commands;

public record AddTrustToWatchlistCommand(
    string TrustId,
    string User
): IRequest<bool>;

public class AddTrustToWatchlist
{
    public class AddTrustToWatchlistCommandHandler(IWatchlistRepository watchlistRepository)
        : IRequestHandler<AddTrustToWatchlistCommand, bool>
    {
        public async Task<bool> Handle(AddTrustToWatchlistCommand request, CancellationToken cancellationToken)
        {
            var watchlistId = new WatchlistId(Guid.NewGuid());

            var watchlistRecord = new Domain.Entities.Watchlist(
                watchlistId,
                null,
                request.TrustId,
                true,
                request.User
            );
            
            await watchlistRepository.AddAsync(watchlistRecord, cancellationToken);

            return true;
        }
    }
}