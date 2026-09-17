using DfE.FindInformationAcademiesTrusts.Http;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Establishments;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Trusts;

namespace DfE.FindInformationAcademiesTrusts.HttpServices;

public class GetTrustsTemp(IDfeHttpClientFactoryTemp httpClientFactoryTemp,
    IHttpClientServiceTemp httpClientServiceTemp) : IGetTrustsTemp
{
    private readonly HttpClient _httpClient = httpClientFactoryTemp.CreateAcademiesClient();
    
    public async Task<TrustListResponseTemp<TrustDto>> SearchTrusts(string searchQuery)
    {
        
        string path = $"v4/trusts?groupName={searchQuery}&urn={searchQuery}&page=1&count=10&status=Open";
        
        _httpClient.DefaultRequestHeaders.Add("x-correlationId", Guid.NewGuid().ToString());
        
        
        ApiResponseTemp<TrustListResponseTemp<TrustDto>> result = await httpClientServiceTemp.Get<TrustListResponseTemp<TrustDto>>(_httpClient, path);

        if (!result.Success) throw new ApiResponseExceptionTemp($"Request to Api failed | StatusCode - {result.StatusCode}");

        return result.Body;
    }
    
    public async Task<TrustDto?> GetTrustByReferenceNumber(string referenceNumber)
    {
        
        string path = $"v4/trust/trustReferenceNumber/{referenceNumber}";
        
        
        ApiResponseTemp<TrustDto> result = await httpClientServiceTemp.Get<TrustDto>(_httpClient, path);

        if (result.NotFound)
        {
            return null;
        }
        
        if (!result.Success) throw new ApiResponseExceptionTemp($"Request to Api failed | StatusCode - {result.StatusCode}");

        return result.Body;
    }
    
    public async Task<TrustDto[]> GetTrustsByReferenceNumbers(
        IEnumerable<string> referenceNumbers)
    {
        string trns = string.Join("&trns=", referenceNumbers);

        string path = $"v4/trusts/trustReferenceNumber/bulk?trns={trns}";

        ApiResponseTemp<TrustDto[]> result =
            await httpClientServiceTemp.Get<TrustDto[]>(
                _httpClient,
                path);

        if (!result.Success)
        {
            throw new ApiResponseExceptionTemp(
                $"Request to Api failed | StatusCode - {result.StatusCode}");
        }

        return result.Body;
    }
    
    public async Task<TrustDto?> GetEstablishmentTrust(int urn)
    {
        string path = "/v4/trusts/establishments/urns";
        var payload = new { urns = new int[] { urn } };

        ApiResponseTemp<Dictionary<int, TrustDto>> result =
            await httpClientServiceTemp.Post<object, Dictionary<int, TrustDto>>(_httpClient, path, payload);

        if (!result.Success)
        {
            if (result.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            else
            {
                throw new ApiResponseExceptionTemp($"Request to Api failed | StatusCode - {result.StatusCode}");
            }
        }

        return result.Body.FirstOrDefault().Value;
    }
}
