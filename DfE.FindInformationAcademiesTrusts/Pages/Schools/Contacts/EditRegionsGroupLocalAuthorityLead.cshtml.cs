using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Services.School;

namespace DfE.FindInformationAcademiesTrusts.Pages.Schools.Contacts;

public class EditRegionsGroupLocalAuthorityLeadModel(
    ISchoolService schoolService,
    ISchoolContactsService schoolContactsService) : EditSchoolContactFormModel(schoolService, schoolContactsService,
    SchoolContactRole.RegionsGroupLocalAuthorityLead)
{
    protected override InternalContact? GetContactFromServiceModel(SchoolInternalContactsServiceModel contacts)
    {
        return contacts.RegionsGroupLocalAuthorityLead as InternalContact;
    }
}
