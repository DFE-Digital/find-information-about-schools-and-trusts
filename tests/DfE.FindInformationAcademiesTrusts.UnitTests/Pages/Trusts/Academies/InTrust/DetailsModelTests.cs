using DfE.FindInformationAcademiesTrusts.Pages;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Academies.InTrust;
using DfE.FindInformationAcademiesTrusts.Services.Academy;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Trusts.Academies.InTrust;

public class AcademiesDetailsModelTests : AcademiesInTrustAreaModelTests<AcademiesInTrustDetailsModel>
{
    private readonly IOtherServicesLinkBuilder _mockLinkBuilder = Substitute.For<IOtherServicesLinkBuilder>();

    public AcademiesDetailsModelTests()
    {
        Sut = new AcademiesInTrustDetailsModel(MockDataSourceService,
                MockTrustService,
                MockAcademyService,
                MockAcademiesExportService,
                MockDateTimeProvider)
            { Uid = TrustUid };
    }

    [Fact]
    public override async Task OnGetAsync_sets_academies_from_academyService()
    {
        var academies = new[]
        {
           new AcademyDetailsServiceModel("1", "", "", "", "", DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10))),
           new AcademyDetailsServiceModel("2", "", "", "", "", DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10))),
           new AcademyDetailsServiceModel("3", "", "", "", "", DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)))
       };
        MockAcademyService.GetAcademiesInTrustDetailsAsync(Sut.Uid).Returns(Task.FromResult(academies));

        _ = await Sut.OnGetAsync();

        Sut.Academies.Should().BeEquivalentTo(academies);
    }

    [Fact]
    public override async Task OnGetAsync_should_configure_TrustPageMetadata_TabPageName()
    {
        _ = await Sut.OnGetAsync();

        Sut.PageMetadata.TabName.Should().Be("Details");
    }

    [Fact]
    public override async Task OnGetAsync_should_set_active_TabList_to_current_tab()
    {
        _ = await Sut.OnGetAsync();

        Sut.TabList.Should().ContainSingle(l => l.LinkIsActive)
            .Which.AspPage.Should().Be("./Details");
    }
}
