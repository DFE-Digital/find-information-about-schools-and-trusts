using System.Runtime.CompilerServices;
using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Data.FiatDb.Repositories;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.DataSource;
using Microsoft.Extensions.Caching.Memory;

namespace DfE.FindInformationAcademiesTrusts.Services.DataSource;

public interface IDataSourceService
{
    Task<DataSourceServiceModel> GetAsync(Source source);
    Task<DataSourceServiceModel> GetSchoolContactDataSourceAsync(int urn, SchoolContactRole role);
    Task<DataSourceServiceModel> GetTrustContactDataSourceAsync(int uid, TrustContactRole role);
}

public class DataSourceService(
    IDataSourceRepository dataSourceRepository,
    IFiatDataSourceRepository fiatDataSourceRepository,
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
            Source.Gias or Source.Mstr or Source.Cdm or Source.Mis => await dataSourceRepository.GetAsync(source),
            Source.ExploreEducationStatistics => freeSchoolMealsAverageProvider.GetFreeSchoolMealsUpdated(),
            Source.Prepare or Source.Complete or Source.ManageFreeSchoolProjects =>
                await dataSourceRepository.GetAsync(source),
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

    public async Task<DataSourceServiceModel> GetSchoolContactDataSourceAsync(int urn, SchoolContactRole role)
    {
        var dataSource = await fiatDataSourceRepository.GetSchoolContactDataSourceAsync(urn, role);
        var dataSourceServiceModel = new DataSourceServiceModel(dataSource.Source, dataSource.LastUpdated,
            dataSource.NextUpdated, dataSource.UpdatedBy);

        return dataSourceServiceModel;
    }

    public async Task<DataSourceServiceModel> GetTrustContactDataSourceAsync(int uid, TrustContactRole role)
    {
        var dataSource = await fiatDataSourceRepository.GetTrustContactDataSourceAsync(uid, role);
        var dataSourceServiceModel = new DataSourceServiceModel(dataSource.Source, dataSource.LastUpdated,
            dataSource.NextUpdated, dataSource.UpdatedBy);

        return dataSourceServiceModel;
    }
}
