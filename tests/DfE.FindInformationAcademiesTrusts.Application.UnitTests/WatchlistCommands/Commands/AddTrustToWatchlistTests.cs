using DfE.FindInformationAcademiesTrusts.Application.WatchlistCommands.Commands;
using DfE.FindInformationAcademiesTrusts.Domain.Entities;
using DfE.FindInformationAcademiesTrusts.Domain.Interfaces.Repositories;
using Moq;

namespace DfE.FindInformationAcademiesTrusts.Application.UnitTests.WatchlistCommands.Commands;

public class AddTrustToWatchlistTests
{
    private readonly Mock<IWatchlistRepository> _mockWatchlistRepository;
    private readonly CancellationToken _cancellationToken;

    public AddTrustToWatchlistTests()
    {
        _mockWatchlistRepository = new Mock<IWatchlistRepository>();
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task Handle_ValidCommand_AddsWatchlistRecordAndReturnsTrue()
    {
        const string trustId = "TR00001";
        const string user = "test.user@education.gov.uk";
        var command = new AddTrustToWatchlistCommand(trustId, user);

        _mockWatchlistRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Watchlist>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Watchlist w, CancellationToken _) => w);

        var handler = new AddTrustToWatchlist.AddTrustToWatchlistCommandHandler(_mockWatchlistRepository.Object);

        var result = await handler.Handle(command, _cancellationToken);

        Assert.True(result);
        _mockWatchlistRepository.Verify(repo => repo.AddAsync(
            It.Is<Watchlist>(w =>
                w.TrustId == trustId &&
                w.EstablishmentId == null &&
                w.IsTrust &&
                w.User == user &&
                w.Id.Value != Guid.Empty),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CalledTwice_CreatesRecordsWithDifferentIds()
    {
        var added = new List<Watchlist>();
        _mockWatchlistRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Watchlist>(), It.IsAny<CancellationToken>()))
            .Callback((Watchlist w, CancellationToken _) => added.Add(w))
            .ReturnsAsync((Watchlist w, CancellationToken _) => w);

        var handler = new AddTrustToWatchlist.AddTrustToWatchlistCommandHandler(_mockWatchlistRepository.Object);
        var command = new AddTrustToWatchlistCommand("TR00001", "user");

        await handler.Handle(command, _cancellationToken);
        await handler.Handle(command, _cancellationToken);

        Assert.Equal(2, added.Count);
        Assert.NotEqual(added[0].Id, added[1].Id);
    }

    [Fact]
    public async Task Handle_RepositoryThrowsException_ExceptionPropagates()
    {
        var command = new AddTrustToWatchlistCommand("TR00001", "user");

        _mockWatchlistRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Watchlist>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        var handler = new AddTrustToWatchlist.AddTrustToWatchlistCommandHandler(_mockWatchlistRepository.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(command, _cancellationToken));
    }
}
