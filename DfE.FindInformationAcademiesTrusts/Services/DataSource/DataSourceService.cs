using System.Runtime.CompilerServices;
using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.DataSource;
using Microsoft.Extensions.Caching.Memory;

namespace DfE.FindInformationAcademiesTrusts.Services.DataSource;

public interface IDataSourceService
{
    Task<DataSourceServiceModel> GetAsync(Source source);
}

public class DataSourceService(
    IDataSourceRepository dataSourceRepository,
    IFreeSchoolMealsAverageProvider freeSchoolMealsAverageProvider,
    IMemoryCache memoryCache) : IDataSourceService
{
    public async Task<DataSourceServiceModel> GetAsync(Source source)
    {
        if (memoryCache.TryGetValue(source, out DataSourceServiceModel? cachedDataSource))
        {
            return cachedDataSource!;
        }

        var dataSource = source switch
        {
            Source.Gias
                or Source.Mstr
                or Source.Cdm
                or Source.Mis
                or Source.MisFurtherEducation
                or Source.Prepare
                or Source.Complete
                or Source.ManageFreeSchoolProjects
                or Source.CompareSchoolCollegePerformanceEnglandPopulation
                or Source.CompareSchoolCollegePerformanceEnglandAttendance =>
                    await dataSourceRepository.GetAsync(source),
            Source.ExploreEducationStatistics => freeSchoolMealsAverageProvider.GetFreeSchoolMealsUpdated(),
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
        };

        var dataSourceServiceModel =
            new DataSourceServiceModel(dataSource.Source, dataSource.LastUpdated, dataSource.NextUpdated);

        if (dataSourceServiceModel.LastUpdated is not null)
        {
            var cacheExpiration = dataSourceServiceModel.NextUpdated switch
            {
                UpdateFrequency.Daily => TimeSpan.FromHours(1),
                UpdateFrequency.Monthly or
                    UpdateFrequency.Annually => TimeSpan.FromDays(1),
                _ => throw new SwitchExpressionException(dataSourceServiceModel.NextUpdated)
            };

            memoryCache.Set(source, dataSourceServiceModel, cacheExpiration);
        }

        return dataSourceServiceModel;
    }
}
