using DfE.FindInformationAcademiesTrusts.Pages.Schools.Ofsted;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Ofsted.ReportCards;
using Microsoft.AspNetCore.Mvc;
using NSubstitute.ReturnsExtensions;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Schools.Ofsted
{
    public class OfstedOverviewModelTests : BaseOfstedAreaModelTests<OfstedOverviewModel>
    {
 
        public OfstedOverviewModelTests()
        {
            Sut = new OfstedOverviewModel(
                    MockSchoolService,
                    MockSchoolOverviewDetailsService,
                    MockTrustService,
                    MockDataSourceService,
                    MockOfstedSchoolDataExportService,
                    MockDateTimeProvider,
                    MockOtherServicesLinkBuilder,
                    MockSchoolNavMenu,
                    MockOfstedService)
                { Urn = SchoolUrn };
        }


        [Fact]
        public override async Task OnGetAsync_should_configure_PageMetadata_SubPageName()
        {
            await Sut.OnGetAsync();

            Sut.PageMetadata.SubPageName.Should().Be("Overview");
        }

        [Fact]
        public async Task OnGet_returns_NotFoundResult_for_unknown_urn()
        {
            MockSchoolService.GetSchoolSummaryAsync(Arg.Any<int>()).ReturnsNull();

            var result = await Sut.OnGetAsync();

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task OnGet_ShouldGetOverviewInspection_and_SetPublicProperty()
        {
            Sut.OverviewInspectionModel.Current.Should().BeNull();
            Sut.OverviewInspectionModel.ShortInspection.Should().BeNull();
            Sut.OverviewInspectionModel.Previous.Should().BeNull();
            
            await Sut.OnGetAsync();

            await MockOfstedService.Received(1).GetOfstedOverviewInspectionAsync(SchoolUrn);
        }


        [Fact]
        public override async Task OnGetAsync_should_call_populate_tablist()
        {
            await Sut.OnGetAsync();
            Sut.TabList.Should().BeEmpty();
        }
    }
}
