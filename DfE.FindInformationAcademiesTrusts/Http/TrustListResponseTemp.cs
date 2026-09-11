namespace DfE.FindInformationAcademiesTrusts.Http
{
    public class TrustListResponseTemp<TItem> where TItem : class
    {
        public IEnumerable<TItem>? Data { get; set; }
    }
}

