using DfE.FindInformationAcademiesTrusts.Data;

namespace DfE.FindInformationAcademiesTrusts.Services.School;

public record SchoolInternalContactsServiceModel(
    Person? RegionsGroupLocalAuthorityLead = null,
    Person? TrustRelationshipManager = null,
    Person? SfsoLead = null);
