using System.Security.Claims;
using DfE.FindInformationAcademiesTrusts.Application.Common.Models;
using DfE.FindInformationAcademiesTrusts.Application.WatchlistCommands.Queries;
using DfE.FindInformationAcademiesTrusts.Domain.ValueObjects;
using DfE.FindInformationAcademiesTrusts.HttpServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrustsModel = DfE.FindInformationAcademiesTrusts.Pages.Watchlist.Trusts;
using WatchlistEntity = DfE.FindInformationAcademiesTrusts.Domain.Entities.Watchlist;
using TrustDto = GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Trusts.TrustDto;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Watchlist;

public class WatchlistTrustsModelTests
{
    [Fact]
    public async Task OnGetAsync_ShouldPopulateTheTrustsAndSchoolsCountForTheCurrentUser()
    {
        const string userName = "user@education.gov.uk";

        var found = new WatchlistEntity(new WatchlistId(Guid.NewGuid()), null, "TR001", true, userName)
        {
            CreatedOn = new DateTime(2026, 1, 15, 10, 30, 0, DateTimeKind.Utc)
        };
        var notFound = new WatchlistEntity(new WatchlistId(Guid.NewGuid()), null, "TR002", true, userName)
        {
            CreatedOn = new DateTime(2026, 1, 16, 10, 30, 0, DateTimeKind.Utc)
        };
        var school = new WatchlistEntity(new WatchlistId(Guid.NewGuid()), "100001", null, false, userName);

        var watchlistQueryService = Substitute.For<IWatchlistQueryService>();
        watchlistQueryService
            .GetAllTrustsForUser(userName, Arg.Any<CancellationToken>())
            .Returns(Result<IEnumerable<WatchlistEntity>>.Success([found, notFound]));
        watchlistQueryService
            .GetAllEstablishmentsForUser(userName, Arg.Any<CancellationToken>())
            .Returns(Result<IEnumerable<WatchlistEntity>>.Success([school]));

        var getTrusts = Substitute.For<IGetTrustsTemp>();
        getTrusts.GetTrustByReferenceNumber("TR001").Returns(new TrustDto
        {
            ReferenceNumber = "TR001",
            GroupUid = "2001",
            Gor = "South East",
            Name = "Test Trust",
            CompaniesHouseNumber = "01234567"
        });
        getTrusts.GetTrustByReferenceNumber("TR002").Returns((TrustDto?)null);

        var sut = new TrustsModel(watchlistQueryService, getTrusts)
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
        Assert.Equal("TR001", item.ReferenceNumber);
        Assert.Equal("2001", item.GroupUid);
        Assert.Equal("South East", item.Region);
        Assert.Equal("Test Trust", item.Name);
        Assert.Equal("01234567", item.CompaniesHouseNumber);
        Assert.Equal(1, sut.TrustsCount);
        Assert.Equal(1, sut.SchoolsCount);
    }
}
