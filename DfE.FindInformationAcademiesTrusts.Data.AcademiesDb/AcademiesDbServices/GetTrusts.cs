using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Http;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Establishments;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Trusts;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.AcademiesDbServices;

public class GetTrusts(IDfeHttpClientFactory httpClientFactory,
    IHttpClientService httpClientService) : IGetTrusts
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateAcademiesClient();
    
    public async Task<TrustListResponse<TrustDto>> SearchTrusts(string searchQuery)
    {
        
        string path = $"v4/trusts?groupName={searchQuery}&urn={searchQuery}&page=1&count=10&status=Open";
        
        _httpClient.DefaultRequestHeaders.Add("x-correlationId", Guid.NewGuid().ToString());
        
        
        ApiResponse<TrustListResponse<TrustDto>> result = await httpClientService.Get<TrustListResponse<TrustDto>>(_httpClient, path);

        if (!result.Success) throw new ApiResponseException($"Request to Api failed | StatusCode - {result.StatusCode}");

        return result.Body;
    }
    
    public async Task<TrustDto?> GetTrustByReferenceNumber(string referenceNumber)
    {
        
        string path = $"v4/trust/trustReferenceNumber/{referenceNumber}";
        
        
        ApiResponse<TrustDto> result = await httpClientService.Get<TrustDto>(_httpClient, path);

        if (result.NotFound)
        {
            return null;
        }

        if (!result.Success)
            throw new ApiResponseException($"Request to Api failed | StatusCode - {result.StatusCode}");

        return result.Body;
    }

   
    
    public async Task<TrustDto?> GetTrustByUkprn(string ukprn)
    {

        string path = $"v4/trust/{ukprn}";


        ApiResponse<TrustDto> result = await httpClientService.Get<TrustDto>(_httpClient, path);

        if (result.NotFound)
        {
            return null;
        }

        if (!result.Success)
            throw new ApiResponseException($"Request to Api failed | StatusCode - {result.StatusCode}");

        return result.Body;
    }
    
    public async Task<List<TrustDto?>> GetTrustsByUkprn(string ukprn)
    {

        string path = $"v4/establishments/trust?trustUkprn={ukprn}";


        ApiResponse<List<TrustDto?>> result = await httpClientService.Get<List<TrustDto?>>(_httpClient, path);

        if (result.NotFound)
        {
            return null!;
        }

        if (!result.Success)
            throw new ApiResponseException($"Request to Api failed | StatusCode - {result.StatusCode}");

        return result.Body;
    }

    public async Task<TrustDto?> GetEstablishmentTrust(int urn)
    {
        string path = "/v4/trusts/establishments/urns";
        var payload = new { urns = new int[] { urn } };

        ApiResponse<Dictionary<int, TrustDto>> result =
            await httpClientService.Post<object, Dictionary<int, TrustDto>>(_httpClient, path, payload);

        if (!result.Success)
        {
            if (result.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            else
            {
                throw new ApiResponseException($"Request to Api failed | StatusCode - {result.StatusCode}");
            }
        }

        return result.Body.FirstOrDefault().Value;
    }
}
