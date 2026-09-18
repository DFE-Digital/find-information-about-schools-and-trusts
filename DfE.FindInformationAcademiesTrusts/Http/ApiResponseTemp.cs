using System.Diagnostics.CodeAnalysis;
using System.Net;


namespace DfE.FindInformationAcademiesTrusts.Http;
[ExcludeFromCodeCoverage]
public class ApiResponseTemp<TBody>(HttpStatusCode statusCode, TBody body)
{
    public bool Success { get; } = (int)statusCode >= 200 && (int)statusCode < 300;
    
    public bool NotFound { get; } = (int)statusCode == 404;
    public HttpStatusCode StatusCode { get; } = statusCode;
    public TBody Body { get; } = body;
}
