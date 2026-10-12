using DfE.FindInformationAcademiesTrusts.Domain.Entities;
using DfE.FindInformationAcademiesTrusts.Domain.Enums;

namespace DfE.FindInformationAcademiesTrusts.Domain.Interfaces.Repositories;

public interface ISchoolContactRepository : IRepository<SchoolContact>
{
    Task<SchoolContact?> GetSchoolContactAsync(int urn, SchoolContactRole role, CancellationToken cancellationToken);
}
