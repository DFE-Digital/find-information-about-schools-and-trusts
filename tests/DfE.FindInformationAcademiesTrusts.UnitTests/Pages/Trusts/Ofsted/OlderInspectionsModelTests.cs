using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Ofsted;
using DfE.FindInformationAcademiesTrusts.Services.Ofsted;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Trusts.Ofsted;

public class OlderInspectionsModelTests : BaseOfstedAreaModelTests<OlderInspectionsModel>
{

    private static int Urn = 1234;
    private static string SchoolName = "Test school";

    private readonly List<TrustOfstedReportServiceModel<OlderInspectionServiceModel>> _mockInspections =
    [
        new()
        {
            Urn = Urn,
            SchoolName = SchoolName,
            ReportDetails = new OlderInspectionServiceModel
            {
                Ratings = [new OfstedRating((int)OfstedRatingScore.Good, new DateTime(2023, 01, 20, 0,0,0, DateTimeKind.Local))]
            }
        }
    ];

    public OlderInspectionsModelTests()
    {
        Sut = new OlderInspectionsModel(MockDataSourceService,
                MockTrustService,
                MockOfstedTrustDataExportService,
                MockDateTimeProvider,
                MockOfstedService
            )
        { Uid = TrustUid };

        MockOfstedService.GetEstablishmentsInTrustOlderOfstedRatings(TrustUid).Returns(_mockInspections);
    }

    [Fact]
    public override async Task OnGetAsync_should_configure_TrustPageMetadata_SubPageName()
    {
        _ = await Sut.OnGetAsync();

        Sut.PageMetadata.SubPageName.Should().Be("Older inspections (before November 2025)");
    }

    [Fact]
    public async Task OnGetAsync_should_get_OlderInspections()
    {
        Sut.OlderOfstedInspections.Should().BeEmpty();

        _ = await Sut.OnGetAsync();

        await MockOfstedService.Received(1).GetEstablishmentsInTrustOlderOfstedRatings(TrustUid);

        Sut.OlderOfstedInspections.Should().BeEquivalentTo(_mockInspections);
    }
}
