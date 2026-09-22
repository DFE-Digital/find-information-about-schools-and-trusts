using DfE.FindInformationAcademiesTrusts.Application.WatchlistCommands.Queries;
using DfE.FindInformationAcademiesTrusts.Domain.Entities;
using DfE.FindInformationAcademiesTrusts.Domain.Interfaces.Repositories;
using DfE.FindInformationAcademiesTrusts.Domain.ValueObjects;
using Moq;

namespace DfE.FindInformationAcademiesTrusts.Application.UnitTests.WatchlistCommands.Queries;

public class WatchlistQueryServiceTests
{
    private const string User = "user@education.gov.uk";

    private readonly Mock<IWatchlistRepository> _mockWatchlistRepository;
    private readonly WatchlistQueryService _service;
    private readonly CancellationToken _cancellationToken;

    public WatchlistQueryServiceTests()
    {
        _mockWatchlistRepository = new Mock<IWatchlistRepository>();
        _service = new WatchlistQueryService(_mockWatchlistRepository.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task GetAllEstablishmentsForUser_ReturnsSuccessWithWatchlists_WhenRepositoryReturnsWatchlists()
    {
        var watchlists = new[]
        {
            new Watchlist(new WatchlistId(Guid.NewGuid()), "100001", null, false, User),
            new Watchlist(new WatchlistId(Guid.NewGuid()), "100002", null, false, User),
            new Watchlist(new WatchlistId(Guid.NewGuid()), "100003", null, false, User)
        };

        _mockWatchlistRepository
            .Setup(r => r.GetEstablishmentsForUser(User, It.IsAny<CancellationToken>()))
            .ReturnsAsync(watchlists);

        var result = await _service.GetAllEstablishmentsForUser(User, _cancellationToken);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Error);
        Assert.Equal(watchlists, result.Value);
        _mockWatchlistRepository.Verify(
            r => r.GetEstablishmentsForUser(User, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllEstablishmentsForUser_ReturnsSuccessWithEmptySequence_WhenRepositoryReturnsNone()
    {
        _mockWatchlistRepository
            .Setup(r => r.GetEstablishmentsForUser(User, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Watchlist>());

        var result = await _service.GetAllEstablishmentsForUser(User, _cancellationToken);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value!);
    }

    [Fact]
    public async Task GetAllEstablishmentsForUser_PropagatesException_WhenRepositoryThrows()
    {
        _mockWatchlistRepository
            .Setup(r => r.GetEstablishmentsForUser(User, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.GetAllEstablishmentsForUser(User, _cancellationToken));
    }

    [Fact]
    public async Task GetAllTrustsForUser_ReturnsSuccessWithWatchlists_WhenRepositoryReturnsWatchlists()
    {
        var watchlists = new[]
        {
            new Watchlist(new WatchlistId(Guid.NewGuid()), null, "TR00001", true, User),
            new Watchlist(new WatchlistId(Guid.NewGuid()), null, "TR00002", true, User)
        };

        _mockWatchlistRepository
            .Setup(r => r.GetTrustsForUser(User, It.IsAny<CancellationToken>()))
            .ReturnsAsync(watchlists);

        var result = await _service.GetAllTrustsForUser(User, _cancellationToken);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Error);
        Assert.Equal(watchlists, result.Value);
        _mockWatchlistRepository.Verify(
            r => r.GetTrustsForUser(User, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllTrustsForUser_ReturnsSuccessWithEmptySequence_WhenRepositoryReturnsNone()
    {
        _mockWatchlistRepository
            .Setup(r => r.GetTrustsForUser(User, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Watchlist>());

        var result = await _service.GetAllTrustsForUser(User, _cancellationToken);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value!);
    }

    [Fact]
    public async Task GetAllTrustsForUser_PropagatesException_WhenRepositoryThrows()
    {
        _mockWatchlistRepository
            .Setup(r => r.GetTrustsForUser(User, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.GetAllTrustsForUser(User, _cancellationToken));
    }
}
