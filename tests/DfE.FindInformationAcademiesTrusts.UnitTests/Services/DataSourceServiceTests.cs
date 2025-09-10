using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Data.FiatDb.Repositories;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.UnitTests.Mocks;
using FluentAssertions.Execution;
using NSubstitute.ExceptionExtensions;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Services;

public class DataSourceServiceTests
{
    private readonly DataSourceService _sut;
    private readonly IDataSourceRepository _mockDataSourceRepository = Substitute.For<IDataSourceRepository>();

    private readonly IFiatDataSourceRepository _mockFiatDataSourceRepository =
        Substitute.For<IFiatDataSourceRepository>();

    private readonly IFreeSchoolMealsAverageProvider _mockFreeSchoolMealsAverageProvider =
        Substitute.For<IFreeSchoolMealsAverageProvider>();

    private readonly MockMemoryCache _mockMemoryCache = new();

    private readonly Dictionary<Source, DataSource> _dummyDataSources = new()
    {
        { Source.Cdm, GetDummyDataSource(Source.Cdm, UpdateFrequency.Daily) },
        { Source.Complete, GetDummyDataSource(Source.Complete, UpdateFrequency.Daily) },
        { Source.ExploreEducationStatistics, GetDummyDataSource(Source.Cdm, UpdateFrequency.Annually) },
        { Source.Gias, GetDummyDataSource(Source.Gias, UpdateFrequency.Daily) },
        { Source.ManageFreeSchoolProjects, GetDummyDataSource(Source.ManageFreeSchoolProjects, UpdateFrequency.Daily) },
        { Source.Mis, GetDummyDataSource(Source.Mis, UpdateFrequency.Monthly) },
        { Source.MisFurtherEducation, GetDummyDataSource(Source.MisFurtherEducation, UpdateFrequency.Monthly) },
        { Source.Mstr, GetDummyDataSource(Source.Mstr, UpdateFrequency.Daily) },
        { Source.Prepare, GetDummyDataSource(Source.Prepare, UpdateFrequency.Daily) },
        { Source.CompareSchoolCollegePerformanceEngland, GetDummyDataSource(Source.CompareSchoolCollegePerformanceEngland, UpdateFrequency.Annually) }
    };

    private readonly DataSource _dummyInternalContactDataSource =
        new(Source.FiatDb, DateTime.Parse("2025-06-01"), null, "some.user@education.gov.uk");

    public DataSourceServiceTests()
    {
        _sut = new DataSourceService(_mockDataSourceRepository, _mockFiatDataSourceRepository,
            _mockFreeSchoolMealsAverageProvider, _mockMemoryCache.Object);

        Source[] supportedAcademiesDbSources =
        [
            Source.Cdm, Source.Complete, Source.Gias, Source.ManageFreeSchoolProjects, Source.Mis,
            Source.MisFurtherEducation, Source.Mstr,
            Source.Prepare,
            Source.CompareSchoolCollegePerformanceEngland,
        ];

        _mockDataSourceRepository.GetAsync(Arg.Is<Source>(source => supportedAcademiesDbSources.Contains(source)))
            .Returns(callInfo => _dummyDataSources[callInfo.Arg<Source>()]);

        _mockDataSourceRepository.GetAsync(Arg.Is<Source>(source => !supportedAcademiesDbSources.Contains(source)))
            .ThrowsAsync(new Exception("Source isn't in supportedAcademiesDbSources. If a test fails unexpectedly with this message, it probably should be added to the list."));

        _mockFreeSchoolMealsAverageProvider.GetFreeSchoolMealsUpdated()
            .Returns(_dummyDataSources[Source.ExploreEducationStatistics]);
    }

    private static DataSource GetDummyDataSource(Source source,
        UpdateFrequency updateFrequency)
    {
        return new DataSource(source, new DateTime(2024, 01, 01), updateFrequency);
    }

    [Fact]
    public async Task GetAsync_uncached_ExploreEducationStatistics_should_call_freeSchoolMealsAverageProvider()
    {
        var result = await _sut.GetAsync(Source.ExploreEducationStatistics);

        result.Should().BeEquivalentTo(_dummyDataSources[Source.ExploreEducationStatistics]);
        _mockFreeSchoolMealsAverageProvider.Received(1).GetFreeSchoolMealsUpdated();
    }

    [Theory]
    [InlineData(Source.Cdm, UpdateFrequency.Daily)]
    [InlineData(Source.ExploreEducationStatistics, UpdateFrequency.Annually)]
    [InlineData(Source.Gias, UpdateFrequency.Daily)]
    [InlineData(Source.Mis, UpdateFrequency.Monthly)]
    [InlineData(Source.MisFurtherEducation, UpdateFrequency.Monthly)]
    [InlineData(Source.Mstr, UpdateFrequency.Daily)]
    [InlineData(Source.CompareSchoolCollegePerformanceEngland, UpdateFrequency.Annually)]
    public async Task GetAsync_cached_should_return_cached_result(Source source, UpdateFrequency updateFrequency)
    {
        var dataSource = new DataSourceServiceModel(source, new DateTime(2024, 01, 01), updateFrequency);
        _mockMemoryCache.AddMockCacheEntry(source, dataSource);

        var result = await _sut.GetAsync(source);

        result.Should().BeEquivalentTo(dataSource);
    }

    [Theory]
    [InlineData(Source.Cdm, UpdateFrequency.Daily)]
    [InlineData(Source.Complete, UpdateFrequency.Daily)]
    [InlineData(Source.ExploreEducationStatistics, UpdateFrequency.Annually)]
    [InlineData(Source.Gias, UpdateFrequency.Daily)]
    [InlineData(Source.ManageFreeSchoolProjects, UpdateFrequency.Daily)]
    [InlineData(Source.Mis, UpdateFrequency.Monthly)]
    [InlineData(Source.MisFurtherEducation, UpdateFrequency.Monthly)]
    [InlineData(Source.Mstr, UpdateFrequency.Daily)]
    [InlineData(Source.Prepare, UpdateFrequency.Daily)]
    [InlineData(Source.CompareSchoolCollegePerformanceEngland, UpdateFrequency.Annually)]
    public async Task GetAsync_uncached_should_call_dataSourceRepository(Source source, UpdateFrequency updateFrequency)
    {
        var expectedCacheTimeSpan =
            updateFrequency is UpdateFrequency.Daily ? TimeSpan.FromHours(1) : TimeSpan.FromDays(1);

        await _sut.GetAsync(source);

        _mockMemoryCache.Object.Received(1).CreateEntry(source);

        var cachedEntry = _mockMemoryCache.MockCacheEntries[source];

        cachedEntry.Value.Should().BeEquivalentTo(_dummyDataSources[source]);
        cachedEntry.AbsoluteExpirationRelativeToNow.Should().Be(expectedCacheTimeSpan);
    }

    [Theory]
    [InlineData(Source.Cdm)]
    [InlineData(Source.Complete)]
    [InlineData(Source.Gias)]
    [InlineData(Source.ManageFreeSchoolProjects)]
    [InlineData(Source.Mis)]
    [InlineData(Source.MisFurtherEducation)]
    [InlineData(Source.Mstr)]
    [InlineData(Source.Prepare)]
    [InlineData(Source.CompareSchoolCollegePerformanceEngland)]
    public async Task GetAsync_uncached_should_call_academiesDbDataSourceRepository(Source source)
    {
        var result = await _sut.GetAsync(source);

        result.Should().BeEquivalentTo(_dummyDataSources[source]);
        await _mockDataSourceRepository.Received(1).GetAsync(source);
    }

    [Theory]
    [InlineData(Source.Cdm, UpdateFrequency.Daily)]
    [InlineData(Source.Complete, UpdateFrequency.Daily)]
    [InlineData(Source.ExploreEducationStatistics, UpdateFrequency.Annually)]
    [InlineData(Source.Gias, UpdateFrequency.Daily)]
    [InlineData(Source.ManageFreeSchoolProjects, UpdateFrequency.Daily)]
    [InlineData(Source.Mis, UpdateFrequency.Monthly)]
    [InlineData(Source.MisFurtherEducation, UpdateFrequency.Monthly)]
    [InlineData(Source.Mstr, UpdateFrequency.Daily)]
    [InlineData(Source.Prepare, UpdateFrequency.Daily)]
    [InlineData(Source.CompareSchoolCollegePerformanceEngland, UpdateFrequency.Annually)]
    public async Task GetAsync_uncached_should_cache_result(Source source, UpdateFrequency updateFrequency)
    {
        var expectedCacheTimeSpan =
            updateFrequency is UpdateFrequency.Daily ? TimeSpan.FromHours(1) : TimeSpan.FromDays(1);

        await _sut.GetAsync(source);

        _mockMemoryCache.Object.Received(1).CreateEntry(source);

        var cachedEntry = _mockMemoryCache.MockCacheEntries[source];

        cachedEntry.Value.Should().BeEquivalentTo(_dummyDataSources[source]);
        cachedEntry.AbsoluteExpirationRelativeToNow.Should().Be(expectedCacheTimeSpan);
    }

    [Fact]
    public async Task GetAsync_uncached_should_not_cache_source_with_null_lastUpdated()
    {
        _mockDataSourceRepository.GetAsync(Source.Gias).Returns(
            Task.FromResult(new DataSource(Source.Gias, null, UpdateFrequency.Daily)));

        await _sut.GetAsync(Source.Gias);

        _mockMemoryCache.Object.DidNotReceive().CreateEntry(Arg.Any<object>());
    }

    [Fact]
    public async Task GetAsync_should_throw_when_unknown_source()
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _sut.GetAsync((Source)999));
    }

    [Theory]
    [InlineData(SchoolContactRole.RegionsGroupLocalAuthorityLead)]
    public async Task GetSchoolContactDataSourceAsync_should_call_fiatDataSourceRepository(
        SchoolContactRole role)
    {
        _mockFiatDataSourceRepository.GetSchoolContactDataSourceAsync(123456, role)
            .Returns(_dummyInternalContactDataSource);

        var result = await _sut.GetSchoolContactDataSourceAsync(123456, role);

        using (new AssertionScope())
        {
            await _mockFiatDataSourceRepository.Received(1).GetSchoolContactDataSourceAsync(123456, role);
            result.Should().BeEquivalentTo(_dummyInternalContactDataSource);
        }
    }

    [Theory]
    [InlineData(TrustContactRole.TrustRelationshipManager)]
    [InlineData(TrustContactRole.SfsoLead)]
    public async Task GetTrustContactDataSourceAsync_should_call_fiatDataSourceRepository(
        TrustContactRole role)
    {
        _mockFiatDataSourceRepository.GetTrustContactDataSourceAsync(4321, role)
            .Returns(_dummyInternalContactDataSource);

        var result = await _sut.GetTrustContactDataSourceAsync(4321, role);

        using (new AssertionScope())
        {
            await _mockFiatDataSourceRepository.Received(1).GetTrustContactDataSourceAsync(4321, role);
            result.Should().BeEquivalentTo(_dummyInternalContactDataSource);
        }
    }
}
