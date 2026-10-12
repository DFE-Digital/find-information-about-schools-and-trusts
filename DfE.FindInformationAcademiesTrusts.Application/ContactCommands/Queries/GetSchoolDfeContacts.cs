using DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Models;
using DfE.FindInformationAcademiesTrusts.Domain.Enums;
using DfE.FindInformationAcademiesTrusts.Domain.Interfaces.Repositories;
using MediatR;

namespace DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Queries;

/// <summary>
/// DfE contacts shown for a school: its own regions group LA lead, plus its trust's contacts when it is in a trust
/// </summary>
public record GetSchoolDfeContactsQuery(
    int Urn,
    int? TrustUid
) : IRequest<SchoolDfeContacts>;

public class GetSchoolDfeContacts
{
    public class GetSchoolDfeContactsQueryHandler(
        ISchoolContactRepository schoolContactRepository,
        ITrustContactRepository trustContactRepository)
        : IRequestHandler<GetSchoolDfeContactsQuery, SchoolDfeContacts>
    {
        public async Task<SchoolDfeContacts> Handle(GetSchoolDfeContactsQuery request,
            CancellationToken cancellationToken)
        {
            var regionsGroupLocalAuthorityLead = await schoolContactRepository.GetSchoolContactAsync(request.Urn,
                SchoolContactRole.RegionsGroupLocalAuthorityLead, cancellationToken);

            if (request.TrustUid is not { } trustUid)
            {
                return new SchoolDfeContacts(regionsGroupLocalAuthorityLead, null, null);
            }

            var trustRelationshipManager = await trustContactRepository.GetTrustContactAsync(trustUid,
                TrustContactRole.TrustRelationshipManager, cancellationToken);
            var sfsoLead =
                await trustContactRepository.GetTrustContactAsync(trustUid, TrustContactRole.SfsoLead, cancellationToken);

            return new SchoolDfeContacts(regionsGroupLocalAuthorityLead, trustRelationshipManager, sfsoLead);
        }
    }
}
