using DfE.FindInformationAcademiesTrusts.Http;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Trusts;

namespace DfE.FindInformationAcademiesTrusts.HttpServices;

public interface IGetTrustsTemp
{
    Task<TrustListResponseTemp<TrustDto>> SearchTrusts(string searchQuery);
    
    Task<TrustDto?> GetTrustByReferenceNumber(string referenceNumber);

    Task<TrustDto[]> GetTrustsByReferenceNumbers(IEnumerable<string> referenceNumbers);


    Task<TrustDto?> GetEstablishmentTrust(int urn);
}
