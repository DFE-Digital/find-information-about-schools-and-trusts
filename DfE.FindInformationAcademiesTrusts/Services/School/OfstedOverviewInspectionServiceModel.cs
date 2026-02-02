using DfE.FindInformationAcademiesTrusts.Data.Enums;

namespace DfE.FindInformationAcademiesTrusts.Services.School
{
    public record OfstedOverviewInspectionServiceModel(
        OverviewServiceModel? Current,
        OverviewServiceModel? Previous,
        ShortInspectionOverviewServiceModel? ShortInspection);

    public class OverviewServiceModel
    {
        public bool IsReportCard { get; set; }

        public DateOnly InspectionDate { get; set; }

        public BeforeOrAfterJoining BeforeOrAfterJoining { get; set; }
    }

    public class ShortInspectionOverviewServiceModel : OverviewServiceModel
    {
        public string? InspectionOutcome { get; set; } = string.Empty;
    }
}
