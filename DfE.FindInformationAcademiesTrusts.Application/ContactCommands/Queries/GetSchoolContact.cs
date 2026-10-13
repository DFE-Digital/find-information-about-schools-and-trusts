using DfE.FindInformationAcademiesTrusts.Domain.Entities;
using DfE.FindInformationAcademiesTrusts.Domain.Enums;
using DfE.FindInformationAcademiesTrusts.Domain.Interfaces.Repositories;
using MediatR;

namespace DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Queries;

public record GetSchoolContactQuery(
    int Urn,
    SchoolContactRole Role
) : IRequest<SchoolContact?>;

public class GetSchoolContact
{
    public class GetSchoolContactQueryHandler(ISchoolContactRepository contactRepository)
        : IRequestHandler<GetSchoolContactQuery, SchoolContact?>
    {
        public async Task<SchoolContact?> Handle(GetSchoolContactQuery request, CancellationToken cancellationToken)
        {
            return await contactRepository.GetSchoolContactAsync(request.Urn, request.Role, cancellationToken);
        }
    }
}
