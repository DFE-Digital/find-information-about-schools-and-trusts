using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Data.FiatDb.Repositories;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;

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
    IContactRepository contactRepository) : ISchoolContactsService
{
    public async Task<Person?> GetInSchoolContactsAsync(int urn)
    {
        var schoolContacts = await schoolRepository.GetSchoolContactsAsync(urn);

        if (schoolContacts is null)
        {
            return null;
        }

        var headteacher = new Person(schoolContacts.Name ?? string.Empty, schoolContacts.Email);

        return headteacher;
    }

    public async Task<SchoolInternalContactsServiceModel> GetInternalContactsAsync(int urn)
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

    public async Task<InternalContactUpdatedServiceModel> UpdateContactAsync(int urn, string? name, string? email,
        SchoolContactRole role)
    {
        var (emailChanged, nameChanged) =
            await contactRepository.UpdateSchoolInternalContactsAsync(urn, name, email, role);

        return new InternalContactUpdatedServiceModel(emailChanged, nameChanged);
    }
}
