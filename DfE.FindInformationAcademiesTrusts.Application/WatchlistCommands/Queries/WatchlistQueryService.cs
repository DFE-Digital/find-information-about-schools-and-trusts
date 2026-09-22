using DfE.FindInformationAcademiesTrusts.Application.Common.Models;
using DfE.FindInformationAcademiesTrusts.Domain.Entities;
using DfE.FindInformationAcademiesTrusts.Domain.Interfaces.Repositories;
using DfE.FindInformationAcademiesTrusts.Domain.ValueObjects;

namespace DfE.FindInformationAcademiesTrusts.Application.WatchlistCommands.Queries;

public class WatchlistQueryService(IWatchlistRepository watchlistRepository) : IWatchlistQueryService
{
    
    public async Task<Result<IEnumerable<Watchlist>>> GetAllEstablishmentsForUser(
        string user,
        CancellationToken cancellationToken)
    {
        var establishments = await watchlistRepository.GetEstablishmentsForUser(user, cancellationToken);
        
        var usersEstablishments = establishments.ToList();

        if (usersEstablishments.Count == 0)
        {
            return Result<IEnumerable<Watchlist>>.Success([]);
        }

        return Result<IEnumerable<Watchlist>>.Success(usersEstablishments);
        
    }

    public async Task<Result<IEnumerable<Watchlist>>> GetAllTrustsForUser(
        string user,
        CancellationToken cancellationToken)
    {
        var trusts  = await watchlistRepository.GetTrustsForUser(user, cancellationToken);


        var watchlists = trusts.ToList();
        
        if (watchlists.Count == 0)
        {
            return Result<IEnumerable<Watchlist>>.Success([]);
        }
        
        
        return Result<IEnumerable<Watchlist>>.Success(watchlists);
    }
}
