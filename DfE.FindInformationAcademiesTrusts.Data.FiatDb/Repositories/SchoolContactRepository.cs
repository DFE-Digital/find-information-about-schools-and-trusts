using DfE.FindInformationAcademiesTrusts.Data.FiatDb.Contexts;
using DfE.FindInformationAcademiesTrusts.Domain.Entities;
using DfE.FindInformationAcademiesTrusts.Domain.Enums;
using DfE.FindInformationAcademiesTrusts.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DfE.FindInformationAcademiesTrusts.Data.FiatDb.Repositories;

public class SchoolContactRepository(FindInformationAcademiesTrustsContext dbContext)
    : Repository<SchoolContact, FindInformationAcademiesTrustsContext>(dbContext), ISchoolContactRepository
{
    public async Task<SchoolContact?> GetSchoolContactAsync(int urn, SchoolContactRole role,
        CancellationToken cancellationToken)
    {
        return await DbSet()
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.Urn == urn && c.Role == role, cancellationToken);
    }
}
