using System.Collections.ObjectModel;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Repositories;
using DfE.FindInformationAcademiesTrusts.Data.Repositories;
using GovUK.Dfe.PersonsApi.Client.Contracts;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.UnitTests.Repositories;

public class TrustGovernanceRepositoryTests
{
    private readonly TrustGovernanceRepository _sut;
    private readonly ITrustsClient _trustsClient = Substitute.For<ITrustsClient>();
    private readonly IEstablishmentsClient _establishmentsClient = Substitute.For<IEstablishmentsClient>();

    private readonly DateTime _lastYear = DateTime.Today.AddYears(-1);
    private readonly DateTime _nextYear = DateTime.Today.AddYears(1);

    public TrustGovernanceRepositoryTests()
    {
        _sut = new TrustGovernanceRepository(_trustsClient, _establishmentsClient);
    }

    [Fact]
    public async Task GetTrustGovernanceAsync_should_return_empty_list_when_no_governors_exist_for_trn()
    {
        _trustsClient.GetAllPersonsAssociatedWithTrustByTrnOrUkPrnAsync("1234")
            .Returns(new ObservableCollection<TrustGovernance>());

        var result = await _sut.GetTrustGovernanceAsync("1234");

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTrustGovernanceAsync_should_return_only_governors_for_that_trn()
    {
        var unexpectedGovernor1 = new Governor("Billy Willy Boatface", "Member", "Some Org", _lastYear, _nextYear, null);
        var unexpectedGovernor2 = new Governor("Milly Tilly Planeface", "Trustee", "Some other Org", _lastYear, _nextYear, null);
        var expectedGovernor = new Governor("Hilly Jilly Trainface", "Goat", "Some third Org", _lastYear, _nextYear, null);

        CreateGovernor("9876", unexpectedGovernor1, unexpectedGovernor2);
        CreateGovernor("1234", expectedGovernor);

        var result = await _sut.GetTrustGovernanceAsync("1234");

        result.Should().NotContain(unexpectedGovernor1);
        result.Should().NotContain(unexpectedGovernor2);
        result.Should().Contain(expectedGovernor);
    }

    [Fact]
    public async Task GetSatGovernanceAsync_should_return_empty_list_when_no_governors_exist_for_urn()
    {
        _establishmentsClient.GetAllPersonsAssociatedWithAcademyByUrnAsync(1234)
            .Returns(new ObservableCollection<AcademyGovernance>());

        var result = await _sut.GetSatGovernanceAsync(1234);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetSatGovernanceAsync_should_return_only_governors_for_that_urn()
    {
        var unexpectedGovernor1 = new Governor("Billy Willy Boatface", "Member", "Some Org", _lastYear, _nextYear, null);
        var unexpectedGovernor2 = new Governor("Milly Tilly Planeface", "Trustee", "Some other Org", _lastYear, _nextYear, null);
        var expectedGovernor = new Governor("Hilly Jilly Trainface", "Goat", "Some third Org", _lastYear, _nextYear, null);

        SetupSatGovernors(9876, unexpectedGovernor1, unexpectedGovernor2);
        SetupSatGovernors(1234, expectedGovernor);

        var result = await _sut.GetSatGovernanceAsync(1234);

        result.Should().NotContain(unexpectedGovernor1);
        result.Should().NotContain(unexpectedGovernor2);
        result.Should().Contain(expectedGovernor);
    }

    [Fact]
    public async Task GetSatGovernanceAsync_should_map_governor_details()
    {
        const int urn = 1234;

        _establishmentsClient.GetAllPersonsAssociatedWithAcademyByUrnAsync(urn)
            .Returns(new ObservableCollection<AcademyGovernance>
            {
                new()
                {
                    Urn = urn,
                    DisplayName = "Hilly Jilly Trainface",
                    Roles = ["Chair of Governors"],
                    AppointingBody = "Members",
                    DateOfAppointment = _lastYear.ToString("dd/MM/yyyy"),
                    DateTermOfOfficeEndsEnded = _nextYear.ToString("dd/MM/yyyy"),
                    Email = "hilly@example.com"
                },
                new()
                {
                    Urn = urn,
                    DisplayName = "Billy Boatface",
                    Roles = ["Governor"],
                    AppointingBody = "Governing Body",
                    DateOfAppointment = null,
                    DateTermOfOfficeEndsEnded = null,
                    Email = null
                }
            });

        var result = await _sut.GetSatGovernanceAsync(urn);

        result.Should().BeEquivalentTo(
        [
            new Governor("Hilly Jilly Trainface", "Chair of Governors", "Members", _lastYear, _nextYear, "hilly@example.com"),
            new Governor("Billy Boatface", "Governor", "Governing Body", null, null, null)
        ]);
    }

    private void CreateGovernor(string trn, params Governor[] governors)
    {
        var trustGovernances = governors.Select(governor => new TrustGovernance
        {
            Trn = trn,
            DisplayName = governor.FullName,
            Roles = [governor.Role],
            AppointingBody = governor.AppointingBody,
            DateOfAppointment = governor.DateOfAppointment?.ToString("dd/MM/yyyy"),
            DateTermOfOfficeEndsEnded = governor.DateOfTermEnd?.ToString("dd/MM/yyyy"),
            Email = governor.Email
        });

        _trustsClient.GetAllPersonsAssociatedWithTrustByTrnOrUkPrnAsync(trn)
            .Returns(new ObservableCollection<TrustGovernance>(trustGovernances));
    }

    private void SetupSatGovernors(int urn, params Governor[] governors)
    {
        var academyGovernances = governors.Select(governor => new AcademyGovernance
        {
            Urn = urn,
            DisplayName = governor.FullName,
            Roles = [governor.Role],
            AppointingBody = governor.AppointingBody,
            DateOfAppointment = governor.DateOfAppointment?.ToString("dd/MM/yyyy"),
            DateTermOfOfficeEndsEnded = governor.DateOfTermEnd?.ToString("dd/MM/yyyy"),
            Email = governor.Email
        });

        _establishmentsClient.GetAllPersonsAssociatedWithAcademyByUrnAsync(urn)
            .Returns(new ObservableCollection<AcademyGovernance>(academyGovernances));
    }
}
