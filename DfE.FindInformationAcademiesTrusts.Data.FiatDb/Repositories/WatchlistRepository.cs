using DfE.FindInformationAcademiesTrusts.Data.FiatDb.Contexts;
using DfE.FindInformationAcademiesTrusts.Domain.Entities;
using DfE.FindInformationAcademiesTrusts.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DfE.FindInformationAcademiesTrusts.Data.FiatDb.Repositories;

public class WatchlistRepository(FindInformationAcademiesTrustsContext dbContext)
    : Repository<Watchlist, FindInformationAcademiesTrustsContext>(dbContext), IWatchlistRepository
{
    public async Task<IEnumerable<Watchlist>> GetEstablishmentsForUser(string user, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(user))
        {
            return Array.Empty<Watchlist>();
        }

        return await DbSet()
            .AsNoTracking()
            .Where(w => w.User == user && !w.IsTrust)
            .Distinct()
            .ToListAsync(cancellationToken);
    }
    
    public async Task<IEnumerable<Watchlist>> GetTrustsForUser(string user, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(user))
        {
            return Array.Empty<Watchlist>();
        }

        return await DbSet()
            .AsNoTracking()
            .Where(w => w.User == user && w.IsTrust)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

}
