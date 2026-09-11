using DfE.FindInformationAcademiesTrusts.Application.Common.Models;
using DfE.FindInformationAcademiesTrusts.Domain.Entities;

namespace DfE.FindInformationAcademiesTrusts.Application.WatchlistCommands.Queries;

public interface IWatchlistQueryService 
{
    Task<Result<IEnumerable<Watchlist>>> GetAllEstablishmentsForUser(
        string user,
        CancellationToken cancellationToken);

    Task<Result<IEnumerable<Watchlist>>> GetAllTrustsForUser(
        string user,
        CancellationToken cancellationToken);
}
