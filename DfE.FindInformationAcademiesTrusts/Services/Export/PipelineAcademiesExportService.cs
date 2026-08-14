using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Exceptions;
using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.Services.Academy;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using static DfE.FindInformationAcademiesTrusts.Services.Export.ExportColumns;

namespace DfE.FindInformationAcademiesTrusts.Services.Export;

public interface IPipelineAcademiesExportService
{
    Task<byte[]> BuildAsync(string uid,string referenceNumber);
}

public class PipelineAcademiesExportService(
    ITrustService trustService,
    IAcademyService academyService) : ExportBuilder("Pipeline Academies"), IPipelineAcademiesExportService
{
    public async Task<byte[]> BuildAsync(string uid,string referenceNumber)
    {
        var trustSummary = await trustService.GetTrustSummaryAsync(referenceNumber);

        if (trustSummary is null)
        {
            throw new DataIntegrityException($"Trust summary not found for trust reference number {referenceNumber}");
        }
        
        var preAdvisoryAcademies = await academyService.GetAcademiesPipelinePreAdvisoryAsync(referenceNumber);
        var postAdvisoryAcademies = await academyService.GetAcademiesPipelinePostAdvisoryAsync(referenceNumber);
        var freeSchools = await academyService.GetAcademiesPipelineFreeSchoolsAsync(referenceNumber);

        return WriteTrustInformation(trustSummary)
            .WriteRows(WriteHeadersForPreAdvisory)
            .WriteRows(() => WriteRows(preAdvisoryAcademies))
            .WriteRows(WriteHeadersForPostAdvisory)
            .WriteRows(() => WriteRows(postAdvisoryAcademies))
            .WriteRows(WriteHeadersForFreeSchools)
            .WriteRows(() => WriteRows(freeSchools))
            .Build();
    }


    private void WriteHeadersForPreAdvisory()
    {
        CurrentRow++;
        WriteHeaders(["Pre-advisory academies"]);
        WriteHeaders([
            CommonColumnNames.SchoolName, CommonColumnNames.Urn, CommonColumnNames.AgeRange,
            CommonColumnNames.LocalAuthority, "Project type", "Proposed conversion or transfer date"
        ]);
    }

    private void WriteHeadersForPostAdvisory()
    {
        CurrentRow++;
        WriteHeaders(["Post-advisory academies"]);
        WriteHeaders([
            CommonColumnNames.SchoolName, CommonColumnNames.Urn, CommonColumnNames.AgeRange,
            CommonColumnNames.LocalAuthority, "Project type", "Proposed conversion or transfer date"
        ]);
    }

    private void WriteHeadersForFreeSchools()
    {
        CurrentRow++;
        WriteHeaders(["Free schools"]);
        WriteHeaders([
            CommonColumnNames.SchoolName, CommonColumnNames.Urn, CommonColumnNames.AgeRange,
            CommonColumnNames.LocalAuthority, "Project type", "Provisional opening date"
        ]);
    }

    private void WriteRows(AcademyPipelineServiceModel[] pipelineAcademies)
    {
        var orderedPipelineAcademies = pipelineAcademies.OrderBy(a => a.EstablishmentName).ToArray();

        foreach (var details in orderedPipelineAcademies)
        {
            GeneratePipelineAcademyRow(details);
        }
    }

    private void GeneratePipelineAcademyRow(AcademyPipelineServiceModel pipelineAcademy)
    {
        SetTextCell(PipelineAcademiesColumns.SchoolName, pipelineAcademy.EstablishmentName ?? string.Empty);
        SetTextCell(PipelineAcademiesColumns.Urn, pipelineAcademy.Urn ?? string.Empty);
        SetTextCell(PipelineAcademiesColumns.AgeRange,
            pipelineAcademy.AgeRange?.ToTabularDisplayString() ?? "Unconfirmed");
        SetTextCell(PipelineAcademiesColumns.LocalAuthority, pipelineAcademy.LocalAuthority ?? string.Empty);
        SetTextCell(PipelineAcademiesColumns.ProjectType, pipelineAcademy.ProjectType ?? string.Empty);

        if (pipelineAcademy.ChangeDate != null)
        {
            SetDateCell(PipelineAcademiesColumns.ChangeDate, pipelineAcademy.ChangeDate);
        }
        else
        {
            SetTextCell(PipelineAcademiesColumns.ChangeDate, "Unconfirmed");
        }

        CurrentRow++;
    }
}
