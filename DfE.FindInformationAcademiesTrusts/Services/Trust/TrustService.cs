using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Domain.Enums;
using DfE.FindInformationAcademiesTrusts.Data.Repositories;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Academy;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Trust;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Trusts;
using Microsoft.Extensions.Caching.Memory;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.School;

namespace DfE.FindInformationAcademiesTrusts.Services.Trust;

public interface ITrustService
{
    Task<TrustSummaryServiceModel?> GetTrustSummaryAsync(string referenceNumber);
    Task<TrustSummaryServiceModel?> GetTrustSummaryAsync(int urn);
    Task<TrustContacts> GetTrustContactsAsync(string uid, string referenceNumber);
    Task<TrustGovernanceServiceModel?> GetTrustGovernanceAsync(string trn);
    Task<TrustOverviewServiceModel> GetTrustOverviewAsync(string trustReferenceNumber, string uid);
}

public class TrustService(
    IAcademyRepository academyRepository,
    ITrustRepository trustRepository,
    ITrustGovernanceProvider trustGovernanceProvider,
    ITrustPupilService trustPupilService,
    IMemoryCache memoryCache,
    IDateTimeProvider dateTimeProvider,
    ITrustOverviewProvider trustOverviewProvider)
    : ITrustService
{
    public async Task<TrustSummaryServiceModel?> GetTrustSummaryAsync(int urn)
    {
        var details = await trustOverviewProvider.GetEstablishmentTrust(urn);

        if (details is null)
        {
            return null;
        }

        return await GetOrCreateCachedTrustSummaryAsync(details.ReferenceNumber, details);
    }

    public async Task<TrustSummaryServiceModel?> GetTrustSummaryAsync(string referenceNumber)
    {
        return await GetOrCreateCachedTrustSummaryAsync(referenceNumber);
    }

    private async Task<TrustSummaryServiceModel?> GetOrCreateCachedTrustSummaryAsync(
        string referenceNumber,
        TrustDto? details = null)
    {
        var cacheKey = $"{nameof(TrustService)}:{referenceNumber}";

        if (memoryCache.TryGetValue(cacheKey, out TrustSummaryServiceModel? cachedTrustSummary))
        {
            return cachedTrustSummary!;
        }

        details ??= await trustOverviewProvider.GetTrustByReferenceNumber(referenceNumber);

        if (details is null)
        {
            return null;
        }

        var count = await academyRepository.GetNumberOfAcademiesInTrustAsync(details.ReferenceNumber);

        var trustSummaryServiceModel = new TrustSummaryServiceModel(details.GroupUid ?? string.Empty, details.ReferenceNumber,
            details.Name, details.Type.Name, count);

        memoryCache.Set(cacheKey, trustSummaryServiceModel,
            new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(10) });

        return trustSummaryServiceModel;
    }

    public async Task<TrustGovernanceServiceModel?> GetTrustGovernanceAsync(string trn)
    {
        var (_, trustType, singleAcademyTrustAcademyUrn) = await GetTrustOverviewWithTypeAsync(trn);

        if (trustType == TrustType.SingleAcademyTrust && singleAcademyTrustAcademyUrn is null)
        {
            return null;
        }

        var governors = trustType is TrustType.MultiAcademyTrust
            ? await trustGovernanceProvider.GetMultiAcademyTrustGovernorsAsync(trn.ToUpper())
            : await trustGovernanceProvider.GetSingleAcademyTrustGovernorsAsync(int.Parse(singleAcademyTrustAcademyUrn!));

        return new TrustGovernanceServiceModel(
            governors.Where(g => g is { IsCurrentOrFutureGovernor: true, HasRoleLeadership: true }).ToArray(),
            governors.Where(g => g is { IsCurrentOrFutureGovernor: true, HasRoleMember: true }).ToArray(),
            governors.Where(g => g is { IsCurrentOrFutureGovernor: true, HasRoleTrustee: true }).ToArray(),
            governors.Where(g => !g.IsCurrentOrFutureGovernor).ToArray(),
            GetGovernanceTurnoverRate(governors));
    }

    public async Task<TrustContacts> GetTrustContactsAsync(string uid, string referenceNumber)
    {
        var (_, _, singleAcademyTrustAcademyUrn) = await GetTrustOverviewWithTypeAsync(referenceNumber);

        return await trustRepository.GetTrustContactsAsync(uid, singleAcademyTrustAcademyUrn);
    }

    public async Task<TrustOverviewServiceModel> GetTrustOverviewAsync(string trustReferenceNumber, string uid)
    {
        var (trustOverview, trustType, singleAcademyTrustAcademyUrn) =
            await GetTrustOverviewWithTypeAsync(trustReferenceNumber);

        var academiesOverview = await academyRepository.GetOverviewOfAcademiesInTrustAsync(trustReferenceNumber);

        var totalAcademies = academiesOverview.Length;

        var academiesByLocalAuthority = academiesOverview
            .GroupBy(a => a.LocalAuthority)
            .ToDictionary(g => g.Key, g => g.Count());

        var totalPupilNumbers = await trustPupilService.GetTotalPupilCountForTrustAsync(trustReferenceNumber);
        var totalCapacity = academiesOverview.Sum(a => a.SchoolCapacity ?? 0);

        var hasIncompleteCapacityData = academiesOverview.Any(a => a.SchoolCapacity is null);

        var overviewModel = new TrustOverviewServiceModel(
            trustOverview.Uid,
            trustOverview.TrustReferenceNumber,
            trustOverview.Ukprn,
            trustOverview.CompaniesHouseNumber,
            trustType,
            trustOverview.Address,
            trustOverview.RegionAndTerritory,
            singleAcademyTrustAcademyUrn,
            trustOverview.OpenedDate,
            totalAcademies,
            academiesByLocalAuthority,
            totalPupilNumbers,
            totalCapacity,
            hasIncompleteCapacityData
        );

        return overviewModel;
    }

    private async Task<(TrustOverview Overview, TrustType Type, string? SingleAcademyTrustAcademyUrn)>
        GetTrustOverviewWithTypeAsync(string trustReferenceNumber)
    {
        var trustOverview = await trustOverviewProvider.GetTrustOverviewAsync(trustReferenceNumber);

        var trustType = trustOverview.Type switch
        {
            "Single-academy trust" => TrustType.SingleAcademyTrust,
            "Multi-academy trust" => TrustType.MultiAcademyTrust,
            _ => throw new InvalidOperationException($"Unknown trust type: {trustOverview.Type}")
        };

        var singleAcademyTrustAcademyUrn = trustType is TrustType.SingleAcademyTrust
            ? await academyRepository.GetSingleAcademyTrustAcademyUrnAsync(trustReferenceNumber)
            : null;

        return (trustOverview, trustType, singleAcademyTrustAcademyUrn);
    }

    public decimal GetGovernanceTurnoverRate(List<Governor> governors)
    {
        var to = dateTimeProvider.Today;
        var from = dateTimeProvider.Today.AddYears(-1);

        var governorsEligibleForTurnoverCalculation = GetGovernorsEligibleForTurnoverCalculation(governors);

        var currentGovernors = governorsEligibleForTurnoverCalculation
            .Where(g => g.DateOfTermEnd is null || g.DateOfTermEnd > to)
            .ToList();

        decimal totalGovernors = currentGovernors.Count;

        if (totalGovernors == 0)
        {
            return 0m;
        }

        var resignations = CountEventsWithinDateRange(
            governorsEligibleForTurnoverCalculation,
            g => g.DateOfTermEnd,
            from,
            to
        );
        var appointments = CountEventsWithinDateRange(
            governorsEligibleForTurnoverCalculation,
            g => g.DateOfAppointment,
            from,
            to
        );

        decimal totalEvents = resignations + appointments;

        return Math.Round(100m * totalEvents / totalGovernors, 1);
    }

    private static List<Governor> GetGovernorsEligibleForTurnoverCalculation(List<Governor> governors)
    {
        var result = governors.Where(g => g.HasRoleTrustee || g.HasRoleChairOfTrustees).ToList();

        // To avoid double counting, Chairs of Trustees should be removed when they are also listed as Trustees.
        // They shouldn't be removed when they are only listed as the Chair of Trustees, or if they are listed as some
        // other non-trustee role.
        var chairsToRemove = result
            .Where(g => g.HasRoleChairOfTrustees)
            .Where(chair => result.Exists(g => g.FullName == chair.FullName && g.HasRoleTrustee))
            .ToList();
        result.RemoveAll(g => chairsToRemove.Contains(g));

        return result;
    }

    private static int CountEventsWithinDateRange<T>(
        IEnumerable<T> items,
        Func<T, DateTime?> dateSelector,
        DateTime rangeStart,
        DateTime rangeEnd)
    {
        return items.Count(item => dateSelector(item) != null &&
                                   dateSelector(item) >= rangeStart &&
                                   dateSelector(item) <= rangeEnd);
    }

}
