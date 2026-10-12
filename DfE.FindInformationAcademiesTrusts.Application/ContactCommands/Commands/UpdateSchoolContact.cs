using DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Models;
using DfE.FindInformationAcademiesTrusts.Domain.Entities;
using DfE.FindInformationAcademiesTrusts.Domain.Enums;
using DfE.FindInformationAcademiesTrusts.Domain.Interfaces.Repositories;
using MediatR;

namespace DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Commands;

public record UpdateSchoolContactCommand(
    int Urn,
    SchoolContactRole Role,
    string? Name,
    string? Email
) : IRequest<ContactUpdated>;

public class UpdateSchoolContact
{
    public class UpdateSchoolContactCommandHandler(ISchoolContactRepository contactRepository)
        : IRequestHandler<UpdateSchoolContactCommand, ContactUpdated>
    {
        public async Task<ContactUpdated> Handle(UpdateSchoolContactCommand request,
            CancellationToken cancellationToken)
        {
            var name = request.Name ?? string.Empty;
            var email = request.Email ?? string.Empty;

            var contact = await contactRepository.GetSchoolContactAsync(request.Urn, request.Role, cancellationToken);

            if (contact is null)
            {
                await contactRepository.AddAsync(new SchoolContact
                {
                    Urn = request.Urn,
                    Role = request.Role,
                    Name = name,
                    Email = email
                }, cancellationToken);

                return new ContactUpdated(true, true);
            }

            var result = new ContactUpdated(contact.Name != name, contact.Email != email);

            contact.Name = name;
            contact.Email = email;
            await contactRepository.UpdateAsync(contact, cancellationToken);

            return result;
        }
    }
}
