using DfE.FindInformationAcademiesTrusts.Data.Repositories.School;
using Microsoft.Extensions.Caching.Memory;

namespace DfE.FindInformationAcademiesTrusts.Services.School;

public interface ISchoolService
{
    Task<SchoolSummaryServiceModel?> GetSchoolSummaryAsync(int urn);

    Task<bool> IsPartOfFederationAsync(int urn);

    Task<SchoolReferenceNumbersServiceModel> GetReferenceNumbersAsync(int urn);

    Task<SchoolGovernanceServiceModel> GetSchoolGovernanceAsync(int urn);
}

public class SchoolService(IMemoryCache memoryCache, ISchoolRepository schoolRepository) : ISchoolService
{
    public async Task<bool> IsPartOfFederationAsync(int urn)
    {
        return await schoolRepository.IsPartOfFederationAsync(urn);
    }

    public async Task<SchoolSummaryServiceModel?> GetSchoolSummaryAsync(int urn)
    {
        var cacheKey = $"{nameof(GetSchoolSummaryAsync)}:{urn}";

        if (memoryCache.TryGetValue(cacheKey, out SchoolSummaryServiceModel? cachedTrustSummary))
        {
            return cachedTrustSummary;
        }

        var summary = await schoolRepository.GetSchoolSummaryAsync(urn);

        if (summary is null)
        {
            return null;
        }

        var schoolSummaryServiceModel =
            new SchoolSummaryServiceModel(urn, summary.Name, summary.Type, summary.Category);

        memoryCache.Set(cacheKey, schoolSummaryServiceModel,
            new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(10) });

        return schoolSummaryServiceModel;
    }

    public async Task<SchoolReferenceNumbersServiceModel> GetReferenceNumbersAsync(int urn)
    {
        var referenceNumbers = await schoolRepository.GetReferenceNumbersAsync(urn);

        if (referenceNumbers is null) return new SchoolReferenceNumbersServiceModel(urn);

        var laestab = referenceNumbers.LaCode is not null && referenceNumbers.EstablishmentNumber is not null
            ? $"{referenceNumbers.LaCode}/{referenceNumbers.EstablishmentNumber}"
            : null;

        return new SchoolReferenceNumbersServiceModel(urn, laestab, referenceNumbers.Ukprn);
    }

    public async Task<SchoolGovernanceServiceModel> GetSchoolGovernanceAsync(int urn)
    {
        var governance = await schoolRepository.GetGovernanceAsync(urn);

        return new SchoolGovernanceServiceModel(
            governance.Where(x => x.IsCurrentOrFutureGovernor).ToArray(),
            governance.Where(x => !x.IsCurrentOrFutureGovernor).ToArray());
    }
}
