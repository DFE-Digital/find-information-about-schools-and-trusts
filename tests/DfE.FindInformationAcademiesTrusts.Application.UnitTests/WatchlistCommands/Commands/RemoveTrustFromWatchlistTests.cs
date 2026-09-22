using DfE.FindInformationAcademiesTrusts.Application.WatchlistCommands.Commands;
using DfE.FindInformationAcademiesTrusts.Domain.Entities;
using DfE.FindInformationAcademiesTrusts.Domain.Interfaces.Repositories;
using DfE.FindInformationAcademiesTrusts.Domain.ValueObjects;
using Moq;

namespace DfE.FindInformationAcademiesTrusts.Application.UnitTests.WatchlistCommands.Commands;

public class RemoveTrustFromWatchlistTests
{
    private readonly Mock<IWatchlistRepository> _mockWatchlistRepository;
    private readonly CancellationToken _cancellationToken;

    public RemoveTrustFromWatchlistTests()
    {
        _mockWatchlistRepository = new Mock<IWatchlistRepository>();
        _cancellationToken = CancellationToken.None;
    }

    private RemoveTrustFromWatchlist.RemoveTrustFromWatchlistCommandHandler CreateHandler() =>
        new(_mockWatchlistRepository.Object);

    [Fact]
    public async Task Handle_ValidCommand_RemovesExistingWatchlistRecordAndReturnsTrue()
    {
        var watchlistId = Guid.NewGuid();
        var existing = new Watchlist(new WatchlistId(watchlistId), null, "TR00001", true, "user@education.gov.uk");
        var command = new RemoveTrustFromWatchlistCommand(watchlistId);

        _mockWatchlistRepository
            .Setup(repo => repo.GetAsync(It.IsAny<object[]>()))
            .ReturnsAsync(existing);
        _mockWatchlistRepository
            .Setup(repo => repo.RemoveAsync(existing, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var result = await CreateHandler().Handle(command, _cancellationToken);

        Assert.True(result);
        _mockWatchlistRepository.Verify(repo => repo.GetAsync(
            It.Is<object[]>(keys => keys.Length > 0 && (WatchlistId)keys[0] == new WatchlistId(watchlistId))), Times.Once);
        _mockWatchlistRepository.Verify(repo => repo.RemoveAsync(existing, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WatchlistRecordNotFound_ExceptionPropagatesAndNothingIsRemoved()
    {
        var command = new RemoveTrustFromWatchlistCommand(Guid.NewGuid());

        _mockWatchlistRepository
            .Setup(repo => repo.GetAsync(It.IsAny<object[]>()))
            .ThrowsAsync(new InvalidOperationException("Not found"));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            CreateHandler().Handle(command, _cancellationToken));

        _mockWatchlistRepository.Verify(
            repo => repo.RemoveAsync(It.IsAny<Watchlist>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_RepositoryThrowsOnRemove_ExceptionPropagates()
    {
        var watchlistId = Guid.NewGuid();
        var existing = new Watchlist(new WatchlistId(watchlistId), null, "TR00001", true, "user");
        var command = new RemoveTrustFromWatchlistCommand(watchlistId);

        _mockWatchlistRepository
            .Setup(repo => repo.GetAsync(It.IsAny<object[]>()))
            .ReturnsAsync(existing);
        _mockWatchlistRepository
            .Setup(repo => repo.RemoveAsync(It.IsAny<Watchlist>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            CreateHandler().Handle(command, _cancellationToken));
    }
}
