using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace DfE.FindInformationAcademiesTrusts.Http;
[ExcludeFromCodeCoverage]
public class ApiResponseExceptionTemp : Exception
{
    public ApiResponseExceptionTemp(string message)
        : base(message)
    {
    }

    public ApiResponseExceptionTemp(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}