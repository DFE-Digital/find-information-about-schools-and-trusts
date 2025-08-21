using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Data.FiatDb.Repositories;
using DfE.FindInformationAcademiesTrusts.Data.Repositories;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Academy;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Contacts;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Trust;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using DfE.FindInformationAcademiesTrusts.UnitTests.Mocks;
using NSubstitute.ReturnsExtensions;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Services;

public class TrustServiceTests
{
    private static readonly TrustOverview BaseTrustOverview = new("2806",
        "TR0012",
        "10012345",
        "123456",
        "Single-academy trust",
        "123 Fairyland Drive, Gotham, GT12 1AB",
        "Oxfordshire",
        new DateTime(2007, 6, 28));

    private readonly TrustService _sut;
    private readonly IAcademyRepository _mockAcademyRepository = Substitute.For<IAcademyRepository>();
    private readonly ITrustRepository _mockTrustRepository = Substitute.For<ITrustRepository>();
    private readonly ITrustGovernanceRepository _mockTrustGovernanceRepository = Substitute.For<ITrustGovernanceRepository>();
    private readonly IContactRepository _mockContactRepository = Substitute.For<IContactRepository>();
    private readonly IDateTimeProvider _mockDateTimeProvider = Substitute.For<IDateTimeProvider>();
    private readonly MockMemoryCache _mockMemoryCache = new();

    public TrustServiceTests()
    {
        _sut = new TrustService(_mockAcademyRepository,
            _mockTrustRepository,
            _mockTrustGovernanceRepository,
            _mockContactRepository,
            _mockMemoryCache.Object,
            _mockDateTimeProvider);

        _mockAcademyRepository.GetSingleAcademyTrustAcademyUrnAsync(Arg.Any<string>())
            .Returns(Task.FromResult<string?>(null));
    }

    [Fact]
    public async Task GetTrustSummaryAsync_cached_should_return_cached_result()
    {
        var uid = "1234";
        var key = $"{nameof(TrustService)}:{uid}";
        var cachedResult = new TrustSummaryServiceModel(uid, "My Trust", "Multi-academy trust", 3);
        _mockMemoryCache.AddMockCacheEntry(key, cachedResult);

        var result = await _sut.GetTrustSummaryAsync(uid);
        result.Should().Be(cachedResult);

        await _mockTrustRepository.DidNotReceive().GetTrustSummaryAsync(uid);
        await _mockAcademyRepository.DidNotReceive().GetNumberOfAcademiesInTrustAsync(uid);
    }

    [Fact]
    public async Task GetTrustSummaryAsync_should_return_null_if_no_giasGroup_found()
    {
        _mockTrustRepository.GetTrustSummaryAsync("this uid doesn't exist")
            .Returns(Task.FromResult<TrustSummary?>(null));

        var result = await _sut.GetTrustSummaryAsync("this uid doesn't exist");
        result.Should().BeNull();
    }

    [Theory]
    [InlineData("2806", "My Trust", "Multi-academy trust", 3)]
    [InlineData("9008", "Another Trust", "Single-academy trust", 1)]
    [InlineData("9008", "Trust with no academies", "Multi-academy trust", 0)]
    public async Task GetTrustSummaryAsync_should_return_trustSummary_if_found(string uid, string name, string type,
        int numAcademies)
    {
        _mockTrustRepository.GetTrustSummaryAsync(uid)!.Returns(Task.FromResult(new TrustSummary(name, type)));
        _mockAcademyRepository.GetNumberOfAcademiesInTrustAsync(uid).Returns(Task.FromResult(numAcademies));

        var result = await _sut.GetTrustSummaryAsync(uid);
        result.Should().BeEquivalentTo(new TrustSummaryServiceModel(uid, name, type, numAcademies));
    }

    [Theory]
    [InlineData("2806", "My Trust", "Multi-academy trust", 3)]
    [InlineData("9008", "Another Trust", "Single-academy trust", 1)]
    [InlineData("9008", "Trust with no academies", "Multi-academy trust", 0)]
    public async Task GetTrustSummaryAsync_uncached_should_cache_result(string uid, string name, string type,
        int numAcademies)
    {
        var key = $"{nameof(TrustService)}:{uid}";

        _mockTrustRepository.GetTrustSummaryAsync(uid)!.Returns(Task.FromResult(new TrustSummary(name, type)));
        _mockAcademyRepository.GetNumberOfAcademiesInTrustAsync(uid).Returns(Task.FromResult(numAcademies));

        await _sut.GetTrustSummaryAsync(uid);

        _mockMemoryCache.Object.Received(1).CreateEntry(key);

        var cachedEntry = _mockMemoryCache.MockCacheEntries[key];

        cachedEntry.Value.Should().BeEquivalentTo(new TrustSummaryServiceModel(uid, name, type, numAcademies));
        cachedEntry.SlidingExpiration.Should().Be(TimeSpan.FromMinutes(10));
    }

    [Fact]
    public async Task GetTrustGovernanceAsync_should_get_governanceResults_for_single_trust()
    {
        var startDate = DateTime.Today.AddYears(-3);
        var futureEndDate = DateTime.Today.AddYears(1);
        var historicEndDate = DateTime.Today.AddYears(-1);
        var today = new DateTime(2023, 10, 1);
        _mockDateTimeProvider.Today.Returns(today);
        var member = new Governor(
            "9999",
            "1234",
            Role: "Member",
            FullName: "First Second Last",
            DateOfAppointment: startDate,
            DateOfTermEnd: futureEndDate,
            AppointingBody: "Nick Warms",
            Email: null
        );
        var trustee = new Governor(
            "9998",
            "1234",
            Role: "Trustee",
            FullName: "First Second Last",
            DateOfAppointment: startDate,
            DateOfTermEnd: futureEndDate,
            AppointingBody: "Nick Warms",
            Email: null
        );
        var leader = new Governor(
            "9999",
            "1234",
            Role: "Chair of Trustees",
            FullName: "First Second Last",
            DateOfAppointment: startDate,
            DateOfTermEnd: futureEndDate,
            AppointingBody: "Nick Warms",
            Email: null
        );
        var historic = new Governor(
            "9999",
            "1234",
            Role: "Trustee",
            FullName: "First Second Last",
            DateOfAppointment: startDate,
            DateOfTermEnd: historicEndDate,
            AppointingBody: "Nick Warms",
            Email: null
        );
        _mockTrustGovernanceRepository.GetTrustGovernanceAsync("1234").Returns([leader, member, trustee, historic]);

        var result = await _sut.GetTrustGovernanceAsync("1234");

        result.HistoricMembers.Should().ContainSingle().Which.Should().BeEquivalentTo(historic);
        result.CurrentMembers.Should().ContainSingle().Which.Should().BeEquivalentTo(member);
        result.CurrentTrustees.Should().ContainSingle().Which.Should().BeEquivalentTo(trustee);
        result.CurrentTrustLeadership.Should().ContainSingle().Which.Should().BeEquivalentTo(leader);
    }

    [Fact]
    public async Task GetTrustContactsAsync_should_get_governanceResults_for_single_trust()
    {
        var person = new Person("First Middle Last", "firstlast@email.com");
        var contacts = new TrustContacts(person, person, person);
        _mockTrustRepository.GetTrustContactsAsync("1234").Returns(Task.FromResult(contacts));
        var internalContact =
            new InternalContact("First Middle Last", "firstlast@email.com", DateTime.Today, "Test@email.com");
        var internalContacts = new TrustInternalContacts(internalContact, internalContact);
        _mockContactRepository.GetTrustInternalContactsAsync("1234").Returns(Task.FromResult(internalContacts));

        var result = await _sut.GetTrustContactsAsync("1234");

        result.Should().BeEquivalentTo(contacts);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("123456")]
    [InlineData("567890")]
    public async Task
        GetTrustOverviewAsync_should_get_singleAcademyTrustAcademyUrn_from_academy_repository_when_trust_is_single_academy_trust(
            string? singleAcademyTrustAcademyUrn)
    {
        _mockAcademyRepository.GetSingleAcademyTrustAcademyUrnAsync("2806")
            .Returns(Task.FromResult(singleAcademyTrustAcademyUrn));
        _mockTrustRepository.GetTrustOverviewAsync("2806").Returns(Task.FromResult(BaseTrustOverview));

        var result = await _sut.GetTrustOverviewAsync("2806");

        result.SingleAcademyTrustAcademyUrn.Should().Be(singleAcademyTrustAcademyUrn);
    }

    [Fact]
    public async Task
        GetTrustOverviewAsync_should_not_get_singleAcademyTrustAcademyUrn_when_trust_is_multi_academy_trust()
    {
        _mockTrustRepository.GetTrustOverviewAsync("2806")
            .Returns(Task.FromResult(BaseTrustOverview with { Type = "Multi-academy trust" }));

        var result = await _sut.GetTrustOverviewAsync("2806");

        result.SingleAcademyTrustAcademyUrn.Should().BeNull();
        await _mockAcademyRepository.DidNotReceive().GetSingleAcademyTrustAcademyUrnAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task GetTrustOverviewAsync_should_set_properties_from_TrustRepo()
    {
        var trustOverview = BaseTrustOverview with
        {
            Uid = "6798",
            GroupId = "TR0034",
            Ukprn = "100999999",
            CompaniesHouseNumber = "999999",
            Address = "99 The Road, Townville, TA9 9CB",
            RegionAndTerritory = "Cumbria",
            OpenedDate = new DateTime(2015, 4, 20)
        };

        _mockTrustRepository.GetTrustOverviewAsync("6798").Returns(Task.FromResult(trustOverview));

        var result = await _sut.GetTrustOverviewAsync("6798");

        result.Should()
            .BeEquivalentTo(trustOverview, options => options.ExcludingMissingMembers().Excluding(t => t.Type));
    }

    [Theory]
    [InlineData("Single-academy trust", TrustType.SingleAcademyTrust)]
    [InlineData("Multi-academy trust", TrustType.MultiAcademyTrust)]
    public async Task GetTrustOverviewAsync_should_set_trustType(string givenType, TrustType expectedTrustType)
    {
        _mockTrustRepository.GetTrustOverviewAsync("2806")
            .Returns(Task.FromResult(BaseTrustOverview with { Type = givenType }));

        var result = await _sut.GetTrustOverviewAsync("2806");

        result.Type.Should().Be(expectedTrustType);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Not a SAT or MAT")]
    public async Task GetTrustOverviewAsync_should_throw_when_trustType_invalid(string givenType)
    {
        _mockTrustRepository.GetTrustOverviewAsync("2806")
            .Returns(Task.FromResult(BaseTrustOverview with { Type = givenType }));

        var action = async () => await _sut.GetTrustOverviewAsync("2806");

        await action.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task GetTrustOverviewAsync_returns_correct_overview_for_trust_with_academies()
    {
        // Arrange
        var uid = "1234";
        var academiesOverview = new AcademyOverview[]
        {
            new("1001", "LocalAuthorityA", 500, 600),
            new("1002", "LocalAuthorityB", 400, 500),
            new("1003", "LocalAuthorityA", 300, 400)
        };

        _mockAcademyRepository.GetOverviewOfAcademiesInTrustAsync(uid).Returns(Task.FromResult(academiesOverview));
        _mockTrustRepository.GetTrustOverviewAsync(uid).Returns(Task.FromResult(BaseTrustOverview with { Uid = uid }));

        // Act
        var result = await _sut.GetTrustOverviewAsync(uid);

        // Assert
        result.Uid.Should().Be(uid);
        result.TotalAcademies.Should().Be(3);
        result.AcademiesByLocalAuthority.Should().BeEquivalentTo(new Dictionary<string, int>
        {
            { "LocalAuthorityA", 2 },
            { "LocalAuthorityB", 1 }
        });
        result.TotalPupilNumbers.Should().Be(500 + 400 + 300);
        result.TotalCapacity.Should().Be(600 + 500 + 400);
    }

    [Fact]
    public async Task GetTrustOverviewAsync_returns_zero_values_for_trust_with_no_academies()
    {
        // Arrange
        var uid = "1234";
        var academiesOverview = Array.Empty<AcademyOverview>();

        _mockAcademyRepository.GetOverviewOfAcademiesInTrustAsync(uid).Returns(Task.FromResult(academiesOverview));
        _mockTrustRepository.GetTrustOverviewAsync(uid).Returns(Task.FromResult(BaseTrustOverview with { Uid = uid }));

        // Act
        var result = await _sut.GetTrustOverviewAsync(uid);

        // Assert
        result.Uid.Should().Be(uid);
        result.TotalAcademies.Should().Be(0);
        result.AcademiesByLocalAuthority.Should().BeEmpty();
        result.TotalPupilNumbers.Should().Be(0);
        result.TotalCapacity.Should().Be(0);
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public async Task UpdateContactsAsync_returns_the_correct_values_changed(bool emailUpdated, bool nameUpdated)
    {
        var expected = new InternalContactUpdated(emailUpdated, nameUpdated);
        _mockContactRepository.UpdateTrustInternalContactsAsync(1234, "Name", "Email", TrustContactRole.SfsoLead)
            .Returns(Task.FromResult(expected));
        var result = await _sut.UpdateContactAsync(1234, "Name", "Email", TrustContactRole.SfsoLead);

        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetAcademyTrustTrustReferenceNumberAsync_should_return_trust_reference_number_when_found()
    {
        // Arrange
        const string uid = "1234";
        const string expectedTrustReferenceNumber = "TRUST123";
        _mockTrustRepository
            .GetTrustReferenceNumberAsync(uid)
            .Returns(Task.FromResult(expectedTrustReferenceNumber));

        // Act
        var result = await _sut.GetTrustReferenceNumberAsync(uid);

        // Assert
        result.Should().Be(expectedTrustReferenceNumber);
    }

    [Fact]
    public async Task GetTrustSummaryAsync_should_return_null_if_trust_uid_is_null()
    {
        var urn = 123;

        _mockAcademyRepository.GetTrustUidFromAcademyUrnAsync(urn).ReturnsNull();

        var result = await _sut.GetTrustSummaryAsync(urn);

        result.Should().BeNull();
        await _mockTrustRepository.Received(0).GetTrustSummaryAsync(Arg.Any<string>());
    }

    [Theory]
    [InlineData(123, "2806", "My Trust", "Multi-academy trust", 3)]
    public async Task GetTrustSummaryAsync_should_return_trust_summary_if_found(int urn, string uid, string name,
        string type, int numAcademies)
    {
        _mockAcademyRepository.GetTrustUidFromAcademyUrnAsync(urn).Returns(uid);

        _mockTrustRepository.GetTrustSummaryAsync(uid).Returns(new TrustSummary(name, type));
        _mockAcademyRepository.GetNumberOfAcademiesInTrustAsync(uid).Returns(numAcademies);

        var result = await _sut.GetTrustSummaryAsync(urn);
        result.Should().BeEquivalentTo(new TrustSummaryServiceModel(uid, name, type, numAcademies));
    }
}
