using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.FiatDb.Repositories;
using DfE.FindInformationAcademiesTrusts.Data.Repositories;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Academy;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Trust;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using DfE.FindInformationAcademiesTrusts.UnitTests.Mocks;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Services;

public class TrustServiceGovernanceTurnoverTests
{
    private readonly TrustService _sut;
    private readonly IAcademyRepository _mockAcademyRepository = Substitute.For<IAcademyRepository>();
    private readonly ITrustRepository _mockTrustRepository = Substitute.For<ITrustRepository>();
    private readonly IContactRepository _mockContactRepository = Substitute.For<IContactRepository>();
    private readonly IDateTimeProvider _mockDateTimeProvider = Substitute.For<IDateTimeProvider>();
    private readonly MockMemoryCache _mockMemoryCache = new();

    public TrustServiceGovernanceTurnoverTests()
    {
        _sut = new TrustService(_mockAcademyRepository,
            _mockTrustRepository,
            _mockContactRepository,
            _mockMemoryCache.Object,
            _mockDateTimeProvider);
    }

    [Fact]
    public void GetGovernanceTurnoverRate_Returns_Zero_When_No_CurrentGovernors()
    {
        // Arrange
        var trustGovernance = new TrustGovernance(
            [],
            [],
            [],
            []
        );

        _mockDateTimeProvider.Today.Returns(new DateTime(2023, 10, 1));

        // Act
        var result = _sut.GetGovernanceTurnoverRate(trustGovernance);

        // Assert
        result.Should().Be(0m);
    }

    [Fact]
    public void GetGovernanceTurnoverRate_Calculates_CorrectTurnover()
    {
        // Arrange
        var startDate = new DateTime(2022, 1, 1);
        var endDate = new DateTime(2023, 1, 1);
        _mockDateTimeProvider.Today.Returns(new DateTime(2023, 10, 1));

        var governor = new Governor("1", "UID", "John Doe", "Member", "Appointing Body", startDate, endDate, null);
        var trustGovernance = new TrustGovernance(
            [],
            [governor],
            [],
            [governor]
        );

        // Act
        var result = _sut.GetGovernanceTurnoverRate(trustGovernance);

        // Assert
        result.Should().BeGreaterThan(0m); // Check if it calculates a rate instead of zero
    }

    [Fact]
    public void GetGovernorsExcludingLeadership_Excludes_LeadershipRoles()
    {
        // Arrange
        var leaderGovernor =
            new Governor("1", "UID", "John Doe", "Chair of Trustees", "Appointing Body", null, null, null);
        var trusteeGovernor = new Governor("2", "UID2", "Jane Doe", "Trustee", "Appointing Body", null, null, null);
        var trustGovernance = new TrustGovernance(
            [leaderGovernor],
            [trusteeGovernor],
            [],
            []
        );

        // Act
        var result = TrustService.GetGovernorsExcludingLeadership(trustGovernance);

        // Assert
        result.Should().Contain(trusteeGovernor);
        result.Should().NotContain(leaderGovernor); // Ensure leadership roles are excluded
    }

    [Fact]
    public void GetCurrentGovernors_Returns_AllCurrentMembersAndTrustees()
    {
        // Arrange
        var member = new Governor("1", "UID1", "John Doe", "Member", "Appointing Body", null, null, null);
        var trustee = new Governor("2", "UID2", "Jane Doe", "Trustee", "Appointing Body", null, null, null);
        var trustGovernance = new TrustGovernance(
            [],
            [member],
            [trustee],
            []
        );

        // Act
        var result = TrustService.GetCurrentGovernors(trustGovernance);

        // Assert
        result.Should().Contain(member);
        result.Should().Contain(trustee);
    }

    [Theory]
    [InlineData("2023-01-01", "2023-12-31", 2)]
    [InlineData("2022-01-01", "2022-05-15", 1)]
    [InlineData("2021-01-01", "2021-12-31", 0)]
    public void CountEventsWithinDateRange_Calculates_CorrectCount(
        string rangeStart, string rangeEnd, int expectedCount)
    {
        // Arrange
        var startDate = DateTime.Parse(rangeStart);
        var endDate = DateTime.Parse(rangeEnd);
        var governors = new List<Governor>
        {
            new("1", "UID1", "John Doe", "Trustee", "Appointing Body", new DateTime(2023, 1, 1), null, null),
            new("2", "UID2", "Jane Doe", "Member", "Appointing Body", new DateTime(2023, 5, 15), null, null),
            new("3", "UID3", "Jake Doe", "Trustee", "Appointing Body", new DateTime(2022, 5, 15), null, null)
        };

        // Act
        var result = TrustService.CountEventsWithinDateRange(governors, g => g.DateOfAppointment, startDate, endDate);

        // Assert
        result.Should().Be(expectedCount);
    }

    [Theory]
    [InlineData(0, 0, 0.0)]
    [InlineData(0, 10, 0.0)]
    public void CalculateTurnoverRate_Returns_Zero_When_TotalCurrentGovernors_Is_Zero(
        int totalCurrentGovernors, int totalEvents, decimal expectedRate)
    {
        // Act
        var result = TrustService.CalculateTurnoverRate(totalCurrentGovernors, totalEvents);

        // Assert
        result.Should().Be(expectedRate);
    }

    [Theory]
    [InlineData(10, 5, 50.0)]
    [InlineData(4, 3, 75.0)]
    [InlineData(3, 1, 33.3)]
    [InlineData(10, 0, 0.0)]
    public void CalculateTurnoverRate_Calculates_CorrectRate_When_TotalCurrentGovernors_Is_Greater_Than_Zero(
        int totalCurrentGovernors, int totalEvents, decimal expectedRate)
    {
        // Act
        var result = TrustService.CalculateTurnoverRate(totalCurrentGovernors, totalEvents);

        // Assert
        result.Should().Be(expectedRate);
    }
}
