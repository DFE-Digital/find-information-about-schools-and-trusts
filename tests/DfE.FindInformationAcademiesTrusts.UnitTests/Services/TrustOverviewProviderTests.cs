using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.AcademiesDbServices;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Trust;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using AddressDto = GovUK.Dfe.CoreLibs.Contracts.Academies.V4.AddressDto;
using NameAndCodeDto = GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Establishments.NameAndCodeDto;
using TrustDto = GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Trusts.TrustDto;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Services;

public class TrustOverviewProviderTests
{
    private readonly TrustOverviewProvider _sut;
    private readonly IGetTrusts _mockGetTrusts = Substitute.For<IGetTrusts>();
    private readonly IStringFormattingUtilities _stringFormattingUtilities = new StringFormattingUtilities();

    public TrustOverviewProviderTests()
    {
        _sut = new TrustOverviewProvider(_mockGetTrusts, _stringFormattingUtilities);
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
}
