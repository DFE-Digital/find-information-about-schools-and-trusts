using DfE.FindInformationAcademiesTrusts.Data.Repositories.PupilCensus;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Pupils;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Schools.Pupils;

public class AttendanceModelTests : BasePupilsAreaModelTests<AttendanceModel>
{
    public AttendanceModelTests()
    {
        Sut = new AttendanceModel(
            MockSchoolPupilService,
            MockDateTimeProvider,
            MockDataSourceService,
            MockSchoolPupilsExportService,
            MockSchoolService,
            MockTrustService,
            MockSchoolNavMenu)
        {
            Urn = SchoolUrn
        };
    }

    public override async Task OnGetAsync_should_configure_PageMetadata_SubPageName()
    {
        await Sut.OnGetAsync();

        Sut.PageMetadata.SubPageName.Should().Be("Attendance");
    }

    [Theory]
    [InlineData(2025, 2021)]
    [InlineData(2030, 2026)]
    [InlineData(2020, 2016)]
    public async Task OnGetAsync_should_set_correct_attendance_data_for_current_time(int latestYear, int earliestYear)
    {
        MockDateTimeProvider.Today.Returns(new DateTime(latestYear, 1, 1, 0, 0, 0, DateTimeKind.Utc));

        var expectedNumberOfYears = latestYear - earliestYear + 1;
        var expectedAttendanceDataViewModels = Enumerable
            .Range(earliestYear, expectedNumberOfYears).Select(year =>
                new AttendanceDataViewModel(
                    year,
                    "10.0%",
                    "10.0",
                    "8.5%",
                    "8.5"
                ));

        await Sut.OnGetAsync();

        Sut.AttendanceData.Should().NotBeNull();
        Sut.AttendanceData.Should().HaveCount(expectedNumberOfYears);
        Sut.AttendanceData.Should().BeEquivalentTo(expectedAttendanceDataViewModels);
    }

    [Theory]
    [InlineData(StatisticKind.Suppressed, "Suppressed", "suppressed")]
    [InlineData(StatisticKind.NotPublished, "Not published", "not-published")]
    [InlineData(StatisticKind.NotApplicable, "Not applicable", "not-applicable")]
    [InlineData(StatisticKind.NotAvailable, "Not available", "not-available")]
    [InlineData(StatisticKind.NotYetSubmitted, "Not yet submitted", "not-yet-submitted")]
    public async Task OnGetAsync_should_set_correct_attendance_data_for_statistics_without_values(StatisticKind kind,
        string expectedDisplayValue, string expectedSortValue)
    {
        var attendance = new Attendance(
            Statistic<decimal>.FromKind(kind),
            Statistic<decimal>.FromKind(kind)
        );

        var expectedAttendanceDataViewModel = new AttendanceDataViewModel(
            2025,
            expectedDisplayValue,
            expectedSortValue,
            expectedDisplayValue,
            expectedSortValue
        );

        MockSchoolPupilService
            .GetAttendanceStatisticsAsync(Arg.Any<int>(), Arg.Any<CensusYear>(), Arg.Any<CensusYear>())
            .Returns(new AnnualStatistics<Attendance> { [2025] = attendance });

        await Sut.OnGetAsync();

        Sut.AttendanceData.Should().NotBeNull();
        Sut.AttendanceData.Should().HaveCount(1);
        Sut.AttendanceData[0].Should().BeEquivalentTo(expectedAttendanceDataViewModel);
    }
}
