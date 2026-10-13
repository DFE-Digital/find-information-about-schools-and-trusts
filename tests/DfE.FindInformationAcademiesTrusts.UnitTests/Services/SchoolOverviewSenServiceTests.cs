using DfE.FindInformationAcademiesTrusts.HttpServices;
using DfE.FindInformationAcademiesTrusts.Services.School;
using GovUK.Dfe.AcademiesApi.Client.Contracts;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Services;

public class SchoolOverviewSenServiceTests
{
    private readonly int _schoolUrn = 123;

    private readonly SchoolOverviewSenService _sut;
    private readonly IGetEstablishmentsTemp _mockGetEstablishments;

    public SchoolOverviewSenServiceTests()
    {
        _mockGetEstablishments = Substitute.For<IGetEstablishmentsTemp>();
        _sut = new SchoolOverviewSenService(_mockGetEstablishments);
    }

    [Fact]
    public async Task should_set_values_correctly()
    {
        var expectedResult = new SchoolOverviewSenServiceModel(
            "2",
            "3",
            "22",
            "4",
            "Resourced",
            new List<string>
            {
                "Sen1", "Sen2", "Sen3", "Sen4", "Sen5", "Sen6", "Sen7", "Sen8", "Sen9", "Sen10", "Sen11",
                "Sen12", "Sen13"
            });
        
        _mockGetEstablishments.GetEstablishmentWithSenData(_schoolUrn)
            .Returns(new EstablishmentResponse
            {
                Urn = _schoolUrn.ToString(),
                EstablishmentName = "cool school",
                ResourcedProvisionOnRoll = "2",
                ResourcedProvisionOnCapacity = "3",
                SenUnitOnRoll = "22",
                SenUnitCapacity = "4",
                TypeOfResourcedProvision = "Resourced",
                SeN1 = "Sen1",
                SeN2 = "Sen2",
                SeN3 = "Sen3",
                SeN4 = "Sen4",
                SeN5 = "Sen5",
                SeN6 = "Sen6",
                SeN7 = "Sen7",
                SeN8 = "Sen8",
                SeN9 = "Sen9",
                SeN10 = "Sen10",
                SeN11 = "Sen11",
                SeN12 = "Sen12",
                SeN13 = "Sen13"
            });
        
        var result = await _sut.GetSchoolOverviewSenAsync(_schoolUrn);
        
        result.Should().BeEquivalentTo(expectedResult);
        result.SenProvisionTypes.Count.Should().Be(13);
    }
}