using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Ofsted;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Trusts.Ofsted;

public class CurrentRatingsModelTests : BaseOfstedAreaModelTests<CurrentRatingsModel>
{
    public CurrentRatingsModelTests()
    {
        Sut = new CurrentRatingsModel(MockDataSourceService,
                MockTrustService,
                MockAcademyService,
                MockOfstedTrustDataExportService,
                MockDateTimeProvider
            )
            { Uid = TrustUid };
    }

    [Fact]
    public override async Task OnGetAsync_should_configure_TrustPageMetadata_SubPageName()
    {
        _ = await Sut.OnGetAsync();

        Sut.PageMetadata.SubPageName.Should().Be("Current ratings");
    }
}
