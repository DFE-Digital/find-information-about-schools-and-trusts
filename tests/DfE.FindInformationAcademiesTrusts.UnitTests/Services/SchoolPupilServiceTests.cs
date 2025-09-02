using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.PupilCensus;
using DfE.FindInformationAcademiesTrusts.Services.School;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Services;

public class SchoolPupilServiceTests
{
    private const int Urn = 123456;

    private readonly SchoolPupilService _sut;
    private readonly IDateTimeProvider _mockDateTimeProvider = Substitute.For<IDateTimeProvider>();
    private readonly IPupilCensusRepository _mockPupilCensusRepository = Substitute.For<IPupilCensusRepository>();

    private readonly SchoolPopulation _dummySchoolPopulation = new(
        new Statistic<int>.WithValue(100),
        new Statistic<int>.WithValue(10),
        new Statistic<decimal>.WithValue(10.0m),
        new Statistic<int>.WithValue(11),
        new Statistic<decimal>.WithValue(11.0m),
        new Statistic<int>.WithValue(12),
        new Statistic<decimal>.WithValue(12.0m),
        new Statistic<int>.WithValue(13),
        new Statistic<decimal>.WithValue(13.0m)
    );

    public SchoolPupilServiceTests()
    {
        _mockPupilCensusRepository.GetSchoolPopulationStatisticsAsync(Arg.Any<int>())
            .Returns(new AnnualStatistics<SchoolPopulation>());

        var dummySchoolPopulationStatistics = new AnnualStatistics<SchoolPopulation>
        {
            { 2025, _dummySchoolPopulation },
            { 2024, _dummySchoolPopulation },
            { 2023, _dummySchoolPopulation },
            { 2022, _dummySchoolPopulation },
            { 2021, _dummySchoolPopulation },
            { 2020, _dummySchoolPopulation },
            { 2019, _dummySchoolPopulation },
            { 2018, _dummySchoolPopulation },
            { 2017, _dummySchoolPopulation },
            { 2016, _dummySchoolPopulation },
            { 2015, _dummySchoolPopulation }
        };
        _mockPupilCensusRepository.GetSchoolPopulationStatisticsAsync(Urn).Returns(dummySchoolPopulationStatistics);

        _mockDateTimeProvider.Today.Returns(new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc));

        _sut = new SchoolPupilService(_mockDateTimeProvider, _mockPupilCensusRepository);
    }

    [Theory]
    [InlineData(2019, 2024, 6)]
    [InlineData(2014, 2018, 5)]
    [InlineData(2009, 2012, 4)]
    public async Task GetSchoolPopulationStatisticsAsync_returns_Unknown_results_when_no_data_for_the_urn_could_be_found(int from, int to, int expectedNumberOfYears)
    {
        var result = await _sut.GetSchoolPopulationStatisticsAsync(999999, from, to);

        result.Should().HaveCount(expectedNumberOfYears);
        result.Values.Should().AllBeEquivalentTo(SchoolPopulation.Unknown);
    }

    [Theory]
    [InlineData(2025)]
    [InlineData(2020)]
    [InlineData(2015)]
    public async Task GetSchoolPopulationStatisticsAsync_returns_one_result_when_from_and_to_are_the_same_year(int year)
    {
        var result = await _sut.GetSchoolPopulationStatisticsAsync(Urn, year, year);

        result.Should().HaveCount(1);
    }

    [Theory]
    [InlineData(2020, 2019)]
    [InlineData(2025, 1999)]
    public async Task
        GetSchoolPopulationStatisticsAsync_throws_ArgumentOutOfRangeException_when_from_is_after_to(int from, int to)
    {
        var act = () => _sut.GetSchoolPopulationStatisticsAsync(Urn, from, to);

        await act.Should().ThrowAsync<ArgumentOutOfRangeException>().WithMessage($"to.Value ('{to}') must be greater than or equal to '{from}'. (Parameter 'to.Value')\nActual value was {to}.");
    }

    [Fact]
    public async Task GetSchoolPopulationStatisticsAsync_fills_in_future_years_with_NotYetSubmitted_SchoolPopulation()
    {
        var result = await _sut.GetSchoolPopulationStatisticsAsync(Urn, 3000, 3005);
        
        result.Should().NotBeEmpty();
        result.Should().HaveCount(6);
        result.Values.Should().AllBeEquivalentTo(SchoolPopulation.NotYetSubmitted);
    }

    [Theory]
    [InlineData(2025, 1, 1, 2024)]
    [InlineData(2024, 6, 15, 2023)]
    [InlineData(2023, 9, 30, 2022)]
    public async Task GetSchoolPopulationStatisticsAsync_considers_this_years_result_to_be_NotYetSubmitted_when_today_is_before_october_31st(
        int year,
        int month,
        int day,
        int expectedMostRecentYearOfData
    )
    {
        _mockDateTimeProvider.Today.Returns(new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc));

        var result = await _sut.GetSchoolPopulationStatisticsAsync(Urn, 2021, 2025);

        result.Should().NotBeEmpty();
        result.Should().HaveCount(5);
        foreach (var y in Enumerable.Range(2021, 5))
        {
            result[y].Should()
                .BeEquivalentTo(y > expectedMostRecentYearOfData ? SchoolPopulation.NotYetSubmitted : _dummySchoolPopulation);
        }
    }

    [Theory]
    [InlineData(2025, 10, 31, 2025)]
    [InlineData(2024, 11, 1, 2024)]
    [InlineData(2023, 12, 31, 2023)]
    public async Task GetSchoolPopulationStatisticsAsync_considers_this_years_result_to_be_available_when_today_is_october_31st_or_later(
        int year,
        int month,
        int day,
        int expectedMostRecentYearOfData
    )
    {
        _mockDateTimeProvider.Today.Returns(new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc));

        var result = await _sut.GetSchoolPopulationStatisticsAsync(Urn, 2021, 2025);

        result.Should().NotBeEmpty();
        result.Should().HaveCount(5);
        foreach (var y in Enumerable.Range(2021, 5))
        {
            result[y].Should()
                .BeEquivalentTo(y > expectedMostRecentYearOfData ? SchoolPopulation.NotYetSubmitted : _dummySchoolPopulation);
        }
    }

    [Fact]
    public async Task GetSchoolPopulationStatisticsAsync_fills_in_missing_years_with_Unknown_SchoolPopulation()
    {
        var populationStatistics = new AnnualStatistics<SchoolPopulation>
        {
            { 2023, _dummySchoolPopulation },
            { 2021, _dummySchoolPopulation },
            { 2020, _dummySchoolPopulation }
        };

        _mockPupilCensusRepository.GetSchoolPopulationStatisticsAsync(Urn).Returns(populationStatistics);

        var result = await _sut.GetSchoolPopulationStatisticsAsync(Urn, 2021, 2024);

        result.Should().NotBeEmpty();
        result.Should().HaveCount(4);
        result[2024].Should().BeEquivalentTo(SchoolPopulation.Unknown);
        result[2023].Should().BeEquivalentTo(_dummySchoolPopulation);
        result[2022].Should().BeEquivalentTo(SchoolPopulation.Unknown);
        result[2021].Should().BeEquivalentTo(_dummySchoolPopulation);
    }
}
