using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Http;
[ExcludeFromCodeCoverage]
public class HttpClientService : IHttpClientService
{
    private readonly ILogger<HttpClientService> _logger;

    public HttpClientService(ILogger<HttpClientService> logger)
    {
        _logger = logger;
    }

    public async Task<ApiResponse<TResponse>> Post<TRequest, TResponse>(HttpClient httpClient, string path, TRequest requestBody)
       where TResponse : class
    {
        JsonContent requestPayload = JsonContent.Create(requestBody);
        HttpResponseMessage result = await httpClient.PostAsync(path, requestPayload);

        return await HandleResponse<TResponse>(result);
    }

    public async Task<ApiResponse<TResponse>> Put<TRequest, TResponse>(HttpClient httpClient, string path, TRequest requestBody)
       where TResponse : class
    {
        JsonContent requestPayload = JsonContent.Create(requestBody);
        HttpResponseMessage result = await httpClient.PutAsync(path, requestPayload);

        return await HandleResponse<TResponse>(result);
    }

    public async Task<ApiResponse<TResponse>> Get<TResponse>(HttpClient httpClient, string path)
       where TResponse : class
    {
        
        HttpResponseMessage result = await httpClient.GetAsync(path);

        return await HandleResponse<TResponse>(result);
    }

    public async Task<ApiResponse<TResponse>> Delete<TResponse>(HttpClient httpClient, string path)
      where TResponse : class
    {
        HttpResponseMessage result = await httpClient.DeleteAsync(path);

        return await HandleResponse<TResponse>(result);
    }

    private async Task<ApiResponse<TResponse>> HandleResponse<TResponse>(HttpResponseMessage result) where TResponse : class
    {
        if (!result.IsSuccessStatusCode) return await HandleUnsuccessfulRequest<TResponse>(result);

        string json = await result.Content.ReadAsStringAsync();

        return new ApiResponse<TResponse>(result.StatusCode, JsonConvert.DeserializeObject<TResponse>(json)!);
    }

    private async Task<ApiResponse<TResponse>> HandleUnsuccessfulRequest<TResponse>(HttpResponseMessage result)
       where TResponse : class
    {
        string content = await result.Content.ReadAsStringAsync();

        _logger.LogError("Request to Api failed | StatusCode - {StatusCode} | Content - {Content}",
           result.StatusCode, content);

        return new ApiResponse<TResponse>(result.StatusCode, null!);
    }
}
