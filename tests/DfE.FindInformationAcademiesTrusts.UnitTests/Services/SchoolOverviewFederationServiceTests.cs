using DfE.FindInformationAcademiesTrusts.Data.Repositories.School;
using DfE.FindInformationAcademiesTrusts.Services.School;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Services;

public class SchoolOverviewFederationServiceTests
{
    private readonly int _schoolUrn = 123;

    private readonly SchoolOverviewFederationService _sut;
    private readonly ISchoolRepository _mockSchoolRepository = Substitute.For<ISchoolRepository>();

    private readonly FederationDetails _federationDetails = new(
        "Groovy federation",
        "12345");

    public SchoolOverviewFederationServiceTests()
    {
        _sut = new SchoolOverviewFederationService(_mockSchoolRepository);

        _federationDetails.OpenedOnDate = DateOnly.FromDateTime(DateTime.Today);
        _federationDetails.Schools = new Dictionary<string, string>
        {
            { "6789", "Another school" },
            { "44567", "A third school" }
        };
    }

    [Fact]
    public async Task should_set_values_correctly()
    {
        var expectedResult = new SchoolOverviewFederationServiceModel(
            "Groovy federation",
            "12345",
            DateOnly.FromDateTime(DateTime.Today),
            new Dictionary<string, string>
            {
                { "6789", "Another school" },
                { "44567", "A third school" }
            });

        _mockSchoolRepository.GetSchoolFederationDetailsAsync(_schoolUrn).Returns(_federationDetails);

        var result = await _sut.GetSchoolOverviewFederationAsync(_schoolUrn);

        result.Should().BeEquivalentTo(expectedResult);
    }
}
