using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Ofsted;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Trusts.Ofsted;

public class OlderInspectionsModelTests : BaseOfstedAreaModelTests<OlderInspectionsModel>
{
    public OlderInspectionsModelTests()
    {
        Sut = new OlderInspectionsModel(MockDataSourceService,
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

        Sut.PageMetadata.SubPageName.Should().Be("Older inspections (before November 2025)");
    }
}
