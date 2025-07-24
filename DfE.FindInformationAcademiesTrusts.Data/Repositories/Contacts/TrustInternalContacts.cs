namespace DfE.FindInformationAcademiesTrusts.Data.Repositories.Contacts;

public record TrustInternalContacts(
    InternalContact? TrustRelationshipManager,
    InternalContact? SfsoLead
);
