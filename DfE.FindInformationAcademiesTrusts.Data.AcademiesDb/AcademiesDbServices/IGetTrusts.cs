using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Http;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Trusts;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.AcademiesDbServices;

public interface IGetTrusts
{
    Task<TrustListResponse<TrustDto>> SearchTrusts(string searchQuery);
    
    Task<TrustDto?> GetTrustByReferenceNumber(string referenceNumber);

    Task<TrustDto?> GetTrustByUkprn(string ukprn);

    Task<List<TrustDto?>> GetTrustsByUkprn(string ukprn);
    
    Task<TrustDto?> GetEstablishmentTrust(int urn);
}
