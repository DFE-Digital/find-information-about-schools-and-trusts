using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Data.FiatDb.Contexts;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.DataSource;
using Microsoft.EntityFrameworkCore;

namespace DfE.FindInformationAcademiesTrusts.Data.FiatDb.Repositories;

public interface IFiatDataSourceRepository
{
    Task<DataSource> GetSchoolContactDataSourceAsync(int urn, SchoolContactRole role);
    Task<DataSource> GetTrustContactDataSourceAsync(int uid, TrustContactRole role);
}

public class FiatDataSourceRepository(FiatDbContext dbContext) : IFiatDataSourceRepository
{
    public async Task<DataSource> GetSchoolContactDataSourceAsync(int urn, SchoolContactRole role)
    {
        return await dbContext
            .SchoolContacts
            .Where(contact => contact.Urn == urn && contact.Role == role)
            .Select(t => new DataSource(Source.FiatDb, t.LastModifiedAtTime, null, t.LastModifiedByEmail))
            .SingleOrDefaultAsync() ?? new DataSource(Source.FiatDb, null, null);
    }

    public async Task<DataSource> GetTrustContactDataSourceAsync(int uid, TrustContactRole role)
    {
        return await dbContext
            .TrustContacts
            .Where(contact => contact.Uid == uid && contact.Role == role)
            .Select(t => new DataSource(Source.FiatDb, t.LastModifiedAtTime, null, t.LastModifiedByEmail))
            .SingleOrDefaultAsync() ?? new DataSource(Source.FiatDb, null, null);
    }
}
