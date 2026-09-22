using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DfE.FindInformationAcademiesTrusts.Http;
[ExcludeFromCodeCoverage]
public class HttpClientServiceTemp : IHttpClientServiceTemp
{
    private readonly ILogger<HttpClientServiceTemp> _logger;

    public HttpClientServiceTemp(ILogger<HttpClientServiceTemp> logger)
    {
        _logger = logger;
    }

    public async Task<ApiResponseTemp<TResponse>> Post<TRequest, TResponse>(HttpClient httpClient, string path, TRequest requestBody)
       where TResponse : class
    {
        JsonContent requestPayload = JsonContent.Create(requestBody);
        HttpResponseMessage result = await httpClient.PostAsync(path, requestPayload);

        return await HandleResponse<TResponse>(result);
    }

    public async Task<ApiResponseTemp<TResponse>> Put<TRequest, TResponse>(HttpClient httpClient, string path, TRequest requestBody)
       where TResponse : class
    {
        JsonContent requestPayload = JsonContent.Create(requestBody);
        HttpResponseMessage result = await httpClient.PutAsync(path, requestPayload);

        return await HandleResponse<TResponse>(result);
    }

    public async Task<ApiResponseTemp<TResponse>> Get<TResponse>(HttpClient httpClient, string path)
       where TResponse : class
    {
        
        HttpResponseMessage result = await httpClient.GetAsync(path);

        return await HandleResponse<TResponse>(result);
    }

    public async Task<ApiResponseTemp<TResponse>> Delete<TResponse>(HttpClient httpClient, string path)
      where TResponse : class
    {
        HttpResponseMessage result = await httpClient.DeleteAsync(path);

        return await HandleResponse<TResponse>(result);
    }

    private async Task<ApiResponseTemp<TResponse>> HandleResponse<TResponse>(HttpResponseMessage result) where TResponse : class
    {
        if (!result.IsSuccessStatusCode) return await HandleUnsuccessfulRequest<TResponse>(result);

        string json = await result.Content.ReadAsStringAsync();

        return new ApiResponseTemp<TResponse>(result.StatusCode, JsonConvert.DeserializeObject<TResponse>(json)!);
    }

    private async Task<ApiResponseTemp<TResponse>> HandleUnsuccessfulRequest<TResponse>(HttpResponseMessage result)
       where TResponse : class
    {
        string content = await result.Content.ReadAsStringAsync();

        _logger.LogError("Request to Api failed | StatusCode - {StatusCode} | Content - {Content}",
           result.StatusCode, content);

        return new ApiResponseTemp<TResponse>(result.StatusCode, null!);
    }
}
