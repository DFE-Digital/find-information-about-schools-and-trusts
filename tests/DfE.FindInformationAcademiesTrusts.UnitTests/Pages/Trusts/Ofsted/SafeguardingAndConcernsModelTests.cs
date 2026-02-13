using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Ofsted;
using DfE.FindInformationAcademiesTrusts.Services.Ofsted;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Trusts.Ofsted
{
    public class SafeguardingAndConcernsModelTests : BaseOfstedAreaModelTests<SafeguardingAndConcernsModel>
    {
        private static int Urn = 1234;
        private static string SchoolName = "Test school";
        private static DateTime DateJoinedTrust = new(2021, 01, 20, 0, 0, 0, DateTimeKind.Local);

        private readonly List<TrustOfstedReportServiceModel<SafeGuardingAndConcernsServiceModel>> _mockSafeGuardingResults =
        [
            new()
            {
                Urn = Urn,
                SchoolName = SchoolName,
                ReportDetails = new SafeGuardingAndConcernsServiceModel(DateJoinedTrust)
            }
        ];

        public SafeguardingAndConcernsModelTests()
        {
            Sut = new SafeguardingAndConcernsModel(MockDataSourceService, MockTrustService, 
                    MockOfstedTrustDataExportService, MockDateTimeProvider, MockOfstedService)
                { Uid = TrustUid };

            MockOfstedService.GetOfstedOverviewSafeguardingAndConcerns(TrustUid).Returns(_mockSafeGuardingResults);
        }

        [Fact]
        public override async Task OnGetAsync_should_configure_TrustPageMetadata_SubPageName()
        {
            _ = await Sut.OnGetAsync();

            Sut.PageMetadata.SubPageName.Should().Be("Safeguarding and concerns");
        }

        [Fact]
        public async Task OnGetAsync_should_get_OverviewInspectionModels()
        {
            Sut.SafeGuardingInspectionModels.Should().BeEmpty();

            _ = await Sut.OnGetAsync();

            await MockOfstedService.Received(1).GetOfstedOverviewSafeguardingAndConcerns(TrustUid);

            Sut.SafeGuardingInspectionModels.Should().BeEquivalentTo(_mockSafeGuardingResults);
        }
    }
}
