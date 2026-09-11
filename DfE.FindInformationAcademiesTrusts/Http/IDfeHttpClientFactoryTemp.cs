namespace DfE.FindInformationAcademiesTrusts.Http;

public interface IDfeHttpClientFactoryTemp : IHttpClientFactory
{
    /// <summary>
    /// Creates an http client pointing to the trams/academies api, with correlation context headers configured
    /// </summary>
    /// <returns></returns>
    HttpClient CreateAcademiesClient();
}
