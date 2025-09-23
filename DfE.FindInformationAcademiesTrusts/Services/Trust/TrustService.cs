using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Data.FiatDb.Repositories;
using DfE.FindInformationAcademiesTrusts.Data.Repositories;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Academy;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Trust;
using Microsoft.Extensions.Caching.Memory;

namespace DfE.FindInformationAcademiesTrusts.Services.Trust;

public interface ITrustService
{
    Task<TrustSummaryServiceModel?> GetTrustSummaryAsync(string uid);
    Task<TrustSummaryServiceModel?> GetTrustSummaryAsync(int urn);
    Task<TrustGovernanceServiceModel> GetTrustGovernanceAsync(string uid);
    Task<TrustContactsServiceModel> GetTrustContactsAsync(string uid);
    Task<TrustOverviewServiceModel> GetTrustOverviewAsync(string uid);

    Task<InternalContactUpdatedServiceModel> UpdateContactAsync(int uid, string? name, string? email,
        TrustContactRole role);

    Task<string> GetTrustReferenceNumberAsync(string uid);
}

public class TrustService(
    IAcademyRepository academyRepository,
    ITrustRepository trustRepository,
    ITrustGovernanceRepository trustGovernanceRepository,
    IContactRepository contactRepository,
    IMemoryCache memoryCache,
    IDateTimeProvider dateTimeProvider)
    : ITrustService
{
    public async Task<TrustSummaryServiceModel?> GetTrustSummaryAsync(int urn)
    {
        var uid = await academyRepository.GetTrustUidFromAcademyUrnAsync(urn);

        if (uid is null)
        {
            return null;
        }

        return await GetTrustSummaryAsync(uid);
    }

    public async Task<TrustSummaryServiceModel?> GetTrustSummaryAsync(string uid)
    {
        var cacheKey = $"{nameof(TrustService)}:{uid}";

        if (memoryCache.TryGetValue(cacheKey, out TrustSummaryServiceModel? cachedTrustSummary))
        {
            return cachedTrustSummary!;
        }

        var summary = await trustRepository.GetTrustSummaryAsync(uid);

        if (summary is null)
        {
            return null;
        }

        var count = await academyRepository.GetNumberOfAcademiesInTrustAsync(uid);

        var trustSummaryServiceModel = new TrustSummaryServiceModel(uid, summary.Name, summary.Type, count);

        memoryCache.Set(cacheKey, trustSummaryServiceModel,
            new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(10) });

        return trustSummaryServiceModel;
    }

    public async Task<TrustGovernanceServiceModel> GetTrustGovernanceAsync(string uid)
    {
        var urn = await academyRepository.GetSingleAcademyTrustAcademyUrnAsync(uid);

        var governors = await trustGovernanceRepository.GetTrustGovernanceAsync(urn ?? uid);

        return new TrustGovernanceServiceModel(
            governors.Where(g => g.IsCurrentOrFutureGovernor && g.HasRoleLeadership).ToArray(),
            governors.Where(g => g.IsCurrentOrFutureGovernor && g.HasRoleMember).ToArray(),
            governors.Where(g => g.IsCurrentOrFutureGovernor && g.HasRoleTrustee).ToArray(),
            governors.Where(g => !g.IsCurrentOrFutureGovernor).ToArray(),
            GetGovernanceTurnoverRate(governors));
    }

    public async Task<TrustContactsServiceModel> GetTrustContactsAsync(string uid)
    {
        var urn = await academyRepository.GetSingleAcademyTrustAcademyUrnAsync(uid);

        var trustContacts =
            await trustRepository.GetTrustContactsAsync(uid, urn);
        var internalContacts = await contactRepository.GetTrustInternalContactsAsync(uid);

        return new TrustContactsServiceModel(
            internalContacts.TrustRelationshipManager,
            internalContacts.SfsoLead,
            ChairOfTrustees: trustContacts.ChairOfTrustees,
            AccountingOfficer: trustContacts.AccountingOfficer,
            ChiefFinancialOfficer: trustContacts.ChiefFinancialOfficer
        );
    }

    public async Task<InternalContactUpdatedServiceModel> UpdateContactAsync(int uid, string? name, string? email,
        TrustContactRole role)
    {
        var (emailChanged, nameChanged) =
            await contactRepository.UpdateTrustInternalContactsAsync(uid, name, email, role);

        return new InternalContactUpdatedServiceModel(emailChanged, nameChanged);
    }

    public async Task<TrustOverviewServiceModel> GetTrustOverviewAsync(string uid)
    {
        var trustOverview = await trustRepository.GetTrustOverviewAsync(uid);
        var trustType = trustOverview.Type switch
        {
            "Single-academy trust" => TrustType.SingleAcademyTrust,
            "Multi-academy trust" => TrustType.MultiAcademyTrust,
            _ => throw new InvalidOperationException($"Unknown trust type: {trustOverview.Type}")
        };

        var singleAcademyTrustAcademyUrn = trustType is TrustType.SingleAcademyTrust
            ? await academyRepository.GetSingleAcademyTrustAcademyUrnAsync(uid)
            : null;

        var academiesOverview = await academyRepository.GetOverviewOfAcademiesInTrustAsync(uid);

        var totalAcademies = academiesOverview.Length;

        var academiesByLocalAuthority = academiesOverview
            .GroupBy(a => a.LocalAuthority)
            .ToDictionary(g => g.Key, g => g.Count());

        var totalPupilNumbers = academiesOverview.Sum(a => a.NumberOfPupils ?? 0);
        var totalCapacity = academiesOverview.Sum(a => a.SchoolCapacity ?? 0);

        var overviewModel = new TrustOverviewServiceModel(
            trustOverview.Uid,
            trustOverview.GroupId,
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
            totalCapacity
        );

        return overviewModel;
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

    public async Task<string> GetTrustReferenceNumberAsync(string uid)
    {
        return await trustRepository.GetTrustReferenceNumberAsync(uid);
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
