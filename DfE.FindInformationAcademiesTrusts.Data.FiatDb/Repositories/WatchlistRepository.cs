using DfE.FindInformationAcademiesTrusts.Data.FiatDb.Contexts;
using DfE.FindInformationAcademiesTrusts.Domain.Entities;
using DfE.FindInformationAcademiesTrusts.Domain.Interfaces.Repositories;

namespace DfE.FindInformationAcademiesTrusts.Data.FiatDb.Repositories;

public class WatchlistRepository(FindInformationAcademiesTrustContext dbContext)
    : Repository<Watchlist, FindInformationAcademiesTrustContext>(dbContext), IWatchlistRepository
{
    
}
