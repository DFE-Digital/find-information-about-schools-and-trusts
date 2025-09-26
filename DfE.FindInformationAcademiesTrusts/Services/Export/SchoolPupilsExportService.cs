using System.Numerics;
using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Exceptions;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.PupilCensus;
using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.Services.School;

namespace DfE.FindInformationAcademiesTrusts.Services.Export;

public interface ISchoolPupilsExportService
{
    Task<byte[]> BuildAsync(int urn);
}

public class SchoolPupilsExportService(
    ISchoolService schoolService,
    ISchoolPupilService schoolPupilService,
    IDateTimeProvider dateTimeProvider
) : ExportBuilder("Pupil population"), ISchoolPupilsExportService
{
    private static readonly List<string> Headers =
    [
        "Year",
        "Number of pupils on role (NOR)",
        "Number of eligible pupils with EHC plan",
        "Percentage of eligible pupils with EHC plan",
        "Number of eligible pupils with SEN support",
        "Percentage of eligible pupils with SEN support",
        "Number of pupils with English as an additional language",
        "Percentage of pupils with English as an additional language",
        "Number of pupils eligible for free school meals",
        "Percentage of pupils eligible for free school meals",
        "Percentage of overall absence",
        "Percentage of enrolments who are persistent absentees"
    ];

    public async Task<byte[]> BuildAsync(int urn)
    {
        var schoolSummary = await schoolService.GetSchoolSummaryAsync(urn);
        
        if (schoolSummary is null)
        {
            throw new DataIntegrityException($"School with URN '{urn}' was not found.");
        }

        var from = CensusYear.Previous(dateTimeProvider, Census.Spring, 3);
        var to = CensusYear.Next(dateTimeProvider, Census.Spring);

        var schoolPopulations = await schoolPupilService.GetSchoolPopulationStatisticsAsync(urn, from, to);
        var attendances = await schoolPupilService.GetAttendanceStatisticsAsync(urn, from, to);

        return WriteSchoolNameAndUrn(schoolSummary)
            .WriteHeaders(Headers)
            .WriteRows(() => WriteRows(from, to, schoolPopulations, attendances))
            .Build();
    }

    private void WriteRows(
        CensusYear from,
        CensusYear to,
        AnnualStatistics<SchoolPopulation> schoolPopulations,
        AnnualStatistics<Attendance> attendances
    )
    {
        foreach (var year in Enumerable.Range(from.Value, to.Value - from.Value + 1).Reverse())
        {
            var schoolPopulation =
                schoolPopulations.TryGetValue(year, out var value) ? value : SchoolPopulation.Unknown;
            var attendance = attendances.TryGetValue(year, out var value2) ? value2 : Attendance.Unknown;
            
            WriteRow(year, schoolPopulation, attendance);
        }
    }

    private void WriteRow(CensusYear year, SchoolPopulation schoolPopulation, Attendance attendance)
    {
        SetNumberCell(ExportColumns.SchoolPopulationColumns.Year, year.Value);
        SetCellToStatistic(
            ExportColumns.SchoolPopulationColumns.NumberOfPupilsOnRole,
            schoolPopulation.PupilsOnRole
        );
        SetCellToStatistic(
            ExportColumns.SchoolPopulationColumns.NumberOfEligiblePupilsWithEhcPlan,
            schoolPopulation.PupilsWithEhcPlan
        );
        SetCellToStatistic(
            ExportColumns.SchoolPopulationColumns.PercentageOfEligiblePupilsWithEhcPlan,
            schoolPopulation.PupilsWithEhcPlanPercentage
        );
        SetCellToStatistic(
            ExportColumns.SchoolPopulationColumns.NumberOfEligiblePupilsWithSenSupport,
            schoolPopulation.PupilsWithSenSupport
        );
        SetCellToStatistic(
            ExportColumns.SchoolPopulationColumns.PercentageOfEligiblePupilsWithSenSupport,
            schoolPopulation.PupilsWithSenSupportPercentage
        );
        SetCellToStatistic(
            ExportColumns.SchoolPopulationColumns.NumberOfPupilsWithEnglishAsAnAdditionalLanguage,
            schoolPopulation.PupilsWithEnglishAsAdditionalLanguage
        );
        SetCellToStatistic(
            ExportColumns.SchoolPopulationColumns.PercentageOfPupilsWithEnglishAsAnAdditionalLanguage,
            schoolPopulation.PupilsWithEnglishAsAdditionalLanguagePercentage
        );
        SetCellToStatistic(
            ExportColumns.SchoolPopulationColumns.NumberOfPupilsEligibleForFreeSchoolMeals,
            schoolPopulation.PupilsEligibleForFreeSchoolMeals
        );
        SetCellToStatistic(
            ExportColumns.SchoolPopulationColumns.PercentageOfPupilsEligibleForFreeSchoolMeals,
            schoolPopulation.PupilsEligibleForFreeSchoolMealsPercentage
        );
        SetCellToStatistic(
            ExportColumns.SchoolPopulationColumns.PercentageOfOverallAbsence,
            attendance.OverallAbsencePercentage
        );
        SetCellToStatistic(
            ExportColumns.SchoolPopulationColumns.PercentageOfEnrolmentsWhoArePersistentAbsentees,
            attendance.EnrolmentsWhoArePersistentAbsenteesPercentage
        );

        CurrentRow++;
    }

    private void SetCellToStatistic(ExportColumns.SchoolPopulationColumns column, Statistic<int> statistic)
    {
        if (statistic is Statistic<int>.WithValue value)
        {
            SetNumberCell(column, value.Value);
            return;
        }
        
        SetTextCell(column, statistic.DisplayValue());
    }

    private void SetCellToStatistic(ExportColumns.SchoolPopulationColumns column, Statistic<decimal> statistic)
    {
        if (statistic is Statistic<decimal>.WithValue value)
        {
            SetNumberCell(column, value.Value / 100.0m, "0.0%");
            return;
        }
        
        SetTextCell(column, statistic.DisplayValue());
    }
}
