namespace DfE.FindInformationAcademiesTrusts.Extensions
{
    using DfE.FindInformationAcademiesTrusts.Services.School;

    public static class OfstedOverviewServiceModelExtensions
    {
        public static string AsInspectionTypeString(this OverviewServiceModel? model)
        {
            if (model is null)
            {
                return "Not applicable";
            }

            return model.IsReportCard ? "Report card" : "Older inspection";
        }
    }
}
