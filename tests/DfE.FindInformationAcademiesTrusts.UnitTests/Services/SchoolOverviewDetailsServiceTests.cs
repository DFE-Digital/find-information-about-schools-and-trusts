using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.School;
using DfE.FindInformationAcademiesTrusts.Services.School;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Services;

public class SchoolOverviewDetailsServiceTests
{
    private readonly int _laMaintainedSchoolUrn = 123;
    private readonly int _academySchoolUrn = 678;

    private readonly SchoolOverviewDetailsService _sut;
    private readonly ISchoolRepository _mockSchoolRepository = Substitute.For<ISchoolRepository>();

    private readonly SchoolDetails _laMaintainedSchoolDetails = new("Cool school",
        "some address", "yorkshire", "leeds", "secondary", new AgeRange(2, 6), "no nursery classes", null, null);

    private readonly SchoolDetails _academySchoolDetails = new("Cool academy",
        "some address", "yorkshire", "leeds", "secondary", new AgeRange(2, 6), "no nursery classes", "some trust",
        new DateTime(2011, 04, 03));


    public SchoolOverviewDetailsServiceTests()
    {
        _sut = new SchoolOverviewDetailsService(_mockSchoolRepository);
    }

    [Fact]
    public async Task If_school_is_la_maintained_should_not_get_date_joined_trust()
    {
        var expectedResult = new SchoolOverviewServiceModel("Cool school",
            "some address", "yorkshire", "leeds", "secondary", new AgeRange(2, 6), NurseryProvision.NoClasses, null,
            null);

        _mockSchoolRepository.GetSchoolDetailsAsync(_laMaintainedSchoolUrn).Returns(_laMaintainedSchoolDetails);

        var result =
            await _sut.GetSchoolOverviewDetailsAsync(_laMaintainedSchoolUrn);

        result.Should().NotBeNull();
        result!.DateJoinedTrust.Should().BeNull();
        result.Should().BeEquivalentTo(expectedResult);

        await _mockSchoolRepository.Received(0).GetDateJoinedTrustAsync(_laMaintainedSchoolUrn);
    }

    [Fact]
    public async Task If_school_is_academy_should_return_with_date_joined_trust()
    {
        var expectedResult = new SchoolOverviewServiceModel("Cool academy",
            "some address", "yorkshire", "leeds", "secondary", new AgeRange(2, 6), NurseryProvision.NoClasses,
            "some trust", new DateTime(2011, 04, 03));

        _mockSchoolRepository.GetSchoolDetailsAsync(_academySchoolUrn).Returns(_academySchoolDetails);

        var result = await _sut.GetSchoolOverviewDetailsAsync(_academySchoolUrn);

        result.Should().NotBeNull();
        result!.DateJoinedTrust.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedResult);
    }

    public static TheoryData<string, NurseryProvision> NurseryProvisionCombinations => new()
    {
        { "", NurseryProvision.NotRecorded },
        { "has nursery classes", NurseryProvision.HasClasses },
        { "haS Nursery classes", NurseryProvision.HasClasses },
        { "no nursery classes", NurseryProvision.NoClasses },
        { "No Nursery Classes", NurseryProvision.NoClasses },
        { "not recorded", NurseryProvision.NotRecorded }
    };

    [Theory]
    [MemberData(nameof(NurseryProvisionCombinations))]
    public void Should_return_correct_nursery_provision(string text, NurseryProvision expectedNurseryProvision)
    {
        var result = SchoolOverviewDetailsService.GetNurseryProvision(text);

        result.Should().Be(expectedNurseryProvision);
    }
}
