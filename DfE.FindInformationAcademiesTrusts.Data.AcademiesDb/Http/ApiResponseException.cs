using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Http;
[ExcludeFromCodeCoverage]
public class ApiResponseException : Exception
{
    public ApiResponseException(string message)
        : base(message)
    {
    }

    public ApiResponseException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}