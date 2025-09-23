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
    private readonly ITrustGovernanceRepository _mockTrustGovernanceRepository = Substitute.For<ITrustGovernanceRepository>();
    private readonly IContactRepository _mockContactRepository = Substitute.For<IContactRepository>();
    private readonly IDateTimeProvider _mockDateTimeProvider = Substitute.For<IDateTimeProvider>();
    private readonly MockMemoryCache _mockMemoryCache = new();

    public TrustServiceGovernanceTurnoverTests()
    {
        _sut = new TrustService(_mockAcademyRepository,
            _mockTrustRepository,
            _mockTrustGovernanceRepository,
            _mockContactRepository,
            _mockMemoryCache.Object,
            _mockDateTimeProvider);
    }

    [Fact]
    public void GetGovernanceTurnoverRate_returns_zero_when_no_current_governors()
    {
        _mockDateTimeProvider.Today.Returns(new DateTime(2023, 10, 1));

        var result = _sut.GetGovernanceTurnoverRate([]);

        result.Should().Be(0m);
    }

    [Theory]
    [InlineData("Accounting Officer")]
    [InlineData("Chair of Governors")]
    [InlineData("Chair of Local Governing Body")]
    [InlineData("Chief Financial Officer")]
    [InlineData("Governance Professional - Federation")]
    [InlineData("Governance Professional - Local Authority Maintained School")]
    [InlineData("Governance Professional - Multi-Academy Trust (MAT)")]
    [InlineData("Governance Professional - Single-Academy Trust (SAT)")]
    [InlineData("Governor")]
    [InlineData("Local Governance Professional - Individual Academy or Free School")]
    [InlineData("Local Governor")]
    [InlineData("Member")]
    [InlineData("Shared Chair of Local Governing Body - Establishment")]
    [InlineData("Shared Chair of Local Governing Body - Group")]
    [InlineData("Shared Governance Professional - Establishment")]
    [InlineData("Shared Governance Professional - Group")]
    [InlineData("Shared Local Governor - Establishment")]
    [InlineData("Shared Local Governor - Group")]
    public void GetGovernanceTurnoverRate_only_includes_trustees_and_the_chair_of_trustees(string role)
    {
        var unexpectedGovernor = new Governor("1", "UID", "John Doe", role, "Appointing Body", new DateTime(2023, 9, 1), null, null);

        _mockDateTimeProvider.Today.Returns(new DateTime(2023, 10, 1));

        var result = _sut.GetGovernanceTurnoverRate([ unexpectedGovernor ]);

        result.Should().Be(0m);
    }

    [Theory]
    [InlineData(2, 3, 10, 50.0)]
    [InlineData(1, 2, 4, 75.0)]
    [InlineData(1, 0, 3, 33.3)]
    [InlineData(0, 0, 10, 0.0)]
    [InlineData(5, 5, 5, 200.0)]
    [InlineData(0, 2, 9, 22.2)]
    public void GetGovernanceTurnoverRate_calculates_correct_turnover_rate(
        int appointments,
        int resignations,
        int currentGovernors,
        decimal expectedRate)
    {
        DateTime? eventDate = new DateTime(2023, 5, 1);
        _mockDateTimeProvider.Today.Returns(new DateTime(2023, 10, 1));

        var createGovernor = (int i) =>
        {
            var dateOfAppointment = i < appointments ? eventDate : null;
            var dateOfTermEnd = i >= currentGovernors ? eventDate : null;
            var role = i == 0 ? "Chair of Trustees" : "Trustee";

            return new Governor(
                "1",
                "UID",
                $"Trustee {i + 1}",
                role,
                "Appointing Body",
                dateOfAppointment,
                dateOfTermEnd,
                null
            );
        };
        
        var totalGovernors = currentGovernors + resignations;

        var governors = Enumerable
            .Range(0, totalGovernors)
            .Select(createGovernor)
            .ToList();

        var result = _sut.GetGovernanceTurnoverRate(governors);

        result.Should().Be(expectedRate);
    }

    [Fact]
    public void GetGovernanceTurnoverRate_should_not_include_Chair_of_Trustees_when_they_are_already_counted_as_a_Trustee_for_current_trustees()
    {
        _mockDateTimeProvider.Today.Returns(new DateTime(2023, 10, 1));

        var chair = new Governor(
            "1",
            "UID",
            "John Johnson",
            "Chair of Trustees",
            "Appointing Body",
            new DateTime(2020, 9, 1),
            new DateTime(2025, 9, 1),
            null
        );
        
        var trustee = new Governor(
            "1",
            "UID",
            "John Johnson",
            "Trustee",
            "Appointing Body",
            new DateTime(2020, 9, 1),
            new DateTime(2025, 9, 1),
            null
        );
        
        var resignedTrustee = new Governor(
            "1",
            "UID",
            "A",
            "Trustee",
            "Appointing Body",
            new DateTime(2020, 9, 1),
            new DateTime(2023, 9, 1),
            null
        );
        
        var result = _sut.GetGovernanceTurnoverRate([chair, trustee, resignedTrustee]);
        
        result.Should().Be(100m);
    }

    [Fact]
    public void GetGovernanceTurnoverRate_should_include_Chair_of_Trustees_when_they_are_not_counted_as_a_Trustee_for_current_trustees()
    {
        _mockDateTimeProvider.Today.Returns(new DateTime(2023, 10, 1));

        var chair = new Governor(
            "1",
            "UID",
            "John Johnson",
            "Chair of Trustees",
            "Appointing Body",
            new DateTime(2020, 9, 1),
            new DateTime(2025, 9, 1),
            null
        );
        
        var member = new Governor(
            "1",
            "UID",
            "John Johnson",
            "Member",
            "Appointing Body",
            new DateTime(2020, 9, 1),
            new DateTime(2025, 9, 1),
            null
        );
        
        var resignedTrustee = new Governor(
            "1",
            "UID",
            "A",
            "Trustee",
            "Appointing Body",
            new DateTime(2020, 9, 1),
            new DateTime(2023, 9, 1),
            null
        );
        
        var result = _sut.GetGovernanceTurnoverRate([chair, member, resignedTrustee]);
        
        result.Should().Be(100m);
    }

    [Fact]
    public void GetGovernanceTurnoverRate_should_not_include_Chair_of_Trustees_when_they_are_already_counted_as_a_Trustee_for_resigned_trustees()
    {
        _mockDateTimeProvider.Today.Returns(new DateTime(2023, 10, 1));

        var chair = new Governor(
            "1",
            "UID",
            "John Johnson",
            "Chair of Trustees",
            "Appointing Body",
            new DateTime(2020, 9, 1),
            new DateTime(2023, 9, 1),
            null
        );
        
        var trustee = new Governor(
            "1",
            "UID",
            "John Johnson",
            "Trustee",
            "Appointing Body",
            new DateTime(2020, 9, 1),
            new DateTime(2023, 9, 1),
            null
        );
        
        var activeTrustee = new Governor(
            "1",
            "UID",
            "A",
            "Trustee",
            "Appointing Body",
            new DateTime(2020, 9, 1),
            new DateTime(2025, 9, 1),
            null
        );
        
        var result = _sut.GetGovernanceTurnoverRate([chair, trustee, activeTrustee]);
        
        result.Should().Be(100m);
    }

    [Fact]
    public void GetGovernanceTurnoverRate_should_include_Chair_of_Trustees_when_they_are_not_counted_as_a_Trustee_for_resigned_trustees()
    {
        _mockDateTimeProvider.Today.Returns(new DateTime(2023, 10, 1));

        var chair = new Governor(
            "1",
            "UID",
            "John Johnson",
            "Chair of Trustees",
            "Appointing Body",
            new DateTime(2020, 9, 1),
            new DateTime(2023, 9, 1),
            null
        );
        
        var member = new Governor(
            "1",
            "UID",
            "John Johnson",
            "Member",
            "Appointing Body",
            new DateTime(2020, 9, 1),
            new DateTime(2023, 9, 1),
            null
        );
        
        var activeTrustee = new Governor(
            "1",
            "UID",
            "A",
            "Trustee",
            "Appointing Body",
            new DateTime(2020, 9, 1),
            new DateTime(2025, 9, 1),
            null
        );
        
        var result = _sut.GetGovernanceTurnoverRate([chair, member, activeTrustee]);
        
        result.Should().Be(100m);
    }
}
