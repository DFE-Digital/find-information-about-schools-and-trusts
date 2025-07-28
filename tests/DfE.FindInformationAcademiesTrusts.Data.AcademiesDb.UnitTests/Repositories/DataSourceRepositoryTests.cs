using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Repositories;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.DataSource;
using Microsoft.Extensions.Logging;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.UnitTests.Repositories;

public class DataSourceRepositoryTests
{
    private readonly DataSourceRepository _sut;
    private readonly MockAcademiesDbContext _mockAcademiesDbContext = new();
    private readonly ILogger<DataSourceRepository> _logger = MockLogger.CreateLogger<DataSourceRepository>();

    public DataSourceRepositoryTests()
    {
        _sut = new DataSourceRepository(_mockAcademiesDbContext.Object, _logger);

        var someDateTime = new DateTime(2020, 01, 01);
        _mockAcademiesDbContext.AddApplicationEvent("Unrelated event", someDateTime);
        _mockAcademiesDbContext.AddApplicationSetting("Unrelated setting", someDateTime);
    }

    [Fact]
    public async Task GetAsync_Throws_WhenUnsupportedDataSource()
    {
        var action = () => _sut.GetAsync(Source.ExploreEducationStatistics);

        await action.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(Source.Cdm, UpdateFrequency.Daily)]
    [InlineData(Source.Gias, UpdateFrequency.Daily)]
    [InlineData(Source.Mis, UpdateFrequency.Monthly)]
    [InlineData(Source.MisFurtherEducation, UpdateFrequency.Monthly)]
    [InlineData(Source.Mstr, UpdateFrequency.Daily)]
    [InlineData(Source.Prepare, UpdateFrequency.Daily)]
    [InlineData(Source.Complete, UpdateFrequency.Daily)]
    [InlineData(Source.ManageFreeSchoolProjects, UpdateFrequency.Daily)]
    public async Task GetAsync_WhenEntryExists_ShouldReturnLatestSuccessfullyFinishedDataSourceUpdate(Source source,
        UpdateFrequency updateFrequency)
    {
        var inProgressUpdateTime = new DateTime(2023, 12, 12, 06, 54, 12);
        var erroredUpdateTime = inProgressUpdateTime.AddHours(-1);
        var lastSuccessfulUpdateTime = inProgressUpdateTime.AddDays(-1);
        var oldSuccessfulUpdateTime = inProgressUpdateTime.AddDays(-2);

        AddSuccessfulDataSourceUpdates(lastSuccessfulUpdateTime);
        AddSuccessfulDataSourceUpdates(oldSuccessfulUpdateTime);
        AddInProgressDataSourceUpdates(inProgressUpdateTime);
        AddErroredDataSourceUpdates(erroredUpdateTime);

        var result = await _sut.GetAsync(source);

        result.Should().BeEquivalentTo(new DataSource(source, lastSuccessfulUpdateTime, updateFrequency));
    }

    [Theory]
    [InlineData(Source.Cdm, "Unable to find when CDM_Daily was last run", UpdateFrequency.Daily)]
    [InlineData(Source.Gias, "Unable to find when GIAS_Daily was last run", UpdateFrequency.Daily)]
    [InlineData(Source.Mis, "Unable to find when ManagementInformationSchoolTableData was last modified",
        UpdateFrequency.Monthly)]
    [InlineData(Source.MisFurtherEducation, "Unable to find when ManagementInformationSchoolTableData was last modified",
        UpdateFrequency.Monthly)]
    [InlineData(Source.Mstr, "Unable to find when MSTR_Daily was last run", UpdateFrequency.Daily)]
    [InlineData(Source.Prepare, "Unable to find last data refresh for MSTR source 'Prepare'", UpdateFrequency.Daily)]
    [InlineData(Source.Complete, "Unable to find last data refresh for MSTR source 'Complete'", UpdateFrequency.Daily)]
    [InlineData(Source.ManageFreeSchoolProjects,
        "Unable to find last data refresh for MSTR source 'ManageFreeSchoolProjects'", UpdateFrequency.Daily)]
    public async Task GetAsync_WhenNoEntryForPipelineExists_ShouldReturnDataSource_WithNullDate_AndLogError(
        Source source, string expectedErrorMessage, UpdateFrequency updateFrequency)
    {
        AddSuccessfulDataSourceUpdatesExceptFor(source);

        var result = await _sut.GetAsync(source);

        _logger.VerifyLogError(expectedErrorMessage);
        result.Should().BeEquivalentTo(new DataSource(source, null, updateFrequency));
    }

    private void AddInProgressDataSourceUpdates(DateTime updateTime)
    {
        _mockAcademiesDbContext.AddApplicationEvent("GIAS_Daily", updateTime, "Started");
        _mockAcademiesDbContext.AddApplicationEvent("MSTR_Daily", updateTime, "Started");
        _mockAcademiesDbContext.AddApplicationEvent("CDM_Daily", updateTime, "Started");
        //MIS does not have an in progress update status
    }

    private void AddErroredDataSourceUpdates(DateTime updateTime)
    {
        _mockAcademiesDbContext.AddApplicationEvent("GIAS_Daily", updateTime, eventType: 'E');
        _mockAcademiesDbContext.AddApplicationEvent("MSTR_Daily", updateTime, eventType: 'E');
        _mockAcademiesDbContext.AddApplicationEvent("CDM_Daily", updateTime, eventType: 'E');
        //MIS does not have an errored update status
        _mockAcademiesDbContext.AddMstrAcademyTransfer("", "", true, false, lastDataRefresh: null);
        _mockAcademiesDbContext.AddMstrAcademyTransfer("", "", false, true, lastDataRefresh: null);
        _mockAcademiesDbContext.AddMstrFreeSchoolProject("", "", lastDataRefresh: null);
    }

    private void AddSuccessfulDataSourceUpdates(DateTime updateTime)
    {
        _mockAcademiesDbContext.AddApplicationEvent("GIAS_Daily", updateTime);
        _mockAcademiesDbContext.AddApplicationEvent("MSTR_Daily", updateTime);
        _mockAcademiesDbContext.AddApplicationEvent("CDM_Daily", updateTime);
        _mockAcademiesDbContext.AddApplicationSetting("ManagementInformationSchoolTableData CSV Filename", updateTime);
        _mockAcademiesDbContext.AddMstrAcademyTransfer("", "", true, false, lastDataRefresh: updateTime);
        _mockAcademiesDbContext.AddMstrAcademyTransfer("", "", false, true, lastDataRefresh: updateTime);
        _mockAcademiesDbContext.AddMstrFreeSchoolProject("", "", lastDataRefresh: updateTime);
    }

    private void AddSuccessfulDataSourceUpdatesExceptFor(Source source)
    {
        var lastUpdateTime = new DateTime(2023, 12, 12, 06, 54, 12);

        if (source is not Source.Gias) _mockAcademiesDbContext.AddApplicationEvent("GIAS_Daily", lastUpdateTime);
        if (source is not Source.Mstr) _mockAcademiesDbContext.AddApplicationEvent("MSTR_Daily", lastUpdateTime);
        if (source is not Source.Cdm) _mockAcademiesDbContext.AddApplicationEvent("CDM_Daily", lastUpdateTime);
        if (source is not Source.Mis && source is not Source.MisFurtherEducation)
            _mockAcademiesDbContext.AddApplicationSetting("ManagementInformationSchoolTableData CSV Filename",
                lastUpdateTime);
        if (source is not Source.Prepare)
            _mockAcademiesDbContext.AddMstrAcademyTransfer("", "", true, false, lastDataRefresh: lastUpdateTime);
        if (source is not Source.Complete)
            _mockAcademiesDbContext.AddMstrAcademyTransfer("", "", false, true, lastDataRefresh: lastUpdateTime);
        if (source is not Source.ManageFreeSchoolProjects)
            _mockAcademiesDbContext.AddMstrFreeSchoolProject("", "", lastDataRefresh: lastUpdateTime);
    }
}
