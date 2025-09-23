using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Contexts;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Extensions;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Models.Gias;
using DfE.FindInformationAcademiesTrusts.Data.Repositories;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Trust;
using Microsoft.EntityFrameworkCore;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Repositories;

public class TrustRepository(
    IAcademiesDbContext academiesDbContext,
    IStringFormattingUtilities stringFormattingUtilities) : ITrustRepository
{
    private IQueryable<GiasGroup> Trusts { get; } = academiesDbContext.Groups.Trusts();

    public async Task<TrustSummary?> GetTrustSummaryAsync(string uid)
    {
        var details = await Trusts
            .Where(g => g.GroupUid == uid)
            .Select(g => new
                {
                    Name = g.GroupName!,
                    Type = g.GroupType!
                }
            ) //GroupName and GroupType will never be null due to EF query filters
            .SingleOrDefaultAsync();

        return details is null ? null : new TrustSummary(details.Name, details.Type);
    }

    public async Task<TrustOverview> GetTrustOverviewAsync(string uid)
    {
        var giasGroup = await Trusts
            .Where(g => g.GroupUid == uid)
            .Select(giasGroup => new
            {
                giasGroup.GroupUid,
                giasGroup.GroupId,
                giasGroup.Ukprn,
                giasGroup.CompaniesHouseNumber,
                giasGroup.GroupType,
                giasGroup.GroupContactStreet,
                giasGroup.GroupContactLocality,
                giasGroup.GroupContactTown,
                giasGroup.GroupContactPostcode,
                giasGroup.IncorporatedOnOpenDate
            })
            .SingleAsync();

        var regionAndTerritory = await GetRegionAndTerritoryAsync(uid);

        var trustOverview = new TrustOverview(
            giasGroup.GroupUid!, //Searched by this field so it must be present
            giasGroup.GroupId!, // GroupId cannot be null for a trust
            giasGroup.Ukprn,
            giasGroup.CompaniesHouseNumber,
            giasGroup.GroupType!, //Enforced by global EF filter
            stringFormattingUtilities.BuildAddressString(
                giasGroup.GroupContactStreet,
                giasGroup.GroupContactLocality,
                giasGroup.GroupContactTown,
                giasGroup.GroupContactPostcode
            ),
            regionAndTerritory,
            giasGroup.IncorporatedOnOpenDate.ParseAsNullableDate()
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


    private async Task<string> GetRegionAndTerritoryAsync(string uid)
    {
        return await academiesDbContext.MstrTrusts
            .Where(m => m.GroupUid == uid)
            .Select(m => m.GORregion)
            .SingleOrDefaultAsync() ?? string.Empty;
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
