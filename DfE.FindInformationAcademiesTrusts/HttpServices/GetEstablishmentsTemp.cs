using System.Net.Http.Json;
using System.Web;
using Dfe.AcademiesApi.Client.Contracts;
using DfE.FindInformationAcademiesTrusts.Http;
using Microsoft.Extensions.Logging;
using EstablishmentDto = GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Establishments.EstablishmentDto;


namespace DfE.FindInformationAcademiesTrusts.HttpServices;

public class GetEstablishmentsTemp(IDfeHttpClientFactoryTemp httpClientFactoryTemp,
    IHttpClientServiceTemp httpClientServiceTemp) : IGetEstablishmentsTemp
{
    private readonly HttpClient _httpClient = httpClientFactoryTemp.CreateAcademiesClient();
    
    public async Task<List<EstablishmentDto>> SearchEstablishments(string searchQuery)
    {
        string path = $"v4/establishments?name={searchQuery}&urn={searchQuery}&excludeClosed=true&matchAny=true";
        
        ApiResponseTemp<List<EstablishmentDto>> result = await httpClientServiceTemp.Get<List<EstablishmentDto>>(_httpClient, path);

        if (!result.Success) throw new ApiResponseExceptionTemp($"Request to Api failed | StatusCode - {result.StatusCode}");

        return result.Body;
    }
    
    public async Task<EstablishmentDto> GetEstablishment(int urn)
    {
        string path = $"v4/establishment/urn/{urn}";

        ApiResponseTemp<EstablishmentDto> result = await httpClientServiceTemp.Get<EstablishmentDto>(_httpClient, path);

        if (!result.Success) throw new ApiResponseExceptionTemp($"Request to Api failed | StatusCode - {result.StatusCode}");

        return result.Body;
    }
    
    public async Task<EstablishmentDto[]> GetEstablishmentsByTrustReferenceNumber(string trustReferenceNumber)
    {
        string path = $"/v4/establishments/trustReferenceNumber?trustReferenceNumber={trustReferenceNumber}";

        ApiResponseTemp<EstablishmentDto[]> result = await httpClientServiceTemp.Get<EstablishmentDto[]>(_httpClient, path);

        if (!result.Success) throw new ApiResponseExceptionTemp($"Request to Api failed | StatusCode - {result.StatusCode}");

        return result.Body;
    }

    public async Task<EstablishmentResponse> GetEstablishmentWithSenData(int urn)
    {
        string path = $"establishment/urn/{urn}";

        ApiResponseTemp<EstablishmentResponse> result = await httpClientServiceTemp.Get<EstablishmentResponse>(_httpClient, path);

        if (!result.Success) throw new ApiResponseExceptionTemp($"Request to Api failed | StatusCode - {result.StatusCode}");

        return result.Body;
    }
    
    public async Task<List<EstablishmentDto>> GetEstablishmentsByUrns(List<int> urns)
    {
        const string path = "v4/establishments/bulk/urns";

        var request = new
        {
            urns = urns.ToArray()
        };

        ApiResponseTemp<List<EstablishmentDto>> result =
            await httpClientServiceTemp.Post<object, List<EstablishmentDto>>(
                _httpClient,
                path,
                request);

        if (!result.Success)
            throw new ApiResponseExceptionTemp($"Request to Api failed | StatusCode - {result.StatusCode}");

        return result.Body;
    }
}
