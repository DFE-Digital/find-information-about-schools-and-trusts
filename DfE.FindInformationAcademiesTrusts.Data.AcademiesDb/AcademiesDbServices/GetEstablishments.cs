using System.Net.Http.Json;
using System.Web;
using Dfe.AcademiesApi.Client.Contracts;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Http;
using Microsoft.Extensions.Logging;
using EstablishmentDto = GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Establishments.EstablishmentDto;


namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.AcademiesDbServices;

public class GetEstablishments(IDfeHttpClientFactory httpClientFactory,
    IHttpClientService httpClientService) : IGetEstablishments
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateAcademiesClient();
    
    public async Task<List<EstablishmentDto>> SearchEstablishments(string searchQuery)
    {
        string path = $"v4/establishments?name={searchQuery}&urn={searchQuery}&excludeClosed=true&matchAny=true";
        
        ApiResponse<List<EstablishmentDto>> result = await httpClientService.Get<List<EstablishmentDto>>(_httpClient, path);

        if (!result.Success) throw new ApiResponseException($"Request to Api failed | StatusCode - {result.StatusCode}");

        return result.Body;
    }
    
    public async Task<EstablishmentDto> GetEstablishment(int urn)
    {
        string path = $"v4/establishment/urn/{urn}";

        ApiResponse<EstablishmentDto> result = await httpClientService.Get<EstablishmentDto>(_httpClient, path);

        if (!result.Success) throw new ApiResponseException($"Request to Api failed | StatusCode - {result.StatusCode}");

        return result.Body;
    }

    public async Task<EstablishmentResponse> GetEstablishmentWithSenData(int urn)
    {
        string path = $"establishment/urn/{urn}";

        ApiResponse<EstablishmentResponse> result = await httpClientService.Get<EstablishmentResponse>(_httpClient, path);

        if (!result.Success) throw new ApiResponseException($"Request to Api failed | StatusCode - {result.StatusCode}");

        return result.Body;
    }
}
