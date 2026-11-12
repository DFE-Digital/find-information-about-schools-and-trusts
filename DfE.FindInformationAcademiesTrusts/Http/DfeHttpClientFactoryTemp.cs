using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace DfE.FindInformationAcademiesTrusts.Http;
[ExcludeFromCodeCoverage]
/// <summary>
/// Creates an http client, with correlation context headers configured. You MUST register this as a scoped dependency
/// </summary>
public class DfeHttpClientFactoryTemp : IDfeHttpClientFactoryTemp
{
    private readonly IHttpClientFactory _httpClientFactory;

    public const string AcademiesClientName = "AcademiesClient";
    private const string CorrelationIdHeaderKey = "x-correlationId";

    public DfeHttpClientFactoryTemp(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Creates an http client pointing to the trams/academies api, with correlation context headers configured
    /// </summary>
    /// <returns></returns>
    public HttpClient CreateAcademiesClient()
    {
        return CreateClient(AcademiesClientName);
    }

    /// <summary>
    /// Creates an http client, with correlation context headers configured
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>A HttpClient.</returns>
    public HttpClient CreateClient(string name)
    {
        var httpClient = _httpClientFactory.CreateClient(name);
        var correlationId = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString();
        httpClient.DefaultRequestHeaders.Remove(CorrelationIdHeaderKey);
        httpClient.DefaultRequestHeaders.Add(CorrelationIdHeaderKey, correlationId);
        return httpClient;
    }
}
