using DfE.FindInformationAcademiesTrusts.Application.Common.Models;
using DfE.FindInformationAcademiesTrusts.Application.Watchlist.Models;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.AcademiesDbServices;
using DfE.FindInformationAcademiesTrusts.Domain.Entities;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Establishments;

namespace DfE.FindInformationAcademiesTrusts.Application.Watchlist.Queries;

public class WatchlistQueryService(IGetEstablishments getEstablishments) : IWatchlistQueryService
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
            EstablishmentId = "100002",
            IsTrust = false,
            User = "James Wilson",
            CreatedOn = new DateTime(2026, 8, 8),
            CreatedBy = "James Wilson"
        },
        new()
        {
            Id = Guid.NewGuid(),
            EstablishmentId = "100003",
            IsTrust = false,
            User = "Emily Carter",
            CreatedOn = new DateTime(2026, 8, 3),
            CreatedBy = "Emily Carter"
        },
        new()
        {
            Id = Guid.NewGuid(),
            EstablishmentId = "100004",
            IsTrust = false,
            User = "David Brown",
            CreatedOn = new DateTime(2026, 7, 28),
            CreatedBy = "David Brown"
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
            TrustId = "200001",
            IsTrust = true,
            User = "Rebecca Hughes",
            CreatedOn = new DateTime(2026, 8, 14),
            CreatedBy = "Rebecca Hughes"
        },
        new()
        {
            Id = Guid.NewGuid(),
            TrustId = "200002",
            IsTrust = true,
            User = "Michael Turner",
            CreatedOn = new DateTime(2026, 8, 9),
            CreatedBy = "Michael Turner"
        },
        new()
        {
            Id = Guid.NewGuid(),
            TrustId = "200003",
            IsTrust = true,
            User = "Amelia Foster",
            CreatedOn = new DateTime(2026, 8, 4),
            CreatedBy = "Amelia Foster"
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

    public async Task<Result<IEnumerable<Domain.Entities.Watchlist>>> GetAllTrustsForUser(
        string user,
        CancellationToken cancellationToken)
    {
        var trusts = Watchlists
            .Where(x => x.User == user && x.IsTrust);

        return Result<IEnumerable<Domain.Entities.Watchlist>>.Success(trusts);
    }
}
