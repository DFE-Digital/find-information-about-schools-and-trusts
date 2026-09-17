using DfE.FindInformationAcademiesTrusts.Domain.Interfaces.Repositories;
using DfE.FindInformationAcademiesTrusts.Domain.ValueObjects;
using MediatR;

namespace DfE.FindInformationAcademiesTrusts.Application.WatchlistCommands.Commands;


public record AddEstablishmentToWatchlistCommand(
    string EstablishmentId,
    string User
): IRequest<bool>;

public class AddEstablishmentToWatchlist
{
    public class AddEstablishmentToWatchlistCommandHandler(IWatchlistRepository watchlistRepository)
        : IRequestHandler<AddEstablishmentToWatchlistCommand, bool>
    {
        public async Task<bool> Handle(AddEstablishmentToWatchlistCommand request, CancellationToken cancellationToken)
        {
            var watchlistId = new WatchlistId(Guid.NewGuid());

            var watchlistRecord = new Domain.Entities.Watchlist(
                watchlistId,
                request.EstablishmentId,
                null,
                false,
                request.User
            );
            
            await watchlistRepository.AddAsync(watchlistRecord, cancellationToken);

            return true;
        }
    }
}
