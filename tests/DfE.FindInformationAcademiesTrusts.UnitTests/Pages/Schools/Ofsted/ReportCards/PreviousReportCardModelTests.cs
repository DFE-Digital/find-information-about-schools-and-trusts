using DfE.FindInformationAcademiesTrusts.Services.Ofsted;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Schools.Ofsted.ReportCards
{
    using DfE.FindInformationAcademiesTrusts.Data;
    using DfE.FindInformationAcademiesTrusts.Pages.Schools.Ofsted.ReportCards;

    public class PreviousReportCardModelTests : BaseOfstedAreaModelTests<PreviousReportCardsModel>
    {
        protected readonly IReportCardsService MockReportCardsService =
            Substitute.For<IReportCardsService>();

        public PreviousReportCardModelTests()
        {
            MockReportCardsService.GetReportCardsAsync(Arg.Any<int>()).Returns(new ReportCardServiceModel());

            Sut = new PreviousReportCardsModel(
                    MockSchoolService,
                    MockSchoolOverviewDetailsService,
                    MockTrustService,
                    MockDataSourceService,
                    MockOfstedSchoolDataExportService,
                    MockDateTimeProvider,
                    MockOtherServicesLinkBuilder,
                    MockSchoolNavMenu,
                    MockReportCardsService
                    )
                { Urn = SchoolUrn };
        }

        [Fact]
        public override async Task OnGetAsync_should_configure_PageMetadata_SubPageName()
        {
            await Sut.OnGetAsync();

            Sut.PageMetadata.SubPageName.Should().Be("Report cards");
            Sut.PageMetadata.TabName.Should().Be("Previous report card");
        }


        [Fact]
        public async Task OnGetAsync_should_set_correct_report_card_data()
        {
            var expectedReportCard = new ReportCardDetails(new DateOnly(2025, 01, 20),
                "https://ofsted.gov.uk/1", "Good", "Good", "Excellent", "Good", "Good", "Outstanding", null,
                "Met", "Good", "None");

            MockReportCardsService.GetReportCardsAsync(SchoolUrn)
                .Returns(new ReportCardServiceModel
                    {
                        PreviousReportCard = expectedReportCard
                    }
                );

            await Sut.OnGetAsync();

            Sut.ReportCard.Should().Be(expectedReportCard);
        }

        [Fact]
        public override async Task OnGetAsync_should_call_populate_tablist()
        {
            _ = await Sut.OnGetAsync();

            _ = MockSchoolNavMenu.Received(1).GetTabLinksForReportCardsOfstedPages(Arg.Any<PreviousReportCardsModel>());
        }
    }
}
