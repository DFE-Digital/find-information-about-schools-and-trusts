using DfE.FindInformationAcademiesTrusts.Data.FiatDb.Contexts;
using DfE.FindInformationAcademiesTrusts.Data.FiatDb.Repositories;
using DfE.FindInformationAcademiesTrusts.Domain.Entities;
using DfE.FindInformationAcademiesTrusts.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DfE.FindInformationAcademiesTrusts.Data.FiatDb.UnitTests.Repositories;

public class WatchlistRepositoryTests
{
    private const string User = "me@test.gov.uk";
    private const string OtherUser = "other.user@test.gov.uk";

    private static FindInformationAcademiesTrustsContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<FindInformationAcademiesTrustsContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var userDetailsProvider = Substitute.For<IUserDetailsProvider>();
        userDetailsProvider.GetUserDetails().Returns(("Test User", "test@user.com"));

        return new FindInformationAcademiesTrustsContext(options, userDetailsProvider);
    }

    private static Watchlist CreateEstablishment(string establishmentId, string user) =>
        new(new WatchlistId(Guid.NewGuid()), establishmentId, null, false, user);

    private static Watchlist CreateTrust(string trustId, string user) =>
        new(new WatchlistId(Guid.NewGuid()), null, trustId, true, user);
    
    private static async Task SetCreatedOn(
        FindInformationAcademiesTrustsContext context,
        params (Watchlist Watchlist, DateTime CreatedOn)[] values)
    {
        foreach (var (watchlist, createdOn) in values)
        {
            watchlist.CreatedOn = createdOn;
        }

        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetEstablishmentsForUser_WhenUserIsNull_ReturnsEmpty()
    {
        await using var context = CreateContext();
        var sut = new WatchlistRepository(context);

        var result = await sut.GetEstablishmentsForUser(null!, CancellationToken.None);

        result.Should().BeEmpty();
        result.Should().BeAssignableTo<IEnumerable<Watchlist>>();
    }

    [Fact]
    public async Task GetEstablishmentsForUser_WhenUserIsEmpty_ReturnsEmpty()
    {
        await using var context = CreateContext();
        var sut = new WatchlistRepository(context);

        var result = await sut.GetEstablishmentsForUser(string.Empty, CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetEstablishmentsForUser_WhenNoMatchingWatchlists_ReturnsEmpty()
    {
        await using var context = CreateContext();
        context.Watchlists.Add(CreateEstablishment("100001", OtherUser));
        await context.SaveChangesAsync();

        var sut = new WatchlistRepository(context);

        var result = await sut.GetEstablishmentsForUser(User, CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetEstablishmentsForUser_ReturnsEstablishmentsForThatUserOnly()
    {
        await using var context = CreateContext();
        context.Watchlists.AddRange(
            CreateEstablishment("100001", User),
            CreateEstablishment("100002", User),
            CreateEstablishment("100003", OtherUser));
        await context.SaveChangesAsync();

        var sut = new WatchlistRepository(context);

        var result = (await sut.GetEstablishmentsForUser(User, CancellationToken.None)).ToList();

        result.Should().HaveCount(2);
        result.Select(w => w.EstablishmentId).Should().BeEquivalentTo("100001", "100002");
        result.Should().OnlyContain(w => w.User == User);
    }

    [Fact]
    public async Task GetEstablishmentsForUser_DoesNotReturnTrusts()
    {
        await using var context = CreateContext();
        context.Watchlists.AddRange(
            CreateEstablishment("100001", User),
            CreateTrust("TR00001", User));
        await context.SaveChangesAsync();

        var sut = new WatchlistRepository(context);

        var result = (await sut.GetEstablishmentsForUser(User, CancellationToken.None)).ToList();

        result.Should().ContainSingle().Which.EstablishmentId.Should().Be("100001");
        result.Should().OnlyContain(w => !w.IsTrust);
    }

    [Fact]
    public async Task GetEstablishmentsForUser_ReturnsEstablishmentsOrderedByCreatedOn()
    {
        await using var context = CreateContext();
        var latest = CreateEstablishment("100003", User);
        var oldest = CreateEstablishment("100001", User);
        var middle = CreateEstablishment("100002", User);
        context.Watchlists.AddRange(latest, oldest, middle);
        await context.SaveChangesAsync();

        var now = DateTime.UtcNow;
        await SetCreatedOn(context, (latest, now), (oldest, now.AddDays(-2)), (middle, now.AddDays(-1)));

        var sut = new WatchlistRepository(context);

        var result = await sut.GetEstablishmentsForUser(User, CancellationToken.None);

        result.Select(w => w.EstablishmentId).Should().Equal("100001", "100002", "100003");
    }

    [Fact]
    public async Task GetEstablishmentsForUser_DoesNotReturnRowsForOtherUsers()
    {
        await using var context = CreateContext();
        var mine = CreateEstablishment("100001", User);
        context.Watchlists.AddRange(mine, CreateEstablishment("100002", OtherUser));
        await context.SaveChangesAsync();

        var sut = new WatchlistRepository(context);

        var result = (await sut.GetEstablishmentsForUser(User, CancellationToken.None)).ToList();

        result.Should().ContainSingle().Which.Id.Should().Be(mine.Id);
    }

    [Fact]
    public async Task GetTrustsForUser_WhenUserIsNull_ReturnsEmpty()
    {
        await using var context = CreateContext();
        var sut = new WatchlistRepository(context);

        var result = await sut.GetTrustsForUser(null!, CancellationToken.None);

        result.Should().BeEmpty();
        result.Should().BeAssignableTo<IEnumerable<Watchlist>>();
    }

    [Fact]
    public async Task GetTrustsForUser_WhenUserIsEmpty_ReturnsEmpty()
    {
        await using var context = CreateContext();
        var sut = new WatchlistRepository(context);

        var result = await sut.GetTrustsForUser(string.Empty, CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTrustsForUser_WhenNoMatchingWatchlists_ReturnsEmpty()
    {
        await using var context = CreateContext();
        context.Watchlists.Add(CreateTrust("TR00001", OtherUser));
        await context.SaveChangesAsync();

        var sut = new WatchlistRepository(context);

        var result = await sut.GetTrustsForUser(User, CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTrustsForUser_ReturnsTrustsForThatUserOnly()
    {
        await using var context = CreateContext();
        context.Watchlists.AddRange(
            CreateTrust("TR00001", User),
            CreateTrust("TR00002", User),
            CreateTrust("TR00003", OtherUser));
        await context.SaveChangesAsync();

        var sut = new WatchlistRepository(context);

        var result = (await sut.GetTrustsForUser(User, CancellationToken.None)).ToList();

        result.Should().HaveCount(2);
        result.Select(w => w.TrustId).Should().BeEquivalentTo("TR00001", "TR00002");
        result.Should().OnlyContain(w => w.User == User);
    }

    [Fact]
    public async Task GetTrustsForUser_DoesNotReturnEstablishments()
    {
        await using var context = CreateContext();
        context.Watchlists.AddRange(
            CreateEstablishment("100001", User),
            CreateTrust("TR00001", User));
        await context.SaveChangesAsync();

        var sut = new WatchlistRepository(context);

        var result = (await sut.GetTrustsForUser(User, CancellationToken.None)).ToList();

        result.Should().ContainSingle().Which.TrustId.Should().Be("TR00001");
        result.Should().OnlyContain(w => w.IsTrust);
    }

    [Fact]
    public async Task GetTrustsForUser_ReturnsTrustsOrderedByCreatedOn()
    {
        await using var context = CreateContext();
        var latest = CreateTrust("TR00003", User);
        var oldest = CreateTrust("TR00001", User);
        var middle = CreateTrust("TR00002", User);
        context.Watchlists.AddRange(latest, oldest, middle);
        await context.SaveChangesAsync();

        var now = DateTime.UtcNow;
        await SetCreatedOn(context, (latest, now), (oldest, now.AddDays(-2)), (middle, now.AddDays(-1)));

        var sut = new WatchlistRepository(context);

        var result = await sut.GetTrustsForUser(User, CancellationToken.None);

        result.Select(w => w.TrustId).Should().Equal("TR00001", "TR00002", "TR00003");
    }

    [Fact]
    public async Task GetTrustsForUser_DoesNotReturnRowsForOtherUsers()
    {
        await using var context = CreateContext();
        var mine = CreateTrust("TR00001", User);
        context.Watchlists.AddRange(mine, CreateTrust("TR00002", OtherUser));
        await context.SaveChangesAsync();

        var sut = new WatchlistRepository(context);

        var result = (await sut.GetTrustsForUser(User, CancellationToken.None)).ToList();

        result.Should().ContainSingle().Which.Id.Should().Be(mine.Id);
    }
}
