using DfE.FindInformationAcademiesTrusts.Data;

namespace DfE.FindInformationAcademiesTrusts.Services.Ofsted
{
    public class OlderInspectionServiceModel : IOfstedInspection
    {
        public List<OfstedRating> Ratings { get; set; } = [];
    }
}
