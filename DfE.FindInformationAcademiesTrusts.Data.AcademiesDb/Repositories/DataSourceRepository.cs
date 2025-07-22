using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Contexts;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.DataSource;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Repositories;

public class DataSourceRepository(
    IAcademiesDbContext academiesDbContext,
    ILogger<DataSourceRepository> logger)
    : IDataSourceRepository
{
    public async Task<DataSource> GetAsync(Source source)
    {
        return source switch
        {
            Source.Gias => await GetDataSourceFromApplicationEvents("GIAS_Daily", Source.Gias, UpdateFrequency.Daily),
            Source.Mstr => await GetDataSourceFromApplicationEvents("MSTR_Daily", Source.Mstr, UpdateFrequency.Daily),
            Source.Cdm => await GetDataSourceFromApplicationEvents("CDM_Daily", Source.Cdm, UpdateFrequency.Daily),
            Source.Mis or Source.MisFurtherEducation => await GetMisEstablishmentsUpdatedAsync(source),
            Source.Prepare
            or Source.Complete
            or Source.ManageFreeSchoolProjects => await GetDataSourceFromMstr(source),
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
        };
    }

    private async Task<DataSource> GetMisEstablishmentsUpdatedAsync(Source source)
    {
        var lastEntry = await academiesDbContext.ApplicationSettings
            .FirstOrDefaultAsync(e => e.Key == "ManagementInformationSchoolTableData CSV Filename");
        if (lastEntry?.Modified is null)
        {
            logger.LogError("Unable to find when ManagementInformationSchoolTableData was last modified");
            return new DataSource(source, null, UpdateFrequency.Monthly);
        }

        return new DataSource(source, lastEntry.Modified.Value, UpdateFrequency.Monthly);
    }

    /// <summary>
    /// Retrieve MSTR data source information by switching on the Source value.
    /// </summary>
    private async Task<DataSource> GetDataSourceFromMstr(Source source)
    {
        // Default to Daily, but adjust if needed
        var updateFrequency = UpdateFrequency.Daily;
        DateTime? lastDataRefresh = null;

        lastDataRefresh = source switch
        {
            Source.Prepare => await academiesDbContext.MstrAcademyTransfers
                                .Where(x=> x.InPrepare.HasValue)
                                .Where(t => t.InPrepare!.Value)
                                .Select(t => t.LastDataRefresh)
                                .MaxAsync(),
            Source.Complete => await academiesDbContext.MstrAcademyTransfers
                                .Where(x => x.InComplete.HasValue)
                                .Where(t => t.InComplete!.Value)
                                .Select(t => t.LastDataRefresh)
                                .MaxAsync(),
            Source.ManageFreeSchoolProjects => await academiesDbContext.MstrFreeSchoolProjects
                                .Select(p => p.LastDataRefresh)
                                .MaxAsync(),
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null),
        };
        if (lastDataRefresh is null)
        {
            logger.LogError("Unable to find last data refresh for MSTR source '{source}'", source);
        }

        return new DataSource(source, lastDataRefresh, updateFrequency);
    }

    private async Task<DataSource> GetDataSourceFromApplicationEvents(string pipelineName, Source source,
        UpdateFrequency updateFrequency)
    {
        var lastEntry = await academiesDbContext.ApplicationEvents
            .Where(e => e.Message == "Finished"
                        && e.EventType != 'E'
                        && e.Description == pipelineName).MaxAsync(e => e.DateTime);
        if (lastEntry is null)
        {
            logger.LogError("Unable to find when {pipelineName} was last run", pipelineName);
            return new DataSource(source, null, updateFrequency);
        }

        return new DataSource(source, lastEntry.Value, updateFrequency);
    }
}
