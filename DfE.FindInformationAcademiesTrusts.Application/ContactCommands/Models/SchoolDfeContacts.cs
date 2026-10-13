using DfE.FindInformationAcademiesTrusts.Domain.Entities;

namespace DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Models;

public record SchoolDfeContacts(
    SchoolContact? RegionsGroupLocalAuthorityLead,
    TrustContact? TrustRelationshipManager,
    TrustContact? SfsoLead);
