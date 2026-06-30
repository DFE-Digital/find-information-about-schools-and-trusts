using ICorrelationContext = GovUK.Dfe.CoreLibs.Http.Interfaces.ICorrelationContext;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Http;

/// <summary>
/// Creates an http client, with correlation context headers configured. You MUST register this as a scoped dependency
/// </summary>
public class DfeHttpClientFactory : IDfeHttpClientFactory
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ICorrelationContext _correlationContext;

    public const string AcademiesClientName = "AcademiesClient";
    private const string CorrelationIdHeaderKey = "x-correlationId";

    public DfeHttpClientFactory(IHttpClientFactory httpClientFactory, ICorrelationContext correlationContext)
    {
        _httpClientFactory = httpClientFactory;
        _correlationContext = correlationContext;
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
        httpClient.DefaultRequestHeaders.Add(CorrelationIdHeaderKey, _correlationContext.CorrelationId.ToString());
        return httpClient;
    }
}
