using DfE.FindInformationAcademiesTrusts.Application.Common.Models;
using DfE.FindInformationAcademiesTrusts.Application.Watchlist.Models;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.AcademiesDbServices;
using DfE.FindInformationAcademiesTrusts.Domain.Entities;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Establishments;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Trusts;

namespace DfE.FindInformationAcademiesTrusts.Application.Watchlist.Queries;

public class WatchlistQueryService(IGetEstablishments getEstablishments, IGetTrusts getTrusts) : IWatchlistQueryService
{
    private static readonly IReadOnlyList<Domain.Entities.Watchlist> Watchlists =
    [
        new()
        {
            Id = Guid.NewGuid(),
            EstablishmentId = "135963",
            IsTrust = false,
            User = "Dan.RYAN@EDUCATION.GOV.UK",
            CreatedOn = new DateTime(2026, 8, 12),
            CreatedBy = "Dan.RYAN@EDUCATION.GOV.UK"
        },
        new()
        {
            Id = Guid.NewGuid(),
            EstablishmentId = "101314",
            IsTrust = false,
            User = "Dan.RYAN@EDUCATION.GOV.UK",
            CreatedOn = new DateTime(2026, 8, 8),
            CreatedBy = "Dan.RYAN@EDUCATION.GOV.UK"
        },
        new()
        {
            Id = Guid.NewGuid(),
            EstablishmentId = "139041",
            IsTrust = false,
            User = "Dan.RYAN@EDUCATION.GOV.UK",
            CreatedOn = new DateTime(2026, 8, 3),
            CreatedBy = "Dan.RYAN@EDUCATION.GOV.UK"
        },
        new()
        {
            Id = Guid.NewGuid(),
            EstablishmentId = "136394",
            IsTrust = false,
            User = "Richika.DOGRA@EDUCATION.GOV.UK",
            CreatedOn = new DateTime(2026, 7, 28),
            CreatedBy = "Richika.DOGRA@EDUCATION.GOV.UK"
        },
        new()
        {
            Id = Guid.NewGuid(),
            EstablishmentId = "139041",
            IsTrust = false,
            User = "Richika.DOGRA@EDUCATION.GOV.UK",
            CreatedOn = new DateTime(2026, 7, 28),
            CreatedBy = "Richika.DOGRA@EDUCATION.GOV.UK"
        },
        new()
        {
            Id = Guid.NewGuid(),
            EstablishmentId = "135963",
            IsTrust = false,
            User = "Richika.DOGRA@EDUCATION.GOV.UK",
            CreatedOn = new DateTime(2026, 7, 28),
            CreatedBy = "Richika.DOGRA@EDUCATION.GOV.UK"
        },
        new()
        {
            Id = Guid.NewGuid(),
            EstablishmentId = "100005",
            IsTrust = false,
            User = "Laura Evans",
            CreatedOn = new DateTime(2026, 7, 21),
            CreatedBy = "Laura Evans"
        },
        new()
        {
            Id = Guid.NewGuid(),
            TrustId = "tr01585",
            IsTrust = true,
            User = "Dan.RYAN@EDUCATION.GOV.UK",
            CreatedOn = new DateTime(2026, 8, 14),
            CreatedBy = "Dan.RYAN@EDUCATION.GOV.UK"
        },
        new()
        {
            Id = Guid.NewGuid(),
            TrustId = "tr01414",
            IsTrust = true,
            User = "Dan.RYAN@EDUCATION.GOV.UK",
            CreatedOn = new DateTime(2026, 8, 9),
            CreatedBy = "Dan.RYAN@EDUCATION.GOV.UK"
        },
        new()
        {
            Id = Guid.NewGuid(),
            TrustId = "tr02343",
            IsTrust = true,
            User = "Dan.RYAN@EDUCATION.GOV.UK",
            CreatedOn = new DateTime(2026, 8, 4),
            CreatedBy = "Dan.RYAN@EDUCATION.GOV.UK"
        },
        new()
        {
            Id = Guid.NewGuid(),
            TrustId = "200004",
            IsTrust = true,
            User = "Daniel Price",
            CreatedOn = new DateTime(2026, 7, 29),
            CreatedBy = "Daniel Price"
        },
        new()
        {
            Id = Guid.NewGuid(),
            TrustId = "200005",
            IsTrust = true,
            User = "Grace Mitchell",
            CreatedOn = new DateTime(2026, 7, 22),
            CreatedBy = "Grace Mitchell"
        }
    ];

    public async Task<Result<IEnumerable<EstablishmentWatchlistDto>>> GetAllEstablishmentsForUser(
        string user,
        CancellationToken cancellationToken)
    {
        var establishments = Watchlists
            .Where(x => x.User == user && !x.IsTrust);

        List<int> urns = establishments
            .Where(x => x.EstablishmentId != null)
            .Select(x => int.Parse(x.EstablishmentId))
            .ToList();
        
        if (urns.Count == 0)
        {
            return Result<IEnumerable<EstablishmentWatchlistDto>>.Success([]);
        }

        IEnumerable<EstablishmentDto> watchlistEstablishments =
            await getEstablishments.GetEstablishmentsByUrns(urns);
        
        IEnumerable<EstablishmentWatchlistDto> result = watchlistEstablishments
            .Select(x => new EstablishmentWatchlistDto(
                x.Name,
                x.Urn,
                x.TrustName ?? "",
                x.LocalAuthorityName));



        return Result<IEnumerable<EstablishmentWatchlistDto>>.Success(result);
    }

    public async Task<Result<IEnumerable<TrustWatchlistDto>>> GetAllTrustsForUser(
        string user,
        CancellationToken cancellationToken)
    {
        var trusts = Watchlists
            .Where(x => x.User == user && x.IsTrust);

        List<string?> referenceNumbers = trusts
            .Where(x => x.TrustId != null)
            .Select(x => x.TrustId)
            .ToList();

        if (referenceNumbers.Count == 0)
        {
            return Result<IEnumerable<TrustWatchlistDto>>.Success([]);
        }

        IEnumerable<TrustDto> watchlistTrusts =
            await getTrusts.GetTrustsByReferenceNumbers(referenceNumbers);

        IEnumerable<TrustWatchlistDto> result = watchlistTrusts
            .Select(x => new TrustWatchlistDto(
                x.Name,
                x.ReferenceNumber,
                x.GroupUid,
                x.Gor,
                x.CompaniesHouseNumber
                
            ));

        return Result<IEnumerable<TrustWatchlistDto>>.Success(result);
    }
}
