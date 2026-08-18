using System.Collections.ObjectModel;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Repositories;
using DfE.FindInformationAcademiesTrusts.Data.Repositories;
using GovUK.Dfe.PersonsApi.Client.Contracts;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.UnitTests.Repositories;

public class TrustGovernanceRepositoryTests
{
    private readonly TrustGovernanceRepository _sut;
    private readonly ITrustsClient _trustsClient = Substitute.For<ITrustsClient>();

    private readonly DateTime _lastYear = DateTime.Today.AddYears(-1);
    private readonly DateTime _nextYear = DateTime.Today.AddYears(1);

    public TrustGovernanceRepositoryTests()
    {
        _sut = new TrustGovernanceRepository(_trustsClient);
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
        var unexpectedGovernor1 = CreateGovernor("9876", _lastYear, _nextYear, "Member", "Billy", "Willy", "Boatface", "Some Org");
        var unexpectedGovernor2 = CreateGovernor("9876", _lastYear, _nextYear, "Trustee", "Milly", "Tilly", "Planeface", "Some other Org");
        var expectedGovernor = CreateGovernor("1234", _lastYear, _nextYear, "Goat", "Hilly", "Jilly", "Trainface", "Some third Org");

        var result = await _sut.GetTrustGovernanceAsync("1234");

        result.Should().NotContain(unexpectedGovernor1);
        result.Should().NotContain(unexpectedGovernor2);
        result.Should().Contain(expectedGovernor);
    }

    private Governor CreateGovernor(
        string trn,
        DateTime? startDate,
        DateTime? endDate,
        string role,
        string forename1,
        string forename2,
        string surname,
        string appointingBody,
        string? email = null)
    {
        var fullName = string.Join(
            ' ',
            new List<string> { forename1, forename2, surname }.Where(n => !string.IsNullOrWhiteSpace(n))
        );

        var trustGovernance = new TrustGovernance
        {
            Trn = trn,
            DisplayName = fullName,
            FirstName = forename1,
            LastName = surname,
            Roles = [role],
            AppointingBody = appointingBody,
            DateOfAppointment = startDate?.ToString("dd/MM/yyyy"),
            DateTermOfOfficeEndsEnded = endDate?.ToString("dd/MM/yyyy"),
            Email = email
        };

        _trustsClient.GetAllPersonsAssociatedWithTrustByTrnOrUkPrnAsync(trn)
            .Returns(new ObservableCollection<TrustGovernance> { trustGovernance });

        return new Governor(
            fullName,
            role,
            appointingBody,
            startDate,
            endDate,
            email
        );
    }
}
