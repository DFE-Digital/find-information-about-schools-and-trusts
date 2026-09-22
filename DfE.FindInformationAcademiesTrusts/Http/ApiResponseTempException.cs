using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace DfE.FindInformationAcademiesTrusts.Http;
[ExcludeFromCodeCoverage]
public class ApiResponseTempException : Exception
{
    public ApiResponseTempException(string message)
        : base(message)
    {
    }

    public ApiResponseTempException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}