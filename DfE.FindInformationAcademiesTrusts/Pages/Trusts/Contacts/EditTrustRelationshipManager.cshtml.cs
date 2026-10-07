using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Domain.Enums;
using DfE.FindInformationAcademiesTrusts.Services.Trust;

namespace DfE.FindInformationAcademiesTrusts.Pages.Trusts.Contacts;

public class EditTrustRelationshipManagerModel(ITrustService trustService)
    : EditTrustContactFormModel(trustService, TrustContactRole.TrustRelationshipManager)
{
    protected override InternalContact? GetContactFromServiceModel(TrustContactsServiceModel contacts)
    {
        return contacts.TrustRelationshipManager;
    }
}
