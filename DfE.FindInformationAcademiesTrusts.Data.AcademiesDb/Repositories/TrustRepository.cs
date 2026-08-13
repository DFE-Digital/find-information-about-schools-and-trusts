using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.AcademiesDbServices;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Contexts;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Extensions;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Models.Gias;
using DfE.FindInformationAcademiesTrusts.Data.Repositories;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.AcademiesDbServices;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Trust;
using Microsoft.EntityFrameworkCore;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Repositories;

public class TrustRepository(
    IAcademiesDbContext academiesDbContext,
    IGetTrusts getTrusts,
    IStringFormattingUtilities stringFormattingUtilities) : ITrustRepository
{
    private IQueryable<GiasGroup> Trusts { get; } = academiesDbContext.Groups.Trusts();

    public async Task<TrustSummary?> GetTrustSummaryAsync(string referenceNumber)
    {
        var details = await getTrusts.GetTrustByReferenceNumber(referenceNumber);
        return details is null
            ? null
            : new TrustSummary(details.Name ?? string.Empty, details.Type.Name ?? string.Empty,details.GroupUid ?? string.Empty,details.ReferenceNumber?.ToString() ?? string.Empty);
    }
    
    public async Task<TrustSummary?> GetTrustSummaryByEstablishmentUrnAsync(int urn)
    {
        var details = await getTrusts.GetEstablishmentTrust(urn);
        return details is null
            ? null
            : new TrustSummary(details.Name ?? string.Empty, details.Type.Name ?? string.Empty,details.GroupUid ?? string.Empty,details.ReferenceNumber?.ToString() ?? string.Empty);

    }

    public async Task<TrustOverview> GetTrustOverviewAsync(string trustReferenceNumber)
    {
        var result = await getTrusts.GetTrustByReferenceNumber(trustReferenceNumber) ?? throw new InvalidOperationException(
        $"Trust with reference number {trustReferenceNumber} was not found.");

        var trustOverview = new TrustOverview(
            result.GroupUid!,
            trustReferenceNumber,
            result.Ukprn,
            result.CompaniesHouseNumber,
            result.Type.Name,
            stringFormattingUtilities.BuildAddressString(
                result.Address.Street,
                result.Address.Locality,
                result.Address.Town,
                result.Address.Postcode
            ),
            result.Gor,
            result.OpenDate.ParseAsNullableDate()
        );

        return trustOverview;
    }

    public static IQueryable<GiasGovernance> FilterBySatOrMat(string uid, string? urn, IQueryable<GiasGovernance> query)
    {
        if (!string.IsNullOrEmpty(urn))
        {
            // Use urn if it's provided as that means this is a Single Academy Trust (SAT)
            return query.Where(g => g.Urn == urn);
        }

        return query.Where(g => g.Uid == uid);
    }

    public async Task<TrustContacts> GetTrustContactsAsync(string uid, string? urn = null)
    {
        var governanceContacts = await GetGovernanceContactsAsync(uid, urn);

        return new TrustContacts(
            governanceContacts.GetValueOrDefault("Accounting Officer"),
            governanceContacts.GetValueOrDefault("Chair of Trustees"),
            governanceContacts.GetValueOrDefault("Chief Financial Officer"));
    }

    private async Task<Dictionary<string, Person>> GetGovernanceContactsAsync(string uid, string? urn = null)
    {
        string[] roles = { "Chair of Trustees", "Accounting Officer", "Chief Financial Officer" };

        IQueryable<GiasGovernance> query = academiesDbContext.GiasGovernances;

        query = FilterBySatOrMat(uid, urn, query);

        var governors = (await query
                .Where(governance => roles.Contains(governance.Role))
                .Select(governance => new
                {
                    governance.Gid,
                    FullName = stringFormattingUtilities.GetFullName(governance.Forename1!, governance.Forename2!,
                        governance.Surname!),
                    EndDate = governance.DateTermOfOfficeEndsEnded.ParseAsNullableDate(),
                    StartDate = governance.DateOfAppointment.ParseAsNullableDate() ?? DateTime.MinValue,
                    Role = governance.Role!
                })
                .ToArrayAsync())
            .Where(g => (g.EndDate == null || g.EndDate >= DateTime.Today) && g.StartDate <= DateTime.Today).ToArray();

        var gids = governors.Select(g => g.Gid).ToArray();

        var governorEmails = await academiesDbContext.TadTrustGovernances
            .Where(tadTrustGovernance => gids.Contains(tadTrustGovernance.Gid))
            .Select(tadTrustGovernance => new { tadTrustGovernance.Gid, tadTrustGovernance.Email }).ToArrayAsync();

        return governors.ToDictionary(
            governor => governor.Role,
            governor => new Person(
                governor.FullName,
                governorEmails.SingleOrDefault(governorEmail => governorEmail.Gid == governor.Gid)?.Email)
        );
    }

    public async Task<string> GetTrustReferenceNumberAsync(string uid)
    {
        var trustReferenceNumber = await Trusts
            .Where(gl => gl.GroupUid == uid)
            .Select(gl => gl.GroupId!) // GroupId cannot be null for a trust
            .SingleAsync();

        return trustReferenceNumber;
    }
}
