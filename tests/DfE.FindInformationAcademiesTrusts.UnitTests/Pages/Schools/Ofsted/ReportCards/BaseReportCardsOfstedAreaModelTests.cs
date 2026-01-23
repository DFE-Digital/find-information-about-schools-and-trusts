using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Ofsted;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Export;
using DfE.FindInformationAcademiesTrusts.Services.Ofsted;
using DfE.FindInformationAcademiesTrusts.Services.School;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Schools.Ofsted.ReportCards;

public abstract class BaseReportCardsOfstedAreaModelTests<T> : BaseSchoolPageTests<T> where T : OfstedAreaModel
{
    protected readonly ISchoolOverviewDetailsService MockSchoolOverviewDetailsService =
        Substitute.For<ISchoolOverviewDetailsService>();

    protected readonly IOfstedSchoolDataExportService MockOfstedSchoolDataExportService =
        Substitute.For<IOfstedSchoolDataExportService>();

    protected readonly IDateTimeProvider MockDateTimeProvider = Substitute.For<IDateTimeProvider>();

    protected readonly IOtherServicesLinkBuilder MockOtherServicesLinkBuilder =
        Substitute.For<IOtherServicesLinkBuilder>();

    protected readonly IReportCardsService MockReportCardsService =
        Substitute.For<IReportCardsService>();

    protected readonly SchoolOverviewServiceModel DummySchoolDetails =
        new("Cool school", "some street, in a town", "Yorkshire", "Leeds", "Secondary", new AgeRange(11, 18),
            NurseryProvision.NotRecorded);

    protected BaseReportCardsOfstedAreaModelTests()
    {
        MockSchoolOverviewDetailsService.GetSchoolOverviewDetailsAsync(Arg.Any<int>(), Arg.Any<SchoolCategory>())
            .Returns(DummySchoolDetails);

        MockOtherServicesLinkBuilder.OfstedReportLinkForSchool(Arg.Any<int>())
            .Returns(call => $"https://reports.ofsted.gov.uk/test/{call.ArgAt<int>(0)}");

        MockOfstedSchoolDataExportService.BuildAsync(Arg.Any<int>()).Returns("Some Excel data"u8.ToArray());

        MockDateTimeProvider.Now.Returns(new DateTime(2025, 7, 1));

        MockReportCardsService.GetReportCardsAsync(Arg.Any<int>()).Returns(new ReportCardServiceModel());
    }

    [Fact]
    public override async Task OnGetAsync_should_configure_PageMetadata_PageName()
    {
        await Sut.OnGetAsync();

        Sut.PageMetadata.PageName.Should().Be("Ofsted");
    }

    [Fact]
    public override async Task OnGetAsync_sets_correct_data_source_list()
    {
        await VerifyCorrectDataSources(SchoolUrn);

        MockDataSourceService.ClearReceivedCalls();

        await VerifyCorrectDataSources(AcademyUrn);
    }

    [Fact]
    public async Task OnGetAsync_sets_correct_DateJoinedTrust_for_academy()
    {
        DummySchoolDetails.DateJoinedTrust = DateOnly.Parse("2011-04-03");
        Sut.Urn = AcademyUrn;

        _ = await Sut.OnGetAsync();

        Sut.DateJoinedTrust.Should().NotBeNull();
        Sut.DateJoinedTrust.Should().Be(DateTime.Parse("2011-04-03"));
    }

    [Fact]
    public async Task OnGetAsync_does_not_set_DateJoinedTrust_for_school()
    {
        _ = await Sut.OnGetAsync();

        Sut.DateJoinedTrust.Should().BeNull();
    }

    //[Fact]
    //public async Task OnGetASync_sets_correct_OfstedReportUrl_for_school()
    //{
    //    Sut.Urn = SchoolUrn;

    //    _ = await Sut.OnGetAsync();

    //    Sut.OfstedReportUrl.Should().Be($"https://reports.ofsted.gov.uk/test/{SchoolUrn}");
    //}

    //[Fact]
    //public async Task OnGetASync_sets_correct_OfstedReportUrl_for_academy()
    //{
    //    Sut.Urn = AcademyUrn;

    //    _ = await Sut.OnGetAsync();

    //    Sut.OfstedReportUrl.Should().Be($"https://reports.ofsted.gov.uk/test/{AcademyUrn}");
    //}

    [Fact]
    public async Task OnGetExportAsync_returns_NotFoundResult_for_unknown_urn()
    {
        var result = await Sut.OnGetExportAsync(999111);

        result.Should().BeOfType<NotFoundResult>();
    }

    //[Fact]
    //public async Task OnGetExportAsync_returns_expected_file()
    //{
    //    var result = await Sut.OnGetExportAsync(SchoolUrn);

    //    result.Should().BeOfType<FileContentResult>();
    //    result.As<FileContentResult>().FileDownloadName.Should().Be("Ofsted-Cool school-2025-07-01.xlsx");
    //}

    //[Fact]
    //public async Task OnGetExportAsync_sanitises_school_name_for_file()
    //{
    //    MockSchoolService.GetSchoolSummaryAsync(SchoolUrn)
    //        .Returns(DummySchoolSummary with { Name = "  School name with invalid characters\0/ " });

    //    var result = await Sut.OnGetExportAsync(SchoolUrn);

    //    result.Should().BeOfType<FileContentResult>();
    //    result.As<FileContentResult>().FileDownloadName.Should()
    //        .Be("Ofsted-School name with invalid characters-2025-07-01.xlsx");
    //}

    [Fact]
    public async Task OnGetAsync_should_populate_TabList_to_tabs()
    {
        _ = await Sut.OnGetAsync();

        Sut.TabList.Should()
            .SatisfyRespectively(
                l =>
                {
                    l.LinkDisplayText.Should().Be("Current report card");
                    l.AspPage.Should().Be("./CurrentReportCards");
                    l.TestId.Should().Be("report-cards-current-report-card-tab");
                },
                l =>
                {
                    l.LinkDisplayText.Should().Be("Previous report card");
                    l.AspPage.Should().Be("./PreviousReportCards");
                    l.TestId.Should().Be("report-cards-previous-report-card-tab");
                });
    }

    private async Task VerifyCorrectDataSources(int urn)
    {
        Sut.Urn = urn;

        _ = await Sut.OnGetAsync();
        await MockDataSourceService.Received(1).GetAsync(Source.Gias);
        await MockDataSourceService.Received(1).GetAsync(Source.Mis);


        Sut.DataSourcesPerPage.Should().BeEquivalentTo([
            new DataSourcePageListEntry("Overview", [
                new DataSourceListEntry(Mocks.MockDataSourceService.Gias, "Date joined trust"),
                new DataSourceListEntry(Mocks.MockDataSourceService.Mis, "All inspection types"),
                new DataSourceListEntry(Mocks.MockDataSourceService.Mis, "All inspection dates")
            ]),
            new DataSourcePageListEntry("Report cards", [
                new DataSourceListEntry(Mocks.MockDataSourceService.Mis, "Current report card ratings"),
                new DataSourceListEntry(Mocks.MockDataSourceService.Mis, "Previous report card ratings")
            ]),
            new DataSourcePageListEntry("Older inspections (before November 2025)", [
                new DataSourceListEntry(Mocks.MockDataSourceService.Mis, "Inspection ratings after September 24"),
                new DataSourceListEntry(Mocks.MockDataSourceService.Mis, "Inspection ratings before September 24")
            ])
        ]);
    }
}
