namespace DfE.FindInformationAcademiesTrusts.Http;

public interface IHttpClientServiceTemp
{
   Task<ApiResponseTemp<TResponse>> Post<TRequest, TResponse>(HttpClient httpClient, string path, TRequest requestBody)
      where TResponse : class;

   Task<ApiResponseTemp<TResponse>> Put<TRequest, TResponse>(HttpClient httpClient, string path, TRequest requestBody)
      where TResponse : class;

   Task<ApiResponseTemp<TResponse>> Get<TResponse>(HttpClient httpClient, string path)
      where TResponse : class;

   Task<ApiResponseTemp<TResponse>> Delete<TResponse>(HttpClient httpClient, string path)
      where TResponse : class;
}
