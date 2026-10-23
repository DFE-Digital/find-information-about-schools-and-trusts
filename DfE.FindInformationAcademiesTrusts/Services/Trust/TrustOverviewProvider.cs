using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.AcademiesDbServices;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Extensions;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Trust;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Trusts;

namespace DfE.FindInformationAcademiesTrusts.Services.Trust;

public interface ITrustOverviewProvider
{
    Task<TrustDto?> GetEstablishmentTrust(int urn);
    Task<TrustDto?> GetTrustByReferenceNumber(string referenceNumber);
    Task<TrustOverview> GetTrustOverviewAsync(string trustReferenceNumber);
}

public class TrustOverviewProvider(
    IGetTrusts getTrusts,
    IStringFormattingUtilities stringFormattingUtilities) : ITrustOverviewProvider
{
    public Task<TrustDto?> GetEstablishmentTrust(int urn)
    {
        return getTrusts.GetEstablishmentTrust(urn);
    }

    public Task<TrustDto?> GetTrustByReferenceNumber(string referenceNumber)
    {
        return getTrusts.GetTrustByReferenceNumber(referenceNumber);
    }

    public async Task<TrustOverview> GetTrustOverviewAsync(string trustReferenceNumber)
    {
        var result = await getTrusts.GetTrustByReferenceNumber(trustReferenceNumber) ?? throw new InvalidOperationException(
            $"Trust with reference number {trustReferenceNumber} was not found.");

        return new TrustOverview(
            result.GroupUid!,
            trustReferenceNumber,
            result.Ukprn,
            result.CompaniesHouseNumber,
            result.Type.Name,
            stringFormattingUtilities.BuildAddressString(
                result.Address.Street,
                result.Address.Locality,
                result.Address.Town,
                result.Address.Postcode
            ),
            result.Gor,
            result.OpenDate.ParseAsNullableDate()
        );
    }
}
