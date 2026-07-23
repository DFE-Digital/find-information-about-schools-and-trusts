namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Http
{
    public class TrustListResponse<TItem> where TItem : class
    {
        public IEnumerable<TItem>? Data { get; set; }
    }
}

