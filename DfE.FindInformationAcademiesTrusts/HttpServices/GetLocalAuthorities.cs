using DfE.FindInformationAcademiesTrusts.Http;
using GovUK.Dfe.AcademiesApi.Client.Contracts;

namespace DfE.FindInformationAcademiesTrusts.HttpServices;

public class GetLocalAuthorities(IDfeHttpClientFactoryTemp httpClientFactoryTemp,
    IHttpClientServiceTemp httpClientServiceTemp) : IGetLocalAuthorities
{
    private readonly HttpClient _httpClient = httpClientFactoryTemp.CreateAcademiesClient();

    public async Task<List<NameAndCodeDto>> SearchLocalAuthorities(string? name, string? code)
    {
        var query = new List<string>();
        if (!string.IsNullOrWhiteSpace(name)) query.Add($"name={Uri.EscapeDataString(name)}");
        if (!string.IsNullOrWhiteSpace(code)) query.Add($"code={Uri.EscapeDataString(code)}");

        string path = query.Count > 0
            ? $"v4/local-authorities?{string.Join("&", query)}"
            : "v4/local-authorities";

        ApiResponseTemp<List<NameAndCodeDto>> result =
            await httpClientServiceTemp.Get<List<NameAndCodeDto>>(_httpClient, path);

        if (!result.Success) throw new ApiResponseTempException($"Request to Api failed | StatusCode - {result.StatusCode}");

        return result.Body;
    }
}
