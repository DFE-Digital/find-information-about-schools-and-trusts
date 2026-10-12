using DfE.FindInformationAcademiesTrusts.Data.FiatDb.Contexts;
using DfE.FindInformationAcademiesTrusts.Domain.Entities;
using DfE.FindInformationAcademiesTrusts.Domain.Enums;
using DfE.FindInformationAcademiesTrusts.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DfE.FindInformationAcademiesTrusts.Data.FiatDb.Repositories;

public class TrustContactRepository(FindInformationAcademiesTrustsContext dbContext)
    : Repository<TrustContact, FindInformationAcademiesTrustsContext>(dbContext), ITrustContactRepository
{
    public async Task<TrustContact?> GetTrustContactAsync(int uid, TrustContactRole role,
        CancellationToken cancellationToken)
    {
        return await DbSet()
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.Uid == uid && c.Role == role, cancellationToken);
    }
}
