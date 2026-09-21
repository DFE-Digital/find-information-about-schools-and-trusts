using System.Security.Claims;
using DfE.FindInformationAcademiesTrusts.Application.Common.Models;
using DfE.FindInformationAcademiesTrusts.Application.WatchlistCommands.Queries;
using DfE.FindInformationAcademiesTrusts.Domain.ValueObjects;
using DfE.FindInformationAcademiesTrusts.HttpServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IndexModel = DfE.FindInformationAcademiesTrusts.Pages.Watchlist.Index;
using WatchlistEntity = DfE.FindInformationAcademiesTrusts.Domain.Entities.Watchlist;
using EstablishmentDto = GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Establishments.EstablishmentDto;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Watchlist;

public class WatchlistIndexModelTests
{
    [Fact]
    public async Task OnGetAsync_ShouldPopulateTheSchoolsAndTrustsForTheCurrentUser()
    {
        const string userName = "user@education.gov.uk";

        var found = new WatchlistEntity(new WatchlistId(Guid.NewGuid()), "100001", null, false, userName)
        {
            CreatedOn = new DateTime(2026, 1, 15, 10, 30, 0, DateTimeKind.Utc)
        };
        var notFound = new WatchlistEntity(new WatchlistId(Guid.NewGuid()), "100002", null, false, userName)
        {
            CreatedOn = new DateTime(2026, 1, 16, 10, 30, 0, DateTimeKind.Utc)
        };
        var trust = new WatchlistEntity(new WatchlistId(Guid.NewGuid()), null, "TR001", true, userName);

        var watchlistQueryService = Substitute.For<IWatchlistQueryService>();
        watchlistQueryService
            .GetAllEstablishmentsForUser(userName, Arg.Any<CancellationToken>())
            .Returns(Result<IEnumerable<WatchlistEntity>>.Success([found, notFound]));
        watchlistQueryService
            .GetAllTrustsForUser(userName, Arg.Any<CancellationToken>())
            .Returns(Result<IEnumerable<WatchlistEntity>>.Success([trust]));

        var getEstablishments = Substitute.For<IGetEstablishmentsTemp>();
        getEstablishments.GetEstablishment(100001).Returns(new EstablishmentDto
        {
            Urn = "100001",
            Name = "Test School",
            TrustName = "Test Trust",
            LocalAuthorityName = "Test Authority"
        });
        getEstablishments.GetEstablishment(100002).Returns((EstablishmentDto)null!);

        var sut = new IndexModel(watchlistQueryService, getEstablishments)
        {
            PageContext = new PageContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Name, userName)], "TestAuth"))
                }
            }
        };

        await sut.OnGetAsync(CancellationToken.None);

        var item = Assert.Single(sut.Items);
        Assert.Equal(userName, sut.CurrentUser);
        Assert.Equal(found.Id.Value, item.WatchlistId);
        Assert.Equal(found.CreatedOn, item.CreatedOn);
        Assert.Equal("100001", item.Urn);
        Assert.Equal("Test School", item.Name);
        Assert.Equal("Test Trust", item.TrustName);
        Assert.Equal("Test Authority", item.LocalAuthority);
        Assert.Equal(1, sut.SchoolsCount);
        Assert.Equal(1, sut.TrustsCount);
    }
}
