using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Extensions;
using DfE.FindInformationAcademiesTrusts.Data.Repositories;
using GovUK.Dfe.PersonsApi.Client.Contracts;

namespace DfE.FindInformationAcademiesTrusts.Services.Trust;

public interface ITrustGovernanceProvider
{
    Task<List<Governor>> GetMultiAcademyTrustGovernorsAsync(string trn);
    Task<List<Governor>> GetSingleAcademyTrustGovernorsAsync(int urn);
}

public class TrustGovernanceProvider(
    ITrustsClient trustsClient,
    IEstablishmentsClient establishmentsClient) : ITrustGovernanceProvider
{
    public async Task<List<Governor>> GetMultiAcademyTrustGovernorsAsync(string trn)
    {
        var result = await trustsClient.GetAllPersonsAssociatedWithTrustByTrnOrUkPrnAsync(trn).ConfigureAwait(false);

        return result.Select(person => ToGovernor(
            person.DisplayName,
            person.Roles[0],
            person.AppointingBody,
            person.DateOfAppointment,
            person.DateTermOfOfficeEndsEnded,
            person.Email)).ToList();
    }

    public async Task<List<Governor>> GetSingleAcademyTrustGovernorsAsync(int urn)
    {
        var result = await establishmentsClient.GetAllPersonsAssociatedWithAcademyByUrnAsync(urn);

        return result.Select(person => ToGovernor(
            person.DisplayName,
            person.Roles[0],
            person.AppointingBody,
            person.DateOfAppointment,
            person.DateTermOfOfficeEndsEnded,
            person.Email)).ToList();
    }

    private static Governor ToGovernor(
        string fullName,
        string role,
        string appointingBody,
        string? dateOfAppointment,
        string? dateOfTermEnd,
        string? email)
    {
        return new Governor(
            FullName: fullName,
            Role: role,
            AppointingBody: appointingBody,
            DateOfAppointment: dateOfAppointment.ParseAsNullableDate(),
            DateOfTermEnd: dateOfTermEnd.ParseAsNullableDate(),
            Email: email);
    }
}
