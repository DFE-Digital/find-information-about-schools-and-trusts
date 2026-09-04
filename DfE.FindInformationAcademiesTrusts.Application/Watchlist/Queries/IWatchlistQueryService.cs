using DfE.FindInformationAcademiesTrusts.Application.Common.Models;
using DfE.FindInformationAcademiesTrusts.Application.Watchlist.Models;

namespace DfE.FindInformationAcademiesTrusts.Application.Watchlist.Queries;

public interface IWatchlistQueryService 
{
    Task<Result<IEnumerable<EstablishmentWatchlistDto>>> GetAllEstablishmentsForUser(
        string user,
        CancellationToken cancellationToken);

    Task<Result<IEnumerable<TrustWatchlistDto>>> GetAllTrustsForUser(
        string user,
        CancellationToken cancellationToken);
}
