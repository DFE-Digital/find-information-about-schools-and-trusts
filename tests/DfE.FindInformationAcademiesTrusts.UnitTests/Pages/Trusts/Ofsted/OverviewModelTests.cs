using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Ofsted;
using DfE.FindInformationAcademiesTrusts.Services.School;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Trusts.Ofsted
{
    public class OverviewModelTests : BaseOfstedAreaModelTests<OverviewModel>
    {
        private readonly OfstedOverviewInspectionServiceModel mockInspectionResult = new(
            null, null, new ShortInspectionOverviewServiceModel
            {
                BeforeOrAfterJoining = BeforeOrAfterJoining.Before,
                InspectionOutcome = "School remains Good",
                InspectionDate = new DateOnly(2025, 7, 1),
                IsReportCard = false
            })
        {
            Urn = 1123,
            SchoolName = "Test school"
        };

        public OverviewModelTests()
        {
            Sut = new OverviewModel(MockDataSourceService, MockTrustService, MockAcademyService,
                    MockOfstedTrustDataExportService, MockDateTimeProvider, MockOfstedService)
                { Uid = TrustUid };

            MockOfstedService.GetOfstedOverviewInspectionForTrustAsync(TrustUid).Returns([mockInspectionResult]);
        }

        [Fact]
        public override async Task OnGetAsync_should_configure_TrustPageMetadata_SubPageName()
        {
            _ = await Sut.OnGetAsync();

            Sut.PageMetadata.SubPageName.Should().Be("Overview");
        }

        [Fact]
        public async Task OnGetAsnc_should_get_OverviewInspectionModels()
        {
            Sut.OverviewInspectionModels.Should().BeEmpty();

            _ = await Sut.OnGetAsync();

            await MockOfstedService.Received(1).GetOfstedOverviewInspectionForTrustAsync(TrustUid);

            Sut.OverviewInspectionModels.Should().BeEquivalentTo([mockInspectionResult]);
        }
    }
}
