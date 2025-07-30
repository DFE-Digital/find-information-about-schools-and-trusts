using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Ofsted;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Trusts.Ofsted;

public class PreviousRatingsModelTests : BaseOfstedAreaModelTests<PreviousRatingsModel>
{
    public PreviousRatingsModelTests()
    {
        Sut = new PreviousRatingsModel(MockDataSourceService,
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

        Sut.PageMetadata.SubPageName.Should().Be("Previous ratings");
    }
}
