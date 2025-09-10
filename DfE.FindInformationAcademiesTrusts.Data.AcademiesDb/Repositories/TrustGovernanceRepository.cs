using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Contexts;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Extensions;
using DfE.FindInformationAcademiesTrusts.Data.Repositories;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Trust;
using Microsoft.EntityFrameworkCore;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Repositories;

public class TrustGovernanceRepository(
    IAcademiesDbContext dbContext,
    IStringFormattingUtilities stringFormattingUtilities
) : ITrustGovernanceRepository
{
    public Task<List<Governor>> GetTrustGovernanceAsync(string uidOrUrn)
    {
        return dbContext.GiasGovernances
            .Where(governance => governance.Uid == uidOrUrn || governance.Urn == uidOrUrn)
            .Select(governance => new Governor(
                governance.Gid!,
                governance.Uid!,
                stringFormattingUtilities.GetFullName(
                    governance.Forename1!,
                    governance.Forename2!,
                    governance.Surname!
                ),
                governance.Role!,
                governance.AppointingBody!,
                governance.DateOfAppointment.ParseAsNullableDate(),
                governance.DateTermOfOfficeEndsEnded.ParseAsNullableDate(),
                null
            ))
            .ToListAsync();
    }
}
