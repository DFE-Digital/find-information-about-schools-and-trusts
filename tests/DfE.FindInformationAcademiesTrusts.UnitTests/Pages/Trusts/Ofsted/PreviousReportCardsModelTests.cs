using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Ofsted.ReportCards;
using DfE.FindInformationAcademiesTrusts.Services.Ofsted;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Trusts.Ofsted;

public class PreviousReportCardsModelTests : BaseOfstedAreaModelTests<PreviousReportCardsModel>
{
    private static readonly ReportCardDetails reportCard = new(new DateOnly(2023, 01, 20),
        "https://ofsted.gov.uk/1", "Good", "Good", "Excellent", "Good", "Good", "Outstanding", null,
        "Met", "Good", "None");

    private static int Urn = 1234;
    private static string SchoolName = "Test school";

    private readonly List<TrustOfstedReportServiceModel<ReportCardServiceModel>> _mockReportCards =
    [
        new()
        {
            Urn = Urn,
            SchoolName = SchoolName,
            ReportDetails = new ReportCardServiceModel
            {
                PreviousReportCard = reportCard,
                LatestReportCard = null

            }
        }
    ];

    public PreviousReportCardsModelTests()
    {
        Sut = new PreviousReportCardsModel(MockDataSourceService,
                MockTrustService,
                MockOfstedTrustDataExportService,
                MockDateTimeProvider,
                MockOfstedService
            )
        { Uid = TrustUid };

        MockOfstedService.GetEstablishmentsInTrustReportCardsAsync(TrustUid).Returns(_mockReportCards);
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

        Sut.PageMetadata.TabName.Should().Be("Previous report card");
    }

    [Fact]
    public async Task OnGetAsync_should_set_ReportCards_from_OfstedService()
    {
        List<ReportCardViewModel> expectedReportCards = [new(Urn, SchoolName, reportCard)];

        Sut.ReportCards.Should().BeEmpty();
        
        _ = await Sut.OnGetAsync();

       await MockOfstedService.Received(1).GetEstablishmentsInTrustReportCardsAsync(TrustUid);

       Sut.ReportCards.Should().BeEquivalentTo(expectedReportCards);
    }
}
