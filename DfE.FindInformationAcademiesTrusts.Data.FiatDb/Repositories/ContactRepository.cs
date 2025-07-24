using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Data.FiatDb.Contexts;
using DfE.FindInformationAcademiesTrusts.Data.FiatDb.Models;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Contacts;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Trust;
using Microsoft.EntityFrameworkCore;

namespace DfE.FindInformationAcademiesTrusts.Data.FiatDb.Repositories;

public interface IContactRepository
{
    Task<TrustInternalContacts> GetTrustInternalContactsAsync(string uid);

    Task<InternalContactUpdated> UpdateTrustInternalContactsAsync(int uid, string? name, string? email,
        TrustContactRole role);

    Task<SchoolInternalContacts> GetSchoolInternalContactsAsync(int urn);

    Task<InternalContactUpdated> UpdateSchoolInternalContactsAsync(int urn, string? name, string? email,
        SchoolContactRole role);
}

public class ContactRepository(FiatDbContext fiatDbContext) : IContactRepository
{
    public async Task<TrustInternalContacts> GetTrustInternalContactsAsync(string uid)
    {
        var trm = await GetTrustRelationshipManagerLinkedTo(uid);
        var sfso = await GetSfsoLeadLinkedTo(uid);

        return new TrustInternalContacts(
            trm,
            sfso);
    }

    public async Task<InternalContactUpdated> UpdateTrustInternalContactsAsync(int uid, string? name, string? email,
        TrustContactRole role)
    {
        var contact = await fiatDbContext.TrustContacts
            .SingleOrDefaultAsync(contact => contact.Uid == uid && contact.Role == role);
        if (contact is null)
        {
            return await AddNewContact(uid, name, email, role);
        }

        var nameUpdated = false;
        var emailUpdated = false;
        if (contact.Name != name)
        {
            nameUpdated = true;
            contact.Name = name ?? string.Empty;
        }

        if (contact.Email != email)
        {
            emailUpdated = true;
            contact.Email = email ?? string.Empty;
        }

        await fiatDbContext.SaveChangesAsync();
        return new InternalContactUpdated(emailUpdated, nameUpdated);
    }

    public async Task<SchoolInternalContacts> GetSchoolInternalContactsAsync(int urn)
    {
        var regionsGroupLocalAuthorityLead = await GetRegionsGroupLocalAuthorityLead(urn);
        return new SchoolInternalContacts(regionsGroupLocalAuthorityLead);
    }

    public async Task<InternalContactUpdated> UpdateSchoolInternalContactsAsync(int urn, string? name, string? email,
        SchoolContactRole role)
    {
        var contact = await fiatDbContext.SchoolContacts
            .SingleOrDefaultAsync(contact => contact.Urn == urn && contact.Role == role);
        if (contact is null)
        {
            return await AddNewContact(urn, name, email, role);
        }

        var nameUpdated = false;
        var emailUpdated = false;
        if (contact.Name != name)
        {
            nameUpdated = true;
            contact.Name = name ?? string.Empty;
        }

        if (contact.Email != email)
        {
            emailUpdated = true;
            contact.Email = email ?? string.Empty;
        }

        await fiatDbContext.SaveChangesAsync();
        return new InternalContactUpdated(emailUpdated, nameUpdated);
    }

    private async Task<InternalContactUpdated> AddNewContact(int uid, string? name, string? email,
        TrustContactRole role)
    {
        fiatDbContext.TrustContacts.Add(new TrustContact
        {
            Name = name ?? string.Empty,
            Email = email ?? string.Empty,
            Role = role,
            Uid = uid
        });
        await fiatDbContext.SaveChangesAsync();
        return new InternalContactUpdated(true, true);
    }

    private async Task<InternalContactUpdated> AddNewContact(int urn, string? name, string? email,
        SchoolContactRole role)
    {
        fiatDbContext.SchoolContacts.Add(new SchoolContact
        {
            Name = name ?? string.Empty,
            Email = email ?? string.Empty,
            Role = role,
            Urn = urn
        });
        await fiatDbContext.SaveChangesAsync();
        return new InternalContactUpdated(true, true);
    }

    private async Task<InternalContact?> GetTrustRelationshipManagerLinkedTo(string uid)
    {
        return await fiatDbContext.TrustContacts.Where(contact =>
                contact.Uid == int.Parse(uid) && contact.Role == TrustContactRole.TrustRelationshipManager)
            .Select(contact => new InternalContact(contact.Name, contact.Email,
                contact.LastModifiedAtTime, contact.LastModifiedByEmail
            )).SingleOrDefaultAsync();
    }

    private async Task<InternalContact?> GetSfsoLeadLinkedTo(string uid)
    {
        return await fiatDbContext.TrustContacts.Where(contact =>
                contact.Uid == int.Parse(uid) && contact.Role == TrustContactRole.SfsoLead)
            .Select(contact => new InternalContact(contact.Name, contact.Email,
                contact.LastModifiedAtTime, contact.LastModifiedByEmail
            )).SingleOrDefaultAsync();
    }

    private async Task<InternalContact?> GetRegionsGroupLocalAuthorityLead(int urn)
    {
        return await fiatDbContext.SchoolContacts
            .Where(contact => contact.Urn == urn && contact.Role == SchoolContactRole.RegionsGroupLocalAuthorityLead)
            .Select(contact => new InternalContact(contact.Name, contact.Email, contact.LastModifiedAtTime,
                contact.LastModifiedByEmail))
            .SingleOrDefaultAsync();
    }
}
