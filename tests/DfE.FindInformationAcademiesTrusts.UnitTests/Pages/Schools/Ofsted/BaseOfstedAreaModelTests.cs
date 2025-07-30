using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Ofsted;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Export;
using DfE.FindInformationAcademiesTrusts.Services.School;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Schools.Ofsted;

public abstract class BaseOfstedAreaModelTests<T> : BaseSchoolPageTests<T> where T : OfstedAreaModel
{
    protected readonly ISchoolOverviewDetailsService MockSchoolOverviewDetailsService =
        Substitute.For<ISchoolOverviewDetailsService>();

    protected readonly IOfstedSchoolDataExportService MockOfstedSchoolDataExportService =
        Substitute.For<IOfstedSchoolDataExportService>();

    protected readonly IDateTimeProvider MockDateTimeProvider = Substitute.For<IDateTimeProvider>();

    protected readonly IOtherServicesLinkBuilder MockOtherServicesLinkBuilder =
        Substitute.For<IOtherServicesLinkBuilder>();

    protected readonly SchoolOverviewServiceModel DummySchoolDetails =
        new("Cool school", "some street, in a town", "Yorkshire", "Leeds", "Secondary", new AgeRange(11, 18),
            NurseryProvision.NotRecorded);

    protected BaseOfstedAreaModelTests()
    {
        MockSchoolOverviewDetailsService.GetSchoolOverviewDetailsAsync(Arg.Any<int>(), Arg.Any<SchoolCategory>())
            .Returns(DummySchoolDetails);

        MockOtherServicesLinkBuilder.OfstedReportLinkForSchool(Arg.Any<int>())
            .Returns(call => $"https://reports.ofsted.gov.uk/test/{call.ArgAt<int>(0)}");

        MockOfstedSchoolDataExportService.BuildAsync(Arg.Any<int>()).Returns("Some Excel data"u8.ToArray());

        MockDateTimeProvider.Now.Returns(new DateTime(2025, 7, 1));
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

    [Fact]
    public async Task OnGetASync_sets_correct_OfstedReportUrl_for_school()
    {
        Sut.Urn = SchoolUrn;

        _ = await Sut.OnGetAsync();

        Sut.OfstedReportUrl.Should().Be($"https://reports.ofsted.gov.uk/test/{SchoolUrn}");
    }

    [Fact]
    public async Task OnGetASync_sets_correct_OfstedReportUrl_for_academy()
    {
        Sut.Urn = AcademyUrn;

        _ = await Sut.OnGetAsync();

        Sut.OfstedReportUrl.Should().Be($"https://reports.ofsted.gov.uk/test/{AcademyUrn}");
    }

    [Fact]
    public async Task OnGetExportAsync_returns_NotFoundResult_for_unknown_urn()
    {
        var result = await Sut.OnGetExportAsync(999111);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task OnGetExportAsync_returns_expected_file()
    {
        var result = await Sut.OnGetExportAsync(SchoolUrn);

        result.Should().BeOfType<FileContentResult>();
        result.As<FileContentResult>().FileDownloadName.Should().Be("Ofsted-Cool school-2025-07-01.xlsx");
    }

    [Fact]
    public async Task OnGetExportAsync_sanitises_school_name_for_file()
    {
        MockSchoolService.GetSchoolSummaryAsync(SchoolUrn)
            .Returns(DummySchoolSummary with { Name = "  School name with invalid characters\0/ " });

        var result = await Sut.OnGetExportAsync(SchoolUrn);

        result.Should().BeOfType<FileContentResult>();
        result.As<FileContentResult>().FileDownloadName.Should()
            .Be("Ofsted-School name with invalid characters-2025-07-01.xlsx");
    }

    private async Task VerifyCorrectDataSources(int urn)
    {
        Sut.Urn = urn;

        _ = await Sut.OnGetAsync();
        await MockDataSourceService.Received(1).GetAsync(Source.Mis);
        await MockDataSourceService.Received(1).GetAsync(Source.MisFurtherEducation);

        List<DataSourceServiceModel> expectedDataSources =
        [
            Mocks.MockDataSourceService.Mis,
            Mocks.MockDataSourceService.MisFurtherEducation
        ];

        Sut.DataSourcesPerPage.Should().BeEquivalentTo([
            new DataSourcePageListEntry("Single headline grades", [
                new DataSourceListEntry(expectedDataSources, "Single headline grades were"),
                new DataSourceListEntry(expectedDataSources, "All inspection dates were")
            ])
        ]);
    }
}
