using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb;
using DfE.FindInformationAcademiesTrusts.HttpServices;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Trust;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Establishments;
using AddressDto = GovUK.Dfe.CoreLibs.Contracts.Academies.V4.AddressDto;
using TrustDto = GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Trusts.TrustDto;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Services;

public class TrustOverviewProviderTests
{
    private readonly TrustOverviewProvider _sut;
    private readonly IGetTrustsTemp _mockGetTrusts = Substitute.For<IGetTrustsTemp>();
    private readonly IGetEstablishmentsTemp _mockGetEstablishments = Substitute.For<IGetEstablishmentsTemp>();
    private readonly IStringFormattingUtilities _stringFormattingUtilities = new StringFormattingUtilities();

    public TrustOverviewProviderTests()
    {
        _sut = new TrustOverviewProvider(_mockGetTrusts, _mockGetEstablishments, _stringFormattingUtilities);
    }

    [Fact]
    public async Task GetTrustOverviewAsync_should_throw_when_trust_is_not_found()
    {
        _mockGetTrusts.GetTrustByReferenceNumber("TR0012").Returns((TrustDto?)null);

        var action = async () => await _sut.GetTrustOverviewAsync("TR0012");

        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Trust with reference number TR0012 was not found.");
    }

    [Theory]
    [InlineData("Multi-academy trust")]
    [InlineData("Single-academy trust")]
    public async Task GetTrustOverviewAsync_should_map_trust_details(string trustType)
    {
        const string trustReferenceNumber = "TR0012";

        _mockGetTrusts.GetTrustByReferenceNumber(trustReferenceNumber)
            .Returns(new TrustDto
            {
                GroupUid = "2806",
                Ukprn = "10012345",
                CompaniesHouseNumber = "01234567",
                Type = new NameAndCodeDto { Name = trustType },
                Address = new AddressDto
                {
                    Street = "12 Academy Way",
                    Locality = "District",
                    Town = "Townville",
                    Postcode = "AB1 2CD"
                },
                Gor = "North West",
                OpenDate = "20/04/2015"
            });

        var result = await _sut.GetTrustOverviewAsync(trustReferenceNumber);

        result.Should().BeEquivalentTo(new TrustOverview(
            "2806",
            trustReferenceNumber,
            "10012345",
            "01234567",
            trustType,
            "12 Academy Way, District, Townville, AB1 2CD",
            "North West",
            new DateTime(2015, 4, 20)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    public async Task GetEstablishmentsInTrustCount_should_return_number_of_establishments(int numberOfEstablishments)
    {
        _mockGetEstablishments.GetEstablishmentsByTrustReferenceNumber("TR0012")
            .Returns(Enumerable.Range(1, numberOfEstablishments)
                .Select(n => new EstablishmentDto { Urn = n.ToString() })
                .ToArray());

        var result = await _sut.GetEstablishmentsInTrustCount("TR0012");

        result.Should().Be(numberOfEstablishments);
    }

    [Fact]
    public async Task GetSingleAcademyTrustUrn_should_return_urn_when_trust_has_one_establishment()
    {
        _mockGetEstablishments.GetEstablishmentsByTrustReferenceNumber("TR0012")
            .Returns([new EstablishmentDto { Urn = "123456" }]);

        var result = await _sut.GetSingleAcademyTrustUrn("TR0012");

        result.Should().Be("123456");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    public async Task GetSingleAcademyTrustUrn_should_return_null_when_trust_does_not_have_exactly_one_establishment(
        int numberOfEstablishments)
    {
        _mockGetEstablishments.GetEstablishmentsByTrustReferenceNumber("TR0012")
            .Returns(Enumerable.Range(1, numberOfEstablishments)
                .Select(n => new EstablishmentDto { Urn = n.ToString() })
                .ToArray());

        var result = await _sut.GetSingleAcademyTrustUrn("TR0012");

        result.Should().BeNull();
    }
}
