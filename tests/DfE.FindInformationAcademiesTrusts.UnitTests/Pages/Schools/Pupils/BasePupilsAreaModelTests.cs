using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.PupilCensus;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Pupils;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Export;
using DfE.FindInformationAcademiesTrusts.Services.School;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Schools.Pupils;

public abstract class BasePupilsAreaModelTests<T> : BaseSchoolPageTests<T> where T : PupilsAreaModel
{
    protected readonly IDateTimeProvider MockDateTimeProvider = Substitute.For<IDateTimeProvider>();
    protected readonly ISchoolPupilService MockSchoolPupilService = Substitute.For<ISchoolPupilService>();
    protected readonly ISchoolPupilsExportService MockSchoolPupilsExportService = Substitute.For<ISchoolPupilsExportService>();
    
    protected readonly SchoolPopulation DummySchoolPopulation = new(
        new Statistic<int>.WithValue(1000),
        new Statistic<int>.WithValue(100),
        new Statistic<decimal>.WithValue(10.0m),
        new Statistic<int>.WithValue(111),
        new Statistic<decimal>.WithValue(11.1m),
        new Statistic<int>.WithValue(123),
        new Statistic<decimal>.WithValue(12.3m),
        new Statistic<int>.WithValue(135),
        new Statistic<decimal>.WithValue(13.5m)
    );

    protected readonly Attendance DummyAttendance = new(
        new Statistic<decimal>.WithValue(10.0m),
        new Statistic<decimal>.WithValue(8.5m)
    );

    protected BasePupilsAreaModelTests()
    {
        MockDateTimeProvider.Today.Returns(new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        MockDateTimeProvider.Now.Returns(new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Utc));
        MockSchoolPupilService
            .GetSchoolPopulationStatisticsAsync(Arg.Any<int>(), Arg.Any<CensusYear>(), Arg.Any<CensusYear>())
            .Returns(call =>
            {
                var from = call.ArgAt<CensusYear>(1);
                var to = call.ArgAt<CensusYear>(2);

                var result = new AnnualStatistics<SchoolPopulation>();
                foreach (var year in Enumerable.Range(from.Value, to.Value - from.Value + 1))
                {
                    result[year] = DummySchoolPopulation;
                }

                return result;
            });
        MockSchoolPupilService
            .GetAttendanceStatisticsAsync(Arg.Any<int>(), Arg.Any<CensusYear>(), Arg.Any<CensusYear>())
            .Returns(call =>
            {
                var from = call.ArgAt<CensusYear>(1);
                var to = call.ArgAt<CensusYear>(2);

                var result = new AnnualStatistics<Attendance>();
                foreach (var year in Enumerable.Range(from.Value, to.Value - from.Value + 1))
                {
                    result[year] = DummyAttendance;
                }

                return result;
            });
    }

    [Fact]
    public override async Task OnGetAsync_should_configure_PageMetadata_PageName()
    {
        await Sut.OnGetAsync();

        Sut.PageMetadata.PageName.Should().Be("Pupils");
    }

    [Fact]
    public override async Task OnGetAsync_sets_correct_data_source_list()
    {
        await Task.CompletedTask;
        _ = await Sut.OnGetAsync();
        await MockDataSourceService.Received(1).GetAsync(Source.CompareSchoolCollegePerformanceEnglandPopulation);

        Sut.DataSourcesPerPage.Should().BeEquivalentTo([
            new DataSourcePageListEntry("Population", [
                new DataSourceListEntry(Mocks.MockDataSourceService.CompareSchoolCollegePerformanceEnglandPopulation)
            ]),
            new DataSourcePageListEntry("Attendance", [
                new DataSourceListEntry(Mocks.MockDataSourceService.CompareSchoolCollegePerformanceEnglandAttendance)
            ])
        ]);
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
        result.As<FileContentResult>().FileDownloadName.Should().Be("Pupil population-Cool school-2025-07-01.xlsx");
    }

    [Fact]
    public async Task OnGetExportAsync_sanitises_school_name_for_file()
    {
        MockSchoolService.GetSchoolSummaryAsync(SchoolUrn)
            .Returns(DummySchoolSummary with { Name = "  School name with invalid characters\0/ " });

        var result = await Sut.OnGetExportAsync(SchoolUrn);

        result.Should().BeOfType<FileContentResult>();
        result.As<FileContentResult>().FileDownloadName.Should()
            .Be("Pupil population-School name with invalid characters-2025-07-01.xlsx");
    }
}
