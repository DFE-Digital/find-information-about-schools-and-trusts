using DfE.FindInformationAcademiesTrusts.Application.Common.Models;

namespace DfE.FindInformationAcademiesTrusts.Application.Watchlist.Queries;

public interface IWatchlistQueryService 
{
    Task<Result<IEnumerable<Domain.Entities.Watchlist>>> GetAllEstablishmentsForUser(
        string user,
        CancellationToken cancellationToken);

    Task<Result<IEnumerable<Domain.Entities.Watchlist>>> GetAllTrustsForUser(
        string user,
        CancellationToken cancellationToken);
}
