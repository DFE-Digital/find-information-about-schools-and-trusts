using Dfe.AcademiesApi.Client.Contracts;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.AcademiesDbServices;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Models.Gias;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Models.Tad;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Repositories;
using DfE.FindInformationAcademiesTrusts.Data.Repositories;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Trust;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4;
using AddressDto = GovUK.Dfe.CoreLibs.Contracts.Academies.V4.AddressDto;
using NameAndCodeDto = GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Establishments.NameAndCodeDto;
using TrustDto = GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Trusts.TrustDto;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.UnitTests.Repositories;

public class TrustRepositoryTests
{
    private readonly TrustRepository _sut;
    private readonly MockAcademiesDbContext _mockAcademiesDbContext = new();
    private readonly IGetTrusts _mockGetTrusts = Substitute.For<IGetTrusts>();

    private readonly IStringFormattingUtilities stringFormattingUtilities = new StringFormattingUtilities();

    private readonly DateTime _lastYear = DateTime.Today.AddYears(-1);
    private readonly DateTime _nextYear = DateTime.Today.AddYears(1);
    private readonly DateTime _yesterday = DateTime.Today.AddDays(-1);
    private readonly DateTime _today = DateTime.Today;
    private readonly DateTime _tomorrow = DateTime.Today.AddDays(1);

    public TrustRepositoryTests()
    {
        _sut = new TrustRepository(_mockAcademiesDbContext.Object,_mockGetTrusts, stringFormattingUtilities);
    }

    [Theory]
    [InlineData("TR2806", "2806", "My Trust", "Multi-academy trust")]
    [InlineData("TR9008", "9008", "Another Trust", "Single-academy trust")]
    [InlineData("TR9010", "9010", "Trust with no academies", "Multi-academy trust")]
    public async Task GetTrustSummaryAsync_should_return_trustSummary_if_found(string referenceNumber, string uid,
        string name, string type)
    {
        _mockGetTrusts.GetTrustByReferenceNumber(referenceNumber).Returns(new TrustDto
        {
            GroupUid = uid,
            Name = name,
            ReferenceNumber = referenceNumber,
            Type = new NameAndCodeDto { Name = type }
        });

        var result = await _sut.GetTrustSummaryAsync(referenceNumber);
        result.Should().BeEquivalentTo(new TrustSummary(name, type, uid, referenceNumber));
    }
    
    
    [Fact]
    public async Task GetTrustOverviewByTrnAsync_should_return_trust_overview_if_found()
    {
        const string referenceNumber = "TR0000";
        const string uid = "2806";
        const string name = "My Trust";
        const string type = "Multi-academy trust";
        const string ukprn = "10000000";
        const string companiesHouseNumber = "01234567";
        const string gor = "London";

        _mockGetTrusts.GetTrustByReferenceNumber(referenceNumber).Returns(new TrustDto
        {
            GroupUid = uid,
            Name = name,
            ReferenceNumber = referenceNumber,
            Type = new NameAndCodeDto { Name = type },
            Ukprn = ukprn,
            CompaniesHouseNumber = companiesHouseNumber,
            Gor = gor,
            OpenDate = "01/09/2020",
            Address = new AddressDto
            {
                Street = "1 Test Street",
                Additional = "Testville",
                Town = "Testington",
                Postcode = "TE1 1ST"
            }
        });

        var result = await _sut.GetTrustOverviewByTrnAsync(referenceNumber);

        result.Should().BeEquivalentTo(new TrustOverview(
            uid,
            referenceNumber,
            ukprn,
            companiesHouseNumber,
            type,
            stringFormattingUtilities.BuildAddressString("1 Test Street", "Testville", "Testington", "TE1 1ST"),
            gor,
            new DateTime(2020, 9, 1)));
    }
    
    [Fact]
    public async Task GetTrustOverviewByTrnAsync_should_return_null_if_trust_not_found()
    {
        const string referenceNumber = "TR0000";

        _mockGetTrusts.GetTrustByReferenceNumber(referenceNumber).Returns((TrustDto?)null);

        var result = await _sut.GetTrustOverviewByTrnAsync(referenceNumber);

        result.Should().BeNull();
    }
    

    [Fact]
    public async Task GetTrustSummaryAsync_should_return_null_if_reference_number_not_found()
    {
        const string referenceNumber = "TR0000";
        _mockGetTrusts.GetTrustByReferenceNumber(referenceNumber).Returns((TrustDto?)null);

        var result = await _sut.GetTrustSummaryAsync(referenceNumber);
        result.Should().BeNull();
    }
    
    
    [Fact]
    public async Task GetTrustSummaryByEstablishmentByUrnAsync_should_return_trustSummary_if_found()
    {
        const int urn = 123;
        const string name = "My Trust";
        const string uid = "12";
        const string type = "Multi-academy trust";
        const string referenceNumber = "TR2806";
        
        _mockGetTrusts.GetEstablishmentTrust(123).Returns(new TrustDto
        {
            GroupUid = uid,
            Name = name,
            ReferenceNumber = referenceNumber,
            Type = new NameAndCodeDto { Name = type }
        });

        var result = await _sut.GetTrustSummaryByEstablishmentUrnAsync(urn);
        result.Should().BeEquivalentTo(new TrustSummary(name, type, uid, referenceNumber));
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
