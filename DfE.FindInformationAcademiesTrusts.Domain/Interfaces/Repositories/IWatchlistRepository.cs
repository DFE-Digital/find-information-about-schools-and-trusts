using DfE.FindInformationAcademiesTrusts.Domain.Entities;

namespace DfE.FindInformationAcademiesTrusts.Domain.Interfaces.Repositories

{
    public interface IWatchlistRepository : IRepository<Watchlist>
    {
        Task<IEnumerable<Watchlist>> GetEstablishmentsForUser(string user, CancellationToken cancellationToken);
        
        Task<IEnumerable<Watchlist>> GetTrustsForUser(string user, CancellationToken cancellationToken);
    }
}
