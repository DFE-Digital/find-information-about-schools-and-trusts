using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Data.FiatDb.Repositories;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.Extensions.Logging;

namespace DfE.FindInformationAcademiesTrusts.Services.School;

public interface ISchoolContactsService
{
    Task<Person?> GetInSchoolContactsAsync(int urn);
    Task<SchoolInternalContactsServiceModel> GetInternalContactsAsync(int urn);

    Task<InternalContactUpdatedServiceModel> UpdateContactAsync(int urn, string? name, string? email,
        SchoolContactRole role);
}

public class SchoolContactsService(
    ITrustService trustService,
    ISchoolRepository schoolRepository,
    IContactRepository contactRepository,
    ILogger<SchoolContactsService> logger) : ISchoolContactsService
{
    public async Task<Person?> GetInSchoolContactsAsync(int urn)
    {
       try
       {
            var schoolContacts = await schoolRepository.GetSchoolContactsAsync(urn);

            if (schoolContacts is null)
            {
                return null;
            }

            var headteacher = new Person(schoolContacts.Name ?? string.Empty, schoolContacts.Email);

            return headteacher;
       }
       catch (Exception ex)
       {
            logger.LogError(ex, "Error getting in-school contacts for URN {urn}", urn);
            return null;
       }
    }

    public async Task<SchoolInternalContactsServiceModel> GetInternalContactsAsync(int urn)
    {
        try
        {
            var schoolInternalContacts = await contactRepository.GetSchoolInternalContactsAsync(urn);

            var trustSummary = await trustService.GetTrustSummaryAsync(urn);
            if (trustSummary is null)
            {
                return new SchoolInternalContactsServiceModel(schoolInternalContacts.RegionsGroupLocalAuthorityLead);
            }

            var trustContacts = await contactRepository.GetTrustInternalContactsAsync(trustSummary.Uid);

            return new SchoolInternalContactsServiceModel(schoolInternalContacts.RegionsGroupLocalAuthorityLead,
                trustContacts.TrustRelationshipManager,
                trustContacts.SfsoLead);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting internal contacts for URN {urn}", urn);
            return new SchoolInternalContactsServiceModel();
        }
    }

    public async Task<InternalContactUpdatedServiceModel> UpdateContactAsync(int urn, string? name, string? email,
        SchoolContactRole role)
    {
        var (emailChanged, nameChanged) =
            await contactRepository.UpdateSchoolInternalContactsAsync(urn, name, email, role);

        return new InternalContactUpdatedServiceModel(emailChanged, nameChanged);
    }
}
