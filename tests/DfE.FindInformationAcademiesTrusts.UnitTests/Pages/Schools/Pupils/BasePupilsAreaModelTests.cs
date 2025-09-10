using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.PupilCensus;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Pupils;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.School;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Schools.Pupils;

public abstract class BasePupilsAreaModelTests<T> : BaseSchoolPageTests<T> where T : PupilsAreaModel
{
    protected readonly IDateTimeProvider MockDateTimeProvider = Substitute.For<IDateTimeProvider>();
    protected readonly ISchoolPupilService MockSchoolPupilService = Substitute.For<ISchoolPupilService>();
    
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

    protected BasePupilsAreaModelTests()
    {
        MockDateTimeProvider.Today.Returns(new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc));
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
        await MockDataSourceService.Received(1).GetAsync(Source.CompareSchoolCollegePerformanceEngland);

        Sut.DataSourcesPerPage.Should().BeEquivalentTo([
            new DataSourcePageListEntry("Population", [
                new DataSourceListEntry(Mocks.MockDataSourceService.CompareSchoolCollegePerformanceEngland)
            ])
        ]);
    }
}
