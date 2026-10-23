using System.Collections.ObjectModel;
using DfE.FindInformationAcademiesTrusts.Data.Repositories;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Establishments;
using GovUK.Dfe.PersonsApi.Client.Contracts;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Services;

public class TrustGovernanceProviderTests
{
    private readonly TrustGovernanceProvider _sut;
    private readonly ITrustsClient _trustsClient = Substitute.For<ITrustsClient>();
    private readonly IEstablishmentsClient _establishmentsClient = Substitute.For<IEstablishmentsClient>();

    public TrustGovernanceProviderTests()
    {
        _sut = new TrustGovernanceProvider(_trustsClient, _establishmentsClient);
    }

    [Fact]
    public async Task GetMultiAcademyTrustGovernorsAsync_should_return_no_governors_when_none_exist()
    {
        const string trn = "TR1234";
        _trustsClient.GetAllPersonsAssociatedWithTrustByTrnOrUkPrnAsync(trn)
            .Returns(new ObservableCollection<TrustGovernance>());

        var result = await _sut.GetMultiAcademyTrustGovernorsAsync(trn);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetMultiAcademyTrustGovernorsAsync_should_map_governor_details()
    {
        const string trn = "TR1234";
        var appointmentDate = new DateTime(2023, 1, 15);
        var termEndDate = new DateTime(2027, 1, 15);

        _trustsClient.GetAllPersonsAssociatedWithTrustByTrnOrUkPrnAsync(trn)
            .Returns(new ObservableCollection<TrustGovernance>
            {
                new()
                {
                    Trn = trn,
                    DisplayName = "Hilly Jilly Trainface",
                    Roles = ["Chair of Trustees"],
                    AppointingBody = "Members",
                    DateOfAppointment = appointmentDate.ToString("dd/MM/yyyy"),
                    DateTermOfOfficeEndsEnded = termEndDate.ToString("dd/MM/yyyy"),
                    Email = "hilly@example.com"
                },
                new()
                {
                    Trn = trn,
                    DisplayName = "Billy Boatface",
                    Roles = ["Trustee"],
                    AppointingBody = "Governing Body",
                    DateOfAppointment = null,
                    DateTermOfOfficeEndsEnded = null,
                    Email = null
                }
            });

        var result = await _sut.GetMultiAcademyTrustGovernorsAsync(trn);

        result.Should().BeEquivalentTo(
        [
            new Governor("Hilly Jilly Trainface", "Chair of Trustees", "Members", appointmentDate, termEndDate,
                "hilly@example.com"),
            new Governor("Billy Boatface", "Trustee", "Governing Body", null, null, null)
        ]);
    }

    [Fact]
    public async Task GetSingleAcademyTrustGovernorsAsync_should_return_no_governors_when_none_exist()
    {
        const int urn = 1234;
        _establishmentsClient.GetAllPersonsAssociatedWithAcademyByUrnAsync(urn)
            .Returns(new ObservableCollection<AcademyGovernance>());

        var result = await _sut.GetSingleAcademyTrustGovernorsAsync(urn);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetSingleAcademyTrustGovernorsAsync_should_map_governor_details()
    {
        const int urn = 1234;
        var appointmentDate = new DateTime(2023, 1, 15);
        var termEndDate = new DateTime(2027, 1, 15);

        _establishmentsClient.GetAllPersonsAssociatedWithAcademyByUrnAsync(urn)
            .Returns(new ObservableCollection<AcademyGovernance>
            {
                new()
                {
                    Urn = urn,
                    DisplayName = "Hilly Jilly Trainface",
                    Roles = ["Chair of Trustees"],
                    AppointingBody = "Members",
                    DateOfAppointment = appointmentDate.ToString("dd/MM/yyyy"),
                    DateTermOfOfficeEndsEnded = termEndDate.ToString("dd/MM/yyyy"),
                    Email = "hilly@example.com"
                },
                new()
                {
                    Urn = urn,
                    DisplayName = "Billy Boatface",
                    Roles = ["Trustee"],
                    AppointingBody = "Governing Body",
                    DateOfAppointment = null,
                    DateTermOfOfficeEndsEnded = null,
                    Email = null
                }
            });

        var result = await _sut.GetSingleAcademyTrustGovernorsAsync(urn);

        result.Should().BeEquivalentTo(
        [
            new Governor("Hilly Jilly Trainface", "Chair of Trustees", "Members", appointmentDate, termEndDate,
                "hilly@example.com"),
            new Governor("Billy Boatface", "Trustee", "Governing Body", null, null, null)
        ]);
    }
}
