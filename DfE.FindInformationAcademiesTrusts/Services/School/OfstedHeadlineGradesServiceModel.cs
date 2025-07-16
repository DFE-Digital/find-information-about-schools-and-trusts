using DfE.FindInformationAcademiesTrusts.Data;

namespace DfE.FindInformationAcademiesTrusts.Services.School;

public record OfstedHeadlineGradesServiceModel(
    OfstedShortInspection ShortInspection,
    OfstedFullInspectionSummary CurrentInspection,
    OfstedFullInspectionSummary PreviousInspection)
{
    public bool HasRecentShortInspection => ShortInspection.InspectionDate > CurrentInspection.InspectionDate;
}
