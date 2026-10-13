using DfE.FindInformationAcademiesTrusts.Domain.Entities;
using DfE.FindInformationAcademiesTrusts.Domain.Enums;

namespace DfE.FindInformationAcademiesTrusts.Domain.Interfaces.Repositories;

public interface ITrustContactRepository : IRepository<TrustContact>
{
    Task<TrustContact?> GetTrustContactAsync(int uid, TrustContactRole role, CancellationToken cancellationToken);
}
