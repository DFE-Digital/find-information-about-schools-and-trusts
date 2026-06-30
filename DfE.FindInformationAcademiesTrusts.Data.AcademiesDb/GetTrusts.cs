using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Http;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Establishments;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Trusts;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb;

public class GetTrusts(IDfeHttpClientFactory httpClientFactory,
    IHttpClientService httpClientService) : IGetTrusts
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateAcademiesClient();
    
    public async Task<TrustListResponse<TrustDto>> SearchTrusts(string searchQuery)
    {
        
        string path = $"v4/trusts?groupName={searchQuery}&urn={searchQuery}&page=1&count=10&status=Open";
        
        //_httpClient.DefaultRequestHeaders.Remove("x-correlationId");
        _httpClient.DefaultRequestHeaders.Add("x-correlationId", Guid.NewGuid().ToString());
        
        
        ApiResponse<TrustListResponse<TrustDto>> result = await httpClientService.Get<TrustListResponse<TrustDto>>(_httpClient, path);

        if (!result.Success) throw new ApiResponseException($"Request to Api failed | StatusCode - {result.StatusCode}");

        return result.Body;
    }
    
    public async Task<TrustDto?> GetTrustByReferenceNumber(string referenceNumber)
    {
        
        string path = $"v4/trust/trustReferenceNumber/{referenceNumber}";
        
        //_httpClient.DefaultRequestHeaders.Remove("x-correlationId");
        _httpClient.DefaultRequestHeaders.Add("x-correlationId", Guid.NewGuid().ToString());
        
        
        ApiResponse<TrustDto> result = await httpClientService.Get<TrustDto>(_httpClient, path);

        if (result.NotFound)
        {
            return null;
        }
        
        if (!result.Success) throw new ApiResponseException($"Request to Api failed | StatusCode - {result.StatusCode}");

        return result.Body;
    }
}
