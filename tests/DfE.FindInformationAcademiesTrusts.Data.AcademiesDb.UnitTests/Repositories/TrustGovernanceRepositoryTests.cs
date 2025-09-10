using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Models.Gias;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Models.Tad;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Repositories;
using DfE.FindInformationAcademiesTrusts.Data.Repositories;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.UnitTests.Repositories;

public class TrustGovernanceRepositoryTests
{
    private readonly TrustGovernanceRepository _sut;
    private readonly MockAcademiesDbContext _mockAcademiesDbContext = new();
    private readonly IStringFormattingUtilities _stringFormattingUtilities = new StringFormattingUtilities();
    
    private readonly DateTime _lastYear = DateTime.Today.AddYears(-1);
    private readonly DateTime _nextYear = DateTime.Today.AddYears(1);

    public TrustGovernanceRepositoryTests()
    {
        _sut = new TrustGovernanceRepository(_mockAcademiesDbContext.Object, _stringFormattingUtilities);
    }

    [Fact]
    public async Task GetTrustGovernanceAsync_should_return_empty_list_when_no_governors_exist_for_uid_or_urn()
    {
        var result = await _sut.GetTrustGovernanceAsync("1234");

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTrustGovernanceAsync_should_return_only_governors_for_that_uid_or_urn()
    {
        var unexpectedGovernor1 = CreateGovernor("9876", "9999", _lastYear, _nextYear);
        var unexpectedGovernor2 = CreateGovernor("9876", "9999", _lastYear, _nextYear);
        var expectedGovernor = CreateGovernor("1234", "9999", _lastYear, _nextYear);
        
        var result = await _sut.GetTrustGovernanceAsync("1234");
        
        result.Should().NotContain(unexpectedGovernor1);
        result.Should().NotContain(unexpectedGovernor2);
        result.Should().Contain(expectedGovernor);
    }

    private Governor CreateGovernor(
        string uid,
        string gid,
        DateTime? startDate,
        DateTime? endDate,
        string role = "Member",
        string forename1 = "First",
        string forename2 = "Second",
        string surname = "Last",
        string appointingBody = "Some Org")
    {
        var fullName = string.Join(
            ' ',
            new List<string> { forename1, forename2, surname }.Where(n => !string.IsNullOrWhiteSpace(n))
        );

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
            fullName,
            role,
            appointingBody,
            startDate,
            endDate,
            null
        );

        var tadTrustGovernance = new TadTrustGovernance
        {
            Gid = gid
        };

        _mockAcademiesDbContext.GiasGovernances.Add(giasGovernance);
        _mockAcademiesDbContext.TadTrustGovernances.Add(tadTrustGovernance);

        return governor;
    }
}
