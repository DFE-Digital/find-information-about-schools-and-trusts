using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Exceptions;
using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.Services.School;

namespace DfE.FindInformationAcademiesTrusts.Services.Export;

public interface IOfstedSchoolDataExportService
{
    Task<byte[]> BuildAsync(int urn);
}

public class OfstedSchoolDataExportService(ISchoolService schoolService, ISchoolOverviewDetailsService schoolOverviewDetailsService) : ExportBuilder("Ofsted"), IOfstedSchoolDataExportService
{
    private readonly List<string> _headers =
    [
        "Inspection type",
        "Date of inspection",
        "Grade",
    ];

    public async Task<byte[]> BuildAsync(int urn)
    {
        var schoolDetails = await schoolService.GetSchoolSummaryAsync(urn);

        if (schoolDetails is null)
        {
            throw new DataIntegrityException($"School summary not found for URN: {urn}");
        }
        
        var schoolOverview =
            await schoolOverviewDetailsService.GetSchoolOverviewDetailsAsync(urn, schoolDetails.Category);
        var headlineGrades = await schoolService.GetOfstedHeadlineGrades(urn);
        return WriteSchoolInformation(schoolOverview, schoolDetails.Category)
            .WriteHeaders(_headers)
            .WriteRows(() => WriteRows(headlineGrades))
            .Build();
    }
    
    private void WriteRows(OfstedHeadlineGradesServiceModel headlineGrades)
    {
        if (headlineGrades.HasRecentShortInspection)
        {
            WriteRecentShortInspectionRow(headlineGrades.ShortInspection);
        }
        WriteFullInspectionRow(headlineGrades.CurrentInspection, true);
        WriteFullInspectionRow(headlineGrades.PreviousInspection, false);
    }

    private void WriteRecentShortInspectionRow(OfstedShortInspection shortInspection)
    {
        WriteInspectionRow("Recent short inspection", shortInspection.InspectionDate, shortInspection.InspectionOutcome);
    }

    private void WriteFullInspectionRow(OfstedFullInspectionSummary fullInspection, bool isCurrent)
    {
        var inspectionType = isCurrent ? "Current inspection" : "Previous inspection";
        WriteInspectionRow(inspectionType, fullInspection.InspectionDate, fullInspection.InspectionOutcome.ToDisplayString(isCurrent));
    }

    private void WriteInspectionRow(string inspectionType, DateTime? inspectionDate, string? inspectionOutcome)
    {
        SetTextCell(ExportColumns.OfstedSchoolColumns.InspectionType, inspectionType);
        SetDateCell(ExportColumns.OfstedSchoolColumns.DateOfInspection, inspectionDate);
        SetTextCell(ExportColumns.OfstedSchoolColumns.Grade, inspectionOutcome ?? string.Empty);

        CurrentRow++;
    }
}
