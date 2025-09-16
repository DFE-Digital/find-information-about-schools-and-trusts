using DfE.FindInformationAcademiesTrusts.Data.Repositories.PupilCensus;
using DfE.FindInformationAcademiesTrusts.Services.Trust;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Services;

public class TrustPupilServiceTests
{
    private const string Uid = "1234";

    private readonly TrustPupilService _sut;
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
    
    public TrustPupilServiceTests()
    {
        _mockPupilCensusRepository.GetMostRecentPopulationStatisticsForTrustAsync(Arg.Any<string>())
            .Returns(new TrustStatistics<SchoolPopulation>());

        var dummyStatisticsForTrust = new TrustStatistics<SchoolPopulation>
        {
            { 123456, _dummySchoolPopulation },
            { 234567, _dummySchoolPopulation },
            { 345678, _dummySchoolPopulation },
            { 456789, _dummySchoolPopulation },
            { 567890, _dummySchoolPopulation }
        };
        
        _mockPupilCensusRepository.GetMostRecentPopulationStatisticsForTrustAsync(Uid)
            .Returns(dummyStatisticsForTrust);
        
        _sut = new TrustPupilService(_mockPupilCensusRepository);
    }

    [Fact]
    public async Task GetTotalPupilCountForTrustAsync_returns_0_when_trust_is_not_found()
    {
        var result = await _sut.GetTotalPupilCountForTrustAsync("999999");
        
        result.Should().Be(0);
    }
    
    [Theory]
    [InlineData(1, 100, 100)]
    [InlineData(5, 500, 2500)]
    [InlineData(30, 1000,30000)]
    public async Task GetTotalPupilCountForTrustAsync_returns_total_pupil_count_for_trust(int numberOfSchools, int pupilsPerSchool, int expectedTotal)
    {
        var statistics = new TrustStatistics<SchoolPopulation>();

        for (var i = 0; i < numberOfSchools; i++)
        {
            statistics[i] = _dummySchoolPopulation with
            {
                PupilsOnRole = new Statistic<int>.WithValue(pupilsPerSchool)
            };
        }
        
        _mockPupilCensusRepository.GetMostRecentPopulationStatisticsForTrustAsync(Uid)
            .Returns(statistics);
        
        var result = await _sut.GetTotalPupilCountForTrustAsync(Uid);
        
        result.Should().Be(expectedTotal);
    }
    
    [Theory]
    [InlineData(StatisticKind.Suppressed)]
    [InlineData(StatisticKind.NotPublished)]
    [InlineData(StatisticKind.NotApplicable)]
    [InlineData(StatisticKind.NotAvailable)]
    [InlineData(StatisticKind.NotYetSubmitted)]
    public async Task GetTotalPupilCountForTrustAsync_counts_0_for_school_when_pupil_count_does_not_have_value(StatisticKind statisticKind)
    {
        var statistics = new TrustStatistics<SchoolPopulation>
        {
            [1234] = _dummySchoolPopulation with { PupilsOnRole = Statistic<int>.FromKind(statisticKind) },
            [2345] = _dummySchoolPopulation
        };
        
        _mockPupilCensusRepository.GetMostRecentPopulationStatisticsForTrustAsync(Uid)
            .Returns(statistics);
        
        var result = await _sut.GetTotalPupilCountForTrustAsync(Uid);
        
        result.Should().Be(100);
    }
    
    [Fact]
    public async Task GetPupilCountsForSchoolsInTrustAsync_returns_empty_result_when_trust_is_not_found()
    {
        var result = await _sut.GetPupilCountsForSchoolsInTrustAsync("999999");
        
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetPupilCountsForSchoolsInTrustAsync_returns_pupil_counts_for_each_school_in_trust()
    {
        var result = await _sut.GetPupilCountsForSchoolsInTrustAsync(Uid);
        
        result.Should().HaveCount(5);
        result.Keys.Should().BeEquivalentTo([123456, 234567, 345678, 456789, 567890]);
        result.Values.Should().AllBeEquivalentTo(new Statistic<int>.WithValue(100));
    }
    
    [Theory]
    [InlineData(StatisticKind.Suppressed)]
    [InlineData(StatisticKind.NotPublished)]
    [InlineData(StatisticKind.NotApplicable)]
    [InlineData(StatisticKind.NotAvailable)]
    [InlineData(StatisticKind.NotYetSubmitted)]
    public async Task GetPupilCountsForSchoolsInTrustAsync_returns_statistic_without_value_when_pupil_count_for_school_does_not_have_value(StatisticKind statisticKind)
    {
        var statistics = new TrustStatistics<SchoolPopulation>
        {
            [123456] = _dummySchoolPopulation with { PupilsOnRole = Statistic<int>.FromKind(statisticKind) }
        };
        
        _mockPupilCensusRepository.GetMostRecentPopulationStatisticsForTrustAsync(Uid)
            .Returns(statistics);
        
        var result = await _sut.GetPupilCountsForSchoolsInTrustAsync(Uid);
        
        result.Should().HaveCount(1);
        result.Keys.Should().BeEquivalentTo([123456]);
        result.Values.Should().AllBeEquivalentTo(Statistic<int>.FromKind(statisticKind));
    }
}
