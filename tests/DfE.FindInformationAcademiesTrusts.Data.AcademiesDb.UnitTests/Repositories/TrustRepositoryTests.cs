using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Models.Gias;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Models.Tad;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Repositories;
using DfE.FindInformationAcademiesTrusts.Data.Repositories;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Trust;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.UnitTests.Repositories;

public class TrustRepositoryTests
{
    private readonly TrustRepository _sut;
    private readonly MockAcademiesDbContext _mockAcademiesDbContext = new();

    private readonly IStringFormattingUtilities stringFormattingUtilities = new StringFormattingUtilities();

    private readonly DateTime _lastYear = DateTime.Today.AddYears(-1);
    private readonly DateTime _nextYear = DateTime.Today.AddYears(1);
    private readonly DateTime _lastDecade = DateTime.Today.AddYears(-10);
    private readonly DateTime _yesterday = DateTime.Today.AddDays(-1);
    private readonly DateTime _today = DateTime.Today;
    private readonly DateTime _tomorrow = DateTime.Today.AddDays(1);

    public TrustRepositoryTests()
    {
        _sut = new TrustRepository(_mockAcademiesDbContext.Object, stringFormattingUtilities);
    }

    [Theory]
    [InlineData("2806", "My Trust", "Multi-academy trust")]
    [InlineData("9008", "Another Trust", "Single-academy trust")]
    [InlineData("9008", "Trust with no academies", "Multi-academy trust")]
    public async Task GetTrustSummaryAsync_should_return_trustSummary_if_found(string uid, string name, string type)
    {
        _ = _mockAcademiesDbContext.AddGiasGroupForTrust(uid, name, groupType: type);

        var result = await _sut.GetTrustSummaryAsync(uid);
        result.Should().BeEquivalentTo(new TrustSummary(name, type));
    }

    [Fact]
    public async Task GetTrustSummaryAsync_should_return_null_if_not_sat_or_mat()
    {
        _ = _mockAcademiesDbContext.AddGiasGroupForFederation("2806");

        var result = await _sut.GetTrustSummaryAsync("2806");
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetTrustOverviewAsync_should_get_regionAndTerritory_from_mstrTrusts()
    {
        _ = _mockAcademiesDbContext.AddGiasGroupForTrust("2806");
        _ = _mockAcademiesDbContext.AddMstrTrust("2806", "My Region");

        var result = await _sut.GetTrustOverviewAsync("2806");

        result.RegionAndTerritory.Should().Be("My Region");
    }

    [Fact]
    public async Task GetTrustOverviewAsync_should_set_regionAndTerritory_to_empty_string_when_mstrTrust_not_available()
    {
        _ = _mockAcademiesDbContext.AddGiasGroupForTrust("2806");

        var result = await _sut.GetTrustOverviewAsync("2806");

        result.RegionAndTerritory.Should().BeEmpty();
    }

    [Fact]
    public async Task
        GetTrustOverviewAsync_should_set_regionAndTerritory_to_empty_string_when_GORregion_in_mstrTrust_null()
    {
        _ = _mockAcademiesDbContext.AddGiasGroupForTrust("2806");
        _ = _mockAcademiesDbContext.AddMstrTrust("2806", null);

        var result = await _sut.GetTrustOverviewAsync("2806");

        result.RegionAndTerritory.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTrustOverviewAsync_should_build_address_from_giasGroup()
    {
        const string street = "a street";
        const string locality = "a locality";
        const string town = "a town";
        const string postcode = "a postcode";
        const string expectedAddress = $"{street}, {locality}, {town}, {postcode}";

        _mockAcademiesDbContext.GiasGroups.Add(new GiasGroup
        {
            GroupUid = "2806",
            GroupId = "TR0012",
            GroupType = "Multi-academy trust",
            GroupContactStreet = street,
            GroupContactLocality = locality,
            GroupContactTown = town,
            GroupContactPostcode = postcode,
            GroupStatusCode = "OPEN",
            GroupName = "SOME TRUST"
        });

        var result = await _sut.GetTrustOverviewAsync("2806");

        result.Address.Should().Be(expectedAddress);
    }

    [Fact]
    public async Task GetTrustOverviewAsync_should_set_properties_from_giasGroup()
    {
        _mockAcademiesDbContext.GiasGroups.Add(new GiasGroup
        {
            GroupUid = "2806",
            GroupId = "TR0012",
            Ukprn = "10012345",
            GroupType = "Multi-academy trust",
            CompaniesHouseNumber = "123456",
            IncorporatedOnOpenDate = "28/06/2007",
            GroupStatusCode = "OPEN",
            GroupName = "SOME TRUST"
        });

        var result = await _sut.GetTrustOverviewAsync("2806");

        result.Should().BeEquivalentTo(new TrustOverview("2806",
            "TR0012",
            "10012345",
            "123456",
            "Multi-academy trust",
            "",
            "",
            new DateTime(2007, 6, 28, 0, 0, 0, DateTimeKind.Utc)
        ));
    }

    [Fact]
    public async Task GetTrustGovernanceAsync_ShouldReturnEmpty_WithNoGovernanceSet()
    {
        var result = await _sut.GetTrustGovernanceAsync("1234");

        result.Should().BeEquivalentTo(new TrustGovernance([], [], [], []));
    }

    [Theory]
    [InlineData("Member")]
    [InlineData("Trustee")]
    [InlineData("Accounting Officer")]
    [InlineData("Chief Financial Officer")]
    [InlineData("Chair of Trustees")]
    public async Task GetTrustGovernanceAsync_ShouldReturnValidGovernors_WithTheCorrectRole(string role)
    {
        var output = CreateGovernor("1234", "9999", _lastYear, _nextYear, role, email: null);

        var result = await _sut.GetTrustGovernanceAsync("1234");

        var members = new List<Governor>();
        var trustees = new List<Governor>();
        var trustLeadership = new List<Governor>();

        switch (role)
        {
            case "Member":
                members.Add(output);
                break;
            case "Trustee":
                trustees.Add(output);
                break;
            case "Accounting Officer":
            case "Chief Financial Officer":
            case "Chair of Trustees":
                trustLeadership.Add(output);
                break;
        }

        result.Should().BeEquivalentTo(
            new TrustGovernance(trustLeadership.ToArray(), members.ToArray(), trustees.ToArray(), [])
        );
    }

    [Theory]
    [InlineData("Shared Chair of Local Governing Body - Group")]
    [InlineData("Shared Local Governor - Group")]
    [InlineData("Governor")]
    [InlineData("Local Govenor")]
    public async Task GetTrustGovernanceAsync_ShouldReturnNoGovernors_WithTheIncorrectRole(string role)
    {
        _ = CreateGovernor("1234", "9999", _lastYear, _nextYear, role);
        var result = await _sut.GetTrustGovernanceAsync("1234");

        result.Should().BeEquivalentTo(
            new TrustGovernance([], [], [], [])
        );
    }

    [Theory]
    [InlineData("Member")]
    [InlineData("Trustee")]
    [InlineData("Accounting Officer")]
    [InlineData("Chief Financial Officer")]
    [InlineData("Chair of Trustees")]
    [InlineData("Shared Chair of Local Governing Body - Group")]
    [InlineData("Shared Local Governor - Group")]
    [InlineData("Governor")]
    [InlineData("Local Govenor")]
    public async Task GetTrustGovernanceAsync_ShouldReturnHistoricGovernors_WhenTheyExist_IrrespectiveOfRole(
        string role)
    {
        var output = CreateGovernor("1234", "9999", _lastDecade, _lastYear, role, email: null);

        var result = await _sut.GetTrustGovernanceAsync("1234");

        result.Should().BeEquivalentTo(
            new TrustGovernance([], [], [], [output])
        );
    }

    [Fact]
    public async Task GetTrustGovernanceAsync_ShouldSortHistoricAndCurrentGovernors()
    {
        var currentMember1 = CreateGovernor("1234", "9999", _lastDecade, _today, email: null);
        var currentMember2 = CreateGovernor("1234", "9998", _lastDecade, _tomorrow, email: null);
        var historicMember = CreateGovernor("1234", "9997", _lastDecade, _yesterday, email: null);

        var result = await _sut.GetTrustGovernanceAsync("1234");

        result.Should().BeEquivalentTo(
            new TrustGovernance([], [currentMember1, currentMember2], [], [historicMember])
        );
    }

    [Fact]
    public async Task GetTrustContactsAsync_Should_Return_Valid_ChairOfTrustees_WhenOneIsPresentForTheTrust()
    {
        var governor = CreateGovernor("1234", "9876", null, _tomorrow, "Chair of Trustees");
        var result = await _sut.GetTrustContactsAsync("1234");
        result.ChairOfTrustees.Should().NotBeNull();
        result.ChairOfTrustees!.FullName.Should().Be(governor.FullName);
        result.ChairOfTrustees!.Email.Should().Be(governor.Email);
    }

    [Fact]
    public async Task GetTrustContactsAsync_Should_Return_Valid_ChiefFinancialOfficer_WhenOneIsPresentForTheTrust()
    {
        var governor = CreateGovernor("1234", "9876", null, _tomorrow, "Chief Financial Officer");
        var result = await _sut.GetTrustContactsAsync("1234");
        result.ChiefFinancialOfficer.Should().NotBeNull();
        result.ChiefFinancialOfficer!.FullName.Should().Be(governor.FullName);
        result.ChiefFinancialOfficer!.Email.Should().Be(governor.Email);
    }

    [Fact]
    public async Task GetTrustContactsAsync_Should_Return_Valid_AccountingOfficer_WhenOneIsPresentForTheTrust()
    {
        var governor = CreateGovernor("1234", "9876", null, _tomorrow, "Accounting Officer");
        var result = await _sut.GetTrustContactsAsync("1234");
        result.AccountingOfficer.Should().NotBeNull();
        result.AccountingOfficer!.FullName.Should().Be(governor.FullName);
        result.AccountingOfficer!.Email.Should().Be(governor.Email);
    }

    [Fact]
    public async Task GetTrustContactsAsync_Should_Return_null_WhenNoDataIsPresent()
    {
        var result = await _sut.GetTrustContactsAsync("9876");
        result.ChairOfTrustees.Should().BeNull();
        result.ChiefFinancialOfficer.Should().BeNull();
        result.AccountingOfficer.Should().BeNull();
    }

    [Fact]
    public async Task GetTrustContactsAsync_Should_Return_null_WhenOtherGovernorsArePresent()
    {
        _ = CreateGovernor("1234", "9876", null, _tomorrow); //Incorrect role
        _ = CreateGovernor("9876", "9876", null, _tomorrow, "Chief Financial Officer"); //Incorrect uid
        var result = await _sut.GetTrustContactsAsync("1234");
        result.ChairOfTrustees.Should().BeNull();
        result.ChiefFinancialOfficer.Should().BeNull();
        result.AccountingOfficer.Should().BeNull();
    }

    [Fact]
    public async Task GetTrustContactsAsync_Should_Return_CorrectDetails_EvenWithoutMatch_in_TadTrustGovernance_table()
    {
        var input = new GiasGovernance
        {
            Gid = "9999",
            Uid = "1234",
            Role = "Chair of Trustees",
            Forename1 = "First",
            Forename2 = "Second",
            Surname = "Last",
            DateOfAppointment = _lastYear.ToString("dd/MM/yyyy"),
            DateTermOfOfficeEndsEnded = _nextYear.ToString("dd/MM/yyyy"),
            AppointingBody = "Nick Warms"
        };

        _mockAcademiesDbContext.GiasGovernances.Add(input);
        var result = await _sut.GetTrustContactsAsync("1234");
        result.ChairOfTrustees.Should().NotBeNull();
        result.ChairOfTrustees!.FullName.Should().Be("First Second Last");
        result.ChairOfTrustees!.Email.Should().BeNull();
    }

    [Fact]
    public async Task GetTrustContactsAsync_ShouldOnlyReturnCurrentGovernors()
    {
        _ = CreateGovernor("1234", "9999", _lastYear, _yesterday, "Chair of Trustees");
        var today = CreateGovernor("1234", "9998", _lastYear, _today, "Chief Financial Officer");
        var tomorrow = CreateGovernor("1234", "9997", _lastYear, _tomorrow, "Accounting Officer");

        var result = await _sut.GetTrustContactsAsync("1234");
        result.ChairOfTrustees.Should().BeNull();
        result.ChiefFinancialOfficer.Should().NotBeNull();
        result.ChiefFinancialOfficer!.FullName.Should().Be(today.FullName);
        result.ChiefFinancialOfficer!.Email.Should().Be(today.Email);
        result.AccountingOfficer.Should().NotBeNull();
        result.AccountingOfficer!.FullName.Should().Be(tomorrow.FullName);
        result.AccountingOfficer!.Email.Should().Be(tomorrow.Email);
    }

    private Governor CreateGovernor(string uid, string gid, DateTime? startDate,
        DateTime? endDate, string role = "Member", string forename1 = "First", string forename2 = "Second",
        string surname = "Last", string appointingBody = "Nick Warms", string? email = "test@email.com")
    {
        var fullName = forename1; //Forename1 is always populated

        if (!string.IsNullOrWhiteSpace(forename2))
            fullName += $" {forename2}";

        if (!string.IsNullOrWhiteSpace(surname))
            fullName += $" {surname}";

        var giasGovernance = new GiasGovernance
        {
            Gid = gid,
            Uid = uid,
            Role = role,
            Forename1 = forename1,
            Forename2 = forename2,
            Surname = surname,
            DateOfAppointment = startDate?.ToString("dd/MM/yyyy"),
            DateTermOfOfficeEndsEnded = endDate?.ToString("dd/MM/yyyy"),
            AppointingBody = appointingBody
        };

        var governor = new Governor(
            gid,
            uid,
            Role: role,
            FullName: fullName,
            DateOfAppointment: startDate,
            DateOfTermEnd: endDate,
            AppointingBody: "Nick Warms",
            Email: email
        );

        var tadTrustGovernance = new TadTrustGovernance
        {
            Gid = gid,
            Email = email
        };

        _mockAcademiesDbContext.GiasGovernances.Add(giasGovernance);
        _mockAcademiesDbContext.TadTrustGovernances.Add(tadTrustGovernance);

        return governor;
    }

    [Fact]
    public void FilterBySatOrMat_WithUrn_FiltersByUrn()
    {
        // Arrange
        var uid = "some-uid";
        var urn = "some-urn";
        var data = new List<GiasGovernance>
        {
            new() { Urn = "some-urn", Uid = "uid-1" },
            new() { Urn = "another-urn", Uid = "uid-2" }
        }.AsQueryable();

        // Act
        var result = TrustRepository.FilterBySatOrMat(uid, urn, data);

        // Assert
        Assert.All(result, g => Assert.Equal("some-urn", g.Urn));
    }

    [Fact]
    public void FilterBySatOrMat_WithNullOrEmptyUrn_FiltersByUid()
    {
        // Arrange
        var uid = "some-uid";
        string? urn = null;
        var data = new List<GiasGovernance>
        {
            new() { Urn = "urn-1", Uid = "some-uid" },
            new() { Urn = "urn-2", Uid = "another-uid" }
        }.AsQueryable();

        // Act
        var result = TrustRepository.FilterBySatOrMat(uid, urn, data);

        // Assert
        Assert.All(result, g => Assert.Equal("some-uid", g.Uid));
    }

    [Fact]
    public async Task GetTrustReferenceNumberAsync_should_return_trustReferenceNumber_for_uid()
    {
        _ = _mockAcademiesDbContext.AddGiasGroupForTrust("2806", trustReferenceNumber: "My trust reference number");

        var result = await _sut.GetTrustReferenceNumberAsync("2806");
        result.Should().BeEquivalentTo("My trust reference number");
    }

    [Fact]
    public async Task GetTrustContactsAsync_ShouldOnlyReturnCurrentChairOfTrusteesWhenOneStartsInFuture()
    {
        var startDateOfNewChair = DateTime.Now.AddDays(2);
        var endDateOfCurrent = DateTime.Now.AddDays(1);

        var currentName = "James";
        var newName = "Pete";

        var newChairId = "5678";

        _ = CreateGovernor("1234", newChairId, startDateOfNewChair, null, "Chair of Trustees", newName);
        _ = CreateGovernor("1234", "9999", null, endDateOfCurrent, "Chair of Trustees", currentName);

        var result = await _sut.GetTrustContactsAsync("1234");
        result.ChairOfTrustees.Should().NotBeNull();
        result.ChairOfTrustees!.FullName.Should().Be($"{currentName} Second Last");
    }
}
