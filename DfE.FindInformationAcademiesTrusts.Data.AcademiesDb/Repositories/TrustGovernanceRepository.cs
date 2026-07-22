using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Contexts;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Extensions;
using DfE.FindInformationAcademiesTrusts.Data.Repositories;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Trust;
using GovUK.Dfe.PersonsApi.Client.Contracts;
using Microsoft.EntityFrameworkCore;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Repositories;

public class TrustGovernanceRepository(
    ITrustsClient trustsClient
) : ITrustGovernanceRepository
{
    public async Task<List<Governor>> GetTrustGovernanceAsync(string trn)
    {
        var result = await trustsClient.GetAllPersonsAssociatedWithTrustByTrnOrUkPrnAsync(trn).ConfigureAwait(false);
        
        //convert result into list of Governors
        var governors = new List<Governor>();

        foreach (var person in result)
        {
            governors.Add(new Governor(
                FullName: person.DisplayName,
                Role: person.Roles.First(),
                AppointingBody: person.AppointingBody,
                DateOfAppointment: person.DateOfAppointment.ParseAsNullableDate(),
                DateOfTermEnd: person.DateTermOfOfficeEndsEnded.ParseAsNullableDate(),
                Email: person.Email));
        }
        
        return governors;
    }
}
