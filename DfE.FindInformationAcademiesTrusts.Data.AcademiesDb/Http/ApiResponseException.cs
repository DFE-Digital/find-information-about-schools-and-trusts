using System.Runtime.Serialization;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Http;

[Serializable]
public class ApiResponseException : Exception
{
    public ApiResponseException(string message) : base(message)
    {
    }

    protected ApiResponseException(SerializationInfo info, StreamingContext context) 
    {
    }
}
