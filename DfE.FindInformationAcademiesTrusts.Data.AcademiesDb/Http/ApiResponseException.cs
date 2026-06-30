using System.Runtime.Serialization;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Http;

[Serializable]
public class ApiResponseException : Exception
{
    public ApiResponseException(string message) : base(message)
    {
    }

    [Obsolete(DiagnosticId = "SYSLIB0051")]
    protected ApiResponseException(SerializationInfo info, StreamingContext context)  : base(info, context)
    {
    }
}
