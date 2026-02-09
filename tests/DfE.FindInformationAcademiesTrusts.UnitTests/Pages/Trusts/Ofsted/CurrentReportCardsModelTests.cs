using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Ofsted.ReportCards;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Trusts.Ofsted;

public class CurrentReportCardsModelTests : BaseOfstedAreaModelTests<CurrentReportCardsModel>
{
    public CurrentReportCardsModelTests()
    {
        Sut = new CurrentReportCardsModel(MockDataSourceService,
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

        Sut.PageMetadata.SubPageName.Should().Be("Report cards");
    }


    [Fact]
    public async Task OnGetAsync_should_configure_TrustPageMetadata_TabName()
    {
        _ = await Sut.OnGetAsync();

        Sut.PageMetadata.TabName.Should().Be("Current report card");
    }
}
