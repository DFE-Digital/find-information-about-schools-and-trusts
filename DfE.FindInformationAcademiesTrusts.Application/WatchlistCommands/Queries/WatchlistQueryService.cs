using DfE.FindInformationAcademiesTrusts.Application.Common.Models;
using DfE.FindInformationAcademiesTrusts.Application.Establishments.Models;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.AcademiesDbServices;
using DfE.FindInformationAcademiesTrusts.Domain.Entities;
using DfE.FindInformationAcademiesTrusts.Domain.Interfaces.Repositories;
using DfE.FindInformationAcademiesTrusts.Domain.ValueObjects;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Establishments;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Trusts;

namespace DfE.FindInformationAcademiesTrusts.Application.WatchlistCommands.Queries;

public class WatchlistQueryService(IWatchlistRepository watchlistRepository) : IWatchlistQueryService
{
    
    public async Task<Result<IEnumerable<Watchlist>>> GetAllEstablishmentsForUser(
        string user,
        CancellationToken cancellationToken)
    {
        var establishments = await watchlistRepository.GetEstablishmentsForUser(user, cancellationToken);
        
        var usersEstablishments = establishments
            .Where(x => x.User == user && !x.IsTrust).ToList();

        if (!usersEstablishments.Any())
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
        
        var usersTrusts = trusts
            .Where(x => x.User == user && x.IsTrust).ToList();

        if (!usersTrusts.Any())
        {
            return Result<IEnumerable<Watchlist>>.Success([]);
        }
        
        
        return Result<IEnumerable<Watchlist>>.Success(usersTrusts);
    }
}
