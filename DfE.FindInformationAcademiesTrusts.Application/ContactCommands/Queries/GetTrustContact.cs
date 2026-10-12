using DfE.FindInformationAcademiesTrusts.Domain.Entities;
using DfE.FindInformationAcademiesTrusts.Domain.Enums;
using DfE.FindInformationAcademiesTrusts.Domain.Interfaces.Repositories;
using MediatR;

namespace DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Queries;

public record GetTrustContactQuery(
    int Uid,
    TrustContactRole Role
) : IRequest<TrustContact?>;

public class GetTrustContact
{
    public class GetTrustContactQueryHandler(ITrustContactRepository contactRepository)
        : IRequestHandler<GetTrustContactQuery, TrustContact?>
    {
        public async Task<TrustContact?> Handle(GetTrustContactQuery request, CancellationToken cancellationToken)
        {
            return await contactRepository.GetTrustContactAsync(request.Uid, request.Role, cancellationToken);
        }
    }
}
