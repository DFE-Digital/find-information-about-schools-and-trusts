using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Domain.Enums;
using DfE.FindInformationAcademiesTrusts.Data.Repositories;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Academy;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Trust;
using DfE.FindInformationAcademiesTrusts.HttpServices;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using DfE.FindInformationAcademiesTrusts.UnitTests.Mocks;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Establishments;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Trusts;
using NSubstitute.ReturnsExtensions;
using Person = DfE.FindInformationAcademiesTrusts.Data.Person;

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
    private readonly IGetEstablishmentsTemp _mockGetEstablishments = Substitute.For<IGetEstablishmentsTemp>();
    private readonly ITrustRepository _mockTrustRepository = Substitute.For<ITrustRepository>();
    private readonly ITrustGovernanceProvider _mockTrustGovernanceProvider = Substitute.For<ITrustGovernanceProvider>();
    private readonly ITrustPupilService _mockTrustPupilService = Substitute.For<ITrustPupilService>();
    private readonly IDateTimeProvider _mockDateTimeProvider = Substitute.For<IDateTimeProvider>();
    private readonly ITrustOverviewProvider _mockTrustOverviewProvider = Substitute.For<ITrustOverviewProvider>();
    private readonly MockMemoryCache _mockMemoryCache = new();

    private readonly DateTime _lastYear = DateTime.Today.AddYears(-1);
    private readonly DateTime _nextYear = DateTime.Today.AddYears(1);

    public TrustServiceTests()
    {
        _sut = new TrustService(_mockAcademyRepository,
            _mockGetEstablishments,
            _mockTrustRepository,
            _mockTrustGovernanceProvider,
            _mockTrustPupilService,
            _mockMemoryCache.Object,
            _mockDateTimeProvider,
            _mockTrustOverviewProvider);

        _mockGetEstablishments.GetEstablishmentsByTrustReferenceNumber(Arg.Any<string>())
            .Returns([]);
        _mockAcademyRepository.GetOverviewOfAcademiesInTrustAsync(Arg.Any<string>())
            .Returns([]);
        _mockDateTimeProvider.Today.Returns(DateTime.Today);
    }

    [Fact]
    public async Task GetTrustSummaryAsync_cached_should_return_cached_result()
    {
        var uid = "1234";
        var referencenumber = "TR1234";
        var key = $"{nameof(TrustService)}:{referencenumber}";
        var cachedResult = new TrustSummaryServiceModel(uid, referencenumber, "My Trust", "Multi-academy trust", 3);
        _mockMemoryCache.AddMockCacheEntry(key, cachedResult);

        var result = await _sut.GetTrustSummaryAsync(referencenumber);
        result.Should().Be(cachedResult);

        await _mockTrustOverviewProvider.DidNotReceive().GetTrustByReferenceNumber(referencenumber);
        await _mockGetEstablishments.DidNotReceive().GetEstablishmentsByTrustReferenceNumber(Arg.Any<string>());
    }

    [Fact]
    public async Task GetTrustSummaryAsync_should_return_null_if_no_giasGroup_found()
    {
        // _mockTrustRepository.GetTrustSummaryAsync("this uid doesn't exist")
        //     .ReturnsNull();
        
        _mockTrustOverviewProvider.GetTrustByReferenceNumber("this uid doesn't exist").Returns((TrustDto?)null);

        var result = await _sut.GetTrustSummaryAsync("this uid doesn't exist");
        result.Should().BeNull();
    }

    [Theory]
    [InlineData("2806", "TR2806", "My Trust", "Multi-academy trust", 3)]
    [InlineData("9008", "TR9008", "Another Trust", "Single-academy trust", 1)]
    [InlineData("9008", "TR9008", "Trust with no academies", "Multi-academy trust", 0)]
    public async Task GetTrustSummaryAsync_should_return_trustSummary_if_found(string uid, string referenceNumber,
        string name, string type, int numAcademies)
    {
        // _mockTrustRepository.GetTrustSummaryAsync(referenceNumber)!
        //     .Returns(new TrustSummary(name, type, uid, referenceNumber));
        _mockTrustOverviewProvider.GetTrustByReferenceNumber(referenceNumber).Returns(new TrustDto
        {
            GroupUid = uid,
            Name = name,
            ReferenceNumber = referenceNumber,
            Type = new NameAndCodeDto { Name = type }
        });
        SetupEstablishmentsInTrust(referenceNumber, numAcademies);

        var result = await _sut.GetTrustSummaryAsync(referenceNumber);
        result.Should().BeEquivalentTo(new TrustSummaryServiceModel(uid, referenceNumber, name, type, numAcademies));
    }

    [Theory]
    [InlineData("2806", "TR2806", "My Trust", "Multi-academy trust", 3)]
    [InlineData("9008", "TR9008", "Another Trust", "Single-academy trust", 1)]
    [InlineData("9008", "TR9008", "Trust with no academies", "Multi-academy trust", 0)]
    public async Task GetTrustSummaryAsync_uncached_should_cache_result(string uid, string referenceNumber,
        string name, string type, int numAcademies)
    {
        var key = $"{nameof(TrustService)}:{referenceNumber}";

        // _mockTrustRepository.GetTrustSummaryAsync(referenceNumber)!
        //     .Returns(new TrustSummary(name, type, uid, referenceNumber));
        _mockTrustOverviewProvider.GetTrustByReferenceNumber(referenceNumber).Returns(new TrustDto
        {
            GroupUid = uid,
            Name = name,
            ReferenceNumber = referenceNumber,
            Type = new NameAndCodeDto { Name = type }
        });
        SetupEstablishmentsInTrust(referenceNumber, numAcademies);

        await _sut.GetTrustSummaryAsync(referenceNumber);

        _mockMemoryCache.Object.Received(1).CreateEntry(key);

        var cachedEntry = _mockMemoryCache.MockCacheEntries[key];

        cachedEntry.Value.Should()
            .BeEquivalentTo(new TrustSummaryServiceModel(uid, referenceNumber, name, type, numAcademies));
        cachedEntry.SlidingExpiration.Should().Be(TimeSpan.FromMinutes(10));
    }

    [Fact]
    public async Task GetTrustGovernanceAsync_should_get_governanceResults_for_single_trust()
    {
        var startDate = DateTime.Today.AddYears(-3);
        var futureEndDate = DateTime.Today.AddYears(1);
        var historicEndDate = DateTime.Today.AddYears(-1);
        var today = new DateTime(2023, 10, 1);
        const string trn = "1234";
        
        _mockDateTimeProvider.Today.Returns(today);
        var member = new Governor(
            Role: "Member",
            FullName: "First Second Last",
            DateOfAppointment: startDate,
            DateOfTermEnd: futureEndDate,
            AppointingBody: "Nick Warms",
            Email: null
        );
        var trustee = new Governor(
            Role: "Trustee",
            FullName: "First Second Last",
            DateOfAppointment: startDate,
            DateOfTermEnd: futureEndDate,
            AppointingBody: "Nick Warms",
            Email: null
        );
        var leader = new Governor(
            Role: "Chair of Trustees",
            FullName: "First Second Last",
            DateOfAppointment: startDate,
            DateOfTermEnd: futureEndDate,
            AppointingBody: "Nick Warms",
            Email: null
        );
        var historic = new Governor(
            Role: "Trustee",
            FullName: "First Second Last",
            DateOfAppointment: startDate,
            DateOfTermEnd: historicEndDate,
            AppointingBody: "Nick Warms",
            Email: null
        );
        _mockTrustOverviewProvider.GetTrustOverviewAsync(trn)
            .Returns(BaseTrustOverview with { Type = "Multi-academy trust" });
        CreateGovernor(trn, leader, member, trustee, historic);

        var result = await _sut.GetTrustGovernanceAsync(trn);

        result!.HistoricMembers.Should().ContainSingle().Which.Should().BeEquivalentTo(historic);
        result.CurrentMembers.Should().ContainSingle().Which.Should().BeEquivalentTo(member);
        result.CurrentTrustees.Should().ContainSingle().Which.Should().BeEquivalentTo(trustee);
        result.CurrentTrustLeadership.Should().ContainSingle().Which.Should().BeEquivalentTo(leader);
    }

    [Fact]
    public async Task GetTrustGovernanceAsync_should_return_no_governors_when_none_exist_for_multi_academy_trust()
    {
        const string trn = "1234";
        _mockTrustOverviewProvider.GetTrustOverviewAsync(trn)
            .Returns(BaseTrustOverview with { Type = "Multi-academy trust" });
        CreateGovernor(trn);

        var result = await _sut.GetTrustGovernanceAsync(trn);

        result.Should().BeEquivalentTo(new TrustGovernanceServiceModel([], [], [], [], 0m));
    }

    [Fact]
    public async Task GetTrustGovernanceAsync_should_return_only_governors_for_that_multi_academy_trust()
    {
        const string trn = "1234";
        var unexpectedGovernor1 = new Governor("Billy Willy Boatface", "Member", "Some Org", _lastYear, _nextYear, null);
        var unexpectedGovernor2 =
            new Governor("Milly Tilly Planeface", "Trustee", "Some other Org", _lastYear, _nextYear, null);
        var expectedGovernor = new Governor("Hilly Jilly Trainface", "Member", "Some third Org", _lastYear, _nextYear, null);

        _mockTrustOverviewProvider.GetTrustOverviewAsync(trn)
            .Returns(BaseTrustOverview with { Type = "Multi-academy trust" });
        CreateGovernor("9876", unexpectedGovernor1, unexpectedGovernor2);
        CreateGovernor(trn, expectedGovernor);

        var result = await _sut.GetTrustGovernanceAsync(trn);

        result!.CurrentMembers.Should().BeEquivalentTo([expectedGovernor]);
        result.CurrentTrustees.Should().BeEmpty();
        result.CurrentTrustLeadership.Should().BeEmpty();
        result.HistoricMembers.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTrustGovernanceAsync_should_return_no_governors_when_none_exist_for_single_academy_trust()
    {
        const string trn = "TR0012";
        const int urn = 1234;
        _mockTrustOverviewProvider.GetTrustOverviewAsync(trn).Returns(BaseTrustOverview);
        SetupEstablishmentsInTrust(trn, urn.ToString());
        SetupSatGovernors(urn);

        var result = await _sut.GetTrustGovernanceAsync(trn);

        result.Should().BeEquivalentTo(new TrustGovernanceServiceModel([], [], [], [], 0m));
    }

    [Fact]
    public async Task GetTrustGovernanceAsync_should_return_only_governors_for_that_single_academy_trust()
    {
        const string trn = "TR0012";
        const int urn = 1234;
        var unexpectedGovernor1 = new Governor("Billy Willy Boatface", "Member", "Some Org", _lastYear, _nextYear, null);
        var unexpectedGovernor2 =
            new Governor("Milly Tilly Planeface", "Trustee", "Some other Org", _lastYear, _nextYear, null);
        var expectedGovernor = new Governor("Hilly Jilly Trainface", "Member", "Some third Org", _lastYear, _nextYear, null);

        _mockTrustOverviewProvider.GetTrustOverviewAsync(trn).Returns(BaseTrustOverview);
        SetupEstablishmentsInTrust(trn, urn.ToString());
        SetupSatGovernors(9876, unexpectedGovernor1, unexpectedGovernor2);
        SetupSatGovernors(urn, expectedGovernor);

        var result = await _sut.GetTrustGovernanceAsync(trn);

        result!.CurrentMembers.Should().BeEquivalentTo([expectedGovernor]);
        result.CurrentTrustees.Should().BeEmpty();
        result.CurrentTrustLeadership.Should().BeEmpty();
        result.HistoricMembers.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTrustContactsAsync_should_get_governanceResults_for_multi_trust()
    {
        _mockTrustOverviewProvider.GetTrustOverviewAsync("TR5678").Returns(BaseTrustOverview with { Type = "Multi-academy trust" });
        var person = new Person("First Middle Last", "firstlast@email.com");
        var contacts = new TrustContacts(person, person, person);
        _mockTrustRepository.GetTrustContactsAsync("1234").Returns(contacts);

        var result = await _sut.GetTrustContactsAsync("1234", "TR5678");

        result.Should().BeEquivalentTo(contacts);
    }
    
    [Fact]
    public async Task GetTrustContactsAsync_should_get_governanceResults_for_single_trust()
    {
        _mockTrustOverviewProvider.GetTrustOverviewAsync("TR5678").Returns(BaseTrustOverview);
        var person = new Person("First Middle Last", "firstlast@email.com");
        var contacts = new TrustContacts(person, person, person);
        _mockTrustRepository.GetTrustContactsAsync("1234").Returns(contacts);

        var result = await _sut.GetTrustContactsAsync("1234", "TR5678");

        result.Should().BeEquivalentTo(contacts);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("123456")]
    [InlineData("567890")]
    public async Task
        GetTrustOverviewAsync_should_get_singleAcademyTrustAcademyUrn_from_establishments_in_trust_when_trust_is_single_academy_trust(
            string? singleAcademyTrustAcademyUrn)
    {
        SetupEstablishmentsInTrust("TR0012", singleAcademyTrustAcademyUrn);
        _mockTrustOverviewProvider.GetTrustOverviewAsync("TR0012").Returns(BaseTrustOverview);

        var result = await _sut.GetTrustOverviewAsync("TR0012", "2806");

        result.SingleAcademyTrustAcademyUrn.Should().Be(singleAcademyTrustAcademyUrn);
    }

    [Fact]
    public async Task
        GetTrustOverviewAsync_should_not_get_singleAcademyTrustAcademyUrn_when_trust_is_multi_academy_trust()
    {
        _mockTrustOverviewProvider.GetTrustOverviewAsync("TR0012")
            .Returns(BaseTrustOverview with { Type = "Multi-academy trust" });

        var result = await _sut.GetTrustOverviewAsync("TR0012", "2806");

        result.SingleAcademyTrustAcademyUrn.Should().BeNull();
        await _mockGetEstablishments.DidNotReceive().GetEstablishmentsByTrustReferenceNumber(Arg.Any<string>());
    }

    [Fact]
    public async Task GetTrustOverviewAsync_should_set_properties_from_TrustRepo()
    {
        var trustOverview = BaseTrustOverview with
        {
            Uid = "6798",
            TrustReferenceNumber = "TR0034",
            Ukprn = "100999999",
            CompaniesHouseNumber = "999999",
            Address = "99 The Road, Townville, TA9 9CB",
            RegionAndTerritory = "Cumbria",
            OpenedDate = new DateTime(2015, 4, 20)
        };

        _mockTrustOverviewProvider.GetTrustOverviewAsync("TR0034").Returns(trustOverview);

        var result = await _sut.GetTrustOverviewAsync("TR0034", "6798");

        result.Should()
            .BeEquivalentTo(trustOverview, options => options.ExcludingMissingMembers().Excluding(t => t.Type));
    }

    [Theory]
    [InlineData("Single-academy trust", TrustType.SingleAcademyTrust)]
    [InlineData("Multi-academy trust", TrustType.MultiAcademyTrust)]
    public async Task GetTrustOverviewAsync_should_set_trustType(string givenType, TrustType expectedTrustType)
    {
        _mockTrustOverviewProvider.GetTrustOverviewAsync("TR0012")
            .Returns(BaseTrustOverview with { Type = givenType });

        var result = await _sut.GetTrustOverviewAsync("TR0012", "2806");

        result.Type.Should().Be(expectedTrustType);
    }


    [Theory]
    [InlineData("")]
    [InlineData("Not a SAT or MAT")]
    public async Task GetTrustOverviewAsync_should_throw_when_trustType_invalid(string givenType)
    {
        _mockTrustOverviewProvider.GetTrustOverviewAsync("TR0012")
            .Returns(BaseTrustOverview with { Type = givenType });

        var action = async () => await _sut.GetTrustOverviewAsync("TR0012", "2806");

        await action.Should().ThrowAsync<InvalidOperationException>().WithMessage($"Unknown trust type: {givenType}");
    }

    [Fact]
    public async Task GetTrustOverviewAsync_returns_correct_overview_for_trust_with_academies()
    {
        // Arrange
        var uid = "1234";
        var trustReferenceNumber = "5678";
        var academiesOverview = new AcademyOverview[]
        {
            new("1001", "LocalAuthorityA", null, 600),
            new("1002", "LocalAuthorityB", null, 500),
            new("1003", "LocalAuthorityA", null, 400)
        };

        _mockAcademyRepository.GetOverviewOfAcademiesInTrustAsync(trustReferenceNumber).Returns(Task.FromResult(academiesOverview));
        _mockTrustOverviewProvider.GetTrustOverviewAsync(trustReferenceNumber).Returns(Task.FromResult(BaseTrustOverview with { Uid = uid, TrustReferenceNumber = trustReferenceNumber }));
        _mockTrustPupilService.GetTotalPupilCountForTrustAsync(trustReferenceNumber).Returns(1200);

        // Act
        var result = await _sut.GetTrustOverviewAsync(trustReferenceNumber, uid);

        // Assert
        result.Uid.Should().Be(uid);
        result.TotalAcademies.Should().Be(3);
        result.AcademiesByLocalAuthority.Should().BeEquivalentTo(new Dictionary<string, int>
        {
            { "LocalAuthorityA", 2 },
            { "LocalAuthorityB", 1 }
        });
        result.TotalPupilNumbers.Should().Be(1200);
        result.TotalCapacity.Should().Be(600 + 500 + 400);
    }

    [Fact]
    public async Task GetTrustOverviewAsync_returns_zero_values_for_trust_with_no_academies()
    {
        // Arrange
        var uid = "1234";
        var trustReferenceNumber = "5678";
        var academiesOverview = Array.Empty<AcademyOverview>();

        _mockAcademyRepository.GetOverviewOfAcademiesInTrustAsync(uid).Returns(academiesOverview);
        _mockTrustOverviewProvider.GetTrustOverviewAsync(trustReferenceNumber).Returns(BaseTrustOverview with { Uid = uid, TrustReferenceNumber = trustReferenceNumber });
        _mockTrustPupilService.GetTotalPupilCountForTrustAsync(uid).Returns(0);

        // Act
        var result = await _sut.GetTrustOverviewAsync(trustReferenceNumber, uid);

        // Assert
        result.Uid.Should().Be(uid);
        result.TotalAcademies.Should().Be(0);
        result.AcademiesByLocalAuthority.Should().BeEmpty();
        result.TotalPupilNumbers.Should().Be(0);
        result.TotalCapacity.Should().Be(0);
    }

    [Fact]
    public async Task GetTrustOverviewAsync_sets_HasIncompleteCapacityData_true_when_any_academy_missing_capacity()
    {
        // Arrange
        var uid = "1234";
        var trustReferenceNumber = "5678";
        var academiesOverview = new AcademyOverview[]
        {
            new("1001", "LocalAuthorityA", null, 600),
            new("1002", "LocalAuthorityB", null, null), // Missing capacity
            new("1003", "LocalAuthorityA", null, 400)
        };

        _mockAcademyRepository.GetOverviewOfAcademiesInTrustAsync(trustReferenceNumber).Returns(Task.FromResult(academiesOverview));
        _mockTrustOverviewProvider.GetTrustOverviewAsync(trustReferenceNumber).Returns(Task.FromResult(BaseTrustOverview with { Uid = uid, TrustReferenceNumber = trustReferenceNumber }));
        _mockTrustPupilService.GetTotalPupilCountForTrustAsync(uid).Returns(1000);

        // Act
        var result = await _sut.GetTrustOverviewAsync(trustReferenceNumber, uid);

        // Assert
        result.HasIncompleteCapacityData.Should().BeTrue();
    }

    [Fact]
    public async Task GetTrustSummaryAsync_should_return_null_if_trust_reference_is_null()
    {
        var urn = 123;
        var referenceNumber = "1234";

        _mockTrustOverviewProvider.GetTrustByReferenceNumber(referenceNumber).ReturnsNull();

        var result = await _sut.GetTrustSummaryAsync(urn);

        result.Should().BeNull();
        await _mockTrustOverviewProvider.Received(0).GetTrustByReferenceNumber(Arg.Any<string>());
    }

    [Theory]
    [InlineData(123, "2806", "TR2806", "My Trust", "Multi-academy trust", 3)]
    public async Task GetTrustSummaryAsync_should_return_trust_summary_if_found(int urn, string uid,
        string referenceNumber, string name, string type, int numAcademies)
    {
        _mockTrustOverviewProvider.GetEstablishmentTrust(urn).Returns(new TrustDto
        {
            GroupUid = uid,
            Name = name,
            ReferenceNumber = referenceNumber,
            Type = new NameAndCodeDto { Name = type }
        });

        _mockTrustOverviewProvider.GetTrustByReferenceNumber(referenceNumber).Returns(new TrustDto
        {
            GroupUid = uid,
            Name = name,
            ReferenceNumber = referenceNumber,
            Type = new NameAndCodeDto { Name = type }
        });
        SetupEstablishmentsInTrust(referenceNumber, numAcademies);
    
        var result = await _sut.GetTrustSummaryAsync(referenceNumber);
        result.Should().BeEquivalentTo(new TrustSummaryServiceModel(uid, referenceNumber, name, type, numAcademies));
    }

    private void CreateGovernor(string trn, params Governor[] governors)
    {
        _mockTrustGovernanceProvider.GetMultiAcademyTrustGovernorsAsync(trn)
            .Returns(governors.ToList());
    }

    private void SetupSatGovernors(int urn, params Governor[] governors)
    {
        _mockTrustGovernanceProvider.GetSingleAcademyTrustGovernorsAsync(urn)
            .Returns(governors.ToList());
    }

    private void SetupEstablishmentsInTrust(string trn, int numberOfEstablishments)
    {
        SetupEstablishmentsInTrust(trn,
            Enumerable.Range(1, numberOfEstablishments).Select(n => n.ToString()).ToArray());
    }

    private void SetupEstablishmentsInTrust(string trn, params string?[] urns)
    {
        _mockGetEstablishments.GetEstablishmentsByTrustReferenceNumber(trn)
            .Returns(urns.OfType<string>().Select(urn => new EstablishmentDto { Urn = urn }).ToArray());
    }
}
