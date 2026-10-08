using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.School;
using DfE.FindInformationAcademiesTrusts.HttpServices;
using DfE.FindInformationAcademiesTrusts.Services.School;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Establishments;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Services;

public class SchoolOverviewDetailsServiceTests
{
    private readonly int _academySchoolUrn = 678;

    private readonly SchoolOverviewDetailsService _sut;
    private readonly IGetEstablishmentsTemp _mockGetEstablishments;
    private readonly IStringFormattingUtilities _stringFormattingUtilities = new StringFormattingUtilities();

    public SchoolOverviewDetailsServiceTests()
    {
        _mockGetEstablishments = Substitute.For<IGetEstablishmentsTemp>();

        _sut = new SchoolOverviewDetailsService(_stringFormattingUtilities, _mockGetEstablishments);
    }

    [Fact]
    public async Task If_school_is_academy_should_return_with_date_joined_trust()
    {
        var expectedResult = new SchoolOverviewServiceModel("Cool academy",
            "123 High Street, Pudsey, Leeds, AB1 2AA", "yorkshire", "leeds", "secondary", new AgeRange(2, 6), NurseryProvision.NoClasses,
            "some trust", new DateTime(2011, 04, 03));
        
        _mockGetEstablishments.GetEstablishment(_academySchoolUrn).Returns(new EstablishmentDto
        {
            Urn = _academySchoolUrn.ToString(),
            Name = expectedResult.Name,
            EstablishmentType = new()
            {
                Name = "Academy converter"
            },
            EstablishmentGroupType = new()
            {
                Name = "Academies"
            },
            Address = new AddressDto
            {
                Street = "123 High Street",
                Locality = "Pudsey",
                Town = "Leeds",
                Postcode = "AB1 2AA"
            },
            Gor = new NameAndCodeDto
            {
                Name = expectedResult.Region
            },
            LocalAuthorityName = expectedResult.LocalAuthority,
            PhaseOfEducation = new NameAndCodeDto
            {
                Name = expectedResult.PhaseOfEducationName
            },
            StatutoryHighAge = "6",
            StatutoryLowAge = "2",
            NurseryProvision = "no nursery classes",
            TrustName =  expectedResult.TrustName,
            DateJoinedTrust = "03/04/2011"
        });

        var result = await _sut.GetSchoolOverviewDetailsAsync(_academySchoolUrn);

        result.Should().NotBeNull();
        result.DateJoinedTrust.Should().NotBeNull();
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
