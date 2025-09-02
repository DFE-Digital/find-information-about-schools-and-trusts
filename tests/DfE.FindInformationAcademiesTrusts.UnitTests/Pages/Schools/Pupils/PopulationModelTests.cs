using DfE.FindInformationAcademiesTrusts.Data.Repositories.PupilCensus;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Pupils;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Schools.Pupils;

public class PopulationModelTests : BasePupilsAreaModelTests<PopulationModel>
{
    public PopulationModelTests()
    {
        Sut = new PopulationModel(
            MockDateTimeProvider,
            MockSchoolPupilService,
            MockDataSourceService,
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

        Sut.PageMetadata.SubPageName.Should().Be("Population");
    }

    [Theory]
    [InlineData(2025, 2021)]
    [InlineData(2030, 2026)]
    [InlineData(2020, 2016)]
    public async Task OnGetAsync_should_set_correct_population_data_for_current_time(int latestYear, int earliestYear)
    {
        MockDateTimeProvider.Today.Returns(new DateTime(latestYear, 1, 1, 0, 0, 0, DateTimeKind.Utc));

        var expectedNumberOfYears = latestYear - earliestYear + 1;
        var expectedPopulationDataViewModels = Enumerable
            .Range(earliestYear, expectedNumberOfYears).Select(year =>
                new PopulationDataViewModel(
                    year,
                    "1000",
                    "1000",
                    "100 (10.0%)",
                    "100",
                    "111 (11.1%)",
                    "111",
                    "123 (12.3%)",
                    "123",
                    "135 (13.5%)",
                    "135"
                ));

        await Sut.OnGetAsync();

        Sut.PopulationData.Should().NotBeNull();
        Sut.PopulationData.Should().HaveCount(expectedNumberOfYears);
        Sut.PopulationData.Should().BeEquivalentTo(expectedPopulationDataViewModels);
    }

    [Theory]
    [InlineData(StatisticKind.Suppressed, "Suppressed", "suppressed")]
    [InlineData(StatisticKind.NotPublished, "Not published", "not-published")]
    [InlineData(StatisticKind.NotApplicable, "Not applicable", "not-applicable")]
    [InlineData(StatisticKind.NotAvailable, "Not available", "not-available")]
    [InlineData(StatisticKind.NotYetSubmitted, "Not yet submitted", "not-yet-submitted")]
    public async Task OnGetAsync_should_set_correct_population_data_for_statistics_without_values(StatisticKind kind,
        string expectedDisplayValue, string expectedSortValue)
    {
        var schoolPopulation = new SchoolPopulation(
            Statistic<int>.FromKind(kind),
            Statistic<int>.FromKind(kind),
            Statistic<decimal>.FromKind(kind),
            Statistic<int>.FromKind(kind),
            Statistic<decimal>.FromKind(kind),
            Statistic<int>.FromKind(kind),
            Statistic<decimal>.FromKind(kind),
            Statistic<int>.FromKind(kind),
            Statistic<decimal>.FromKind(kind)
        );

        var expectedPopulationDataViewModel = new PopulationDataViewModel(
            2025,
            expectedDisplayValue,
            expectedSortValue,
            expectedDisplayValue,
            expectedSortValue,
            expectedDisplayValue,
            expectedSortValue,
            expectedDisplayValue,
            expectedSortValue,
            expectedDisplayValue,
            expectedSortValue
        );

        MockSchoolPupilService
            .GetSchoolPopulationStatisticsAsync(Arg.Any<int>(), Arg.Any<CensusYear>(), Arg.Any<CensusYear>())
            .Returns(new AnnualStatistics<SchoolPopulation> { [2025] = schoolPopulation });

        await Sut.OnGetAsync();

        Sut.PopulationData.Should().NotBeNull();
        Sut.PopulationData.Should().HaveCount(1);
        Sut.PopulationData[0].Should().BeEquivalentTo(expectedPopulationDataViewModel);
    }
}
