using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.PupilCensus;
using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Schools.Pupils;

public class AttendanceModel(
    IDateTimeProvider dateTimeProvider,
    ISchoolPupilService schoolPupilService,
    IDataSourceService dataSourceService,
    ISchoolService schoolService,
    ITrustService trustService,
    ISchoolNavMenu schoolNavMenu
) : PupilsAreaModel(dataSourceService, schoolService, trustService, schoolNavMenu)
{
    public const string SubPageName = "Attendance";
    public override PageMetadata PageMetadata => base.PageMetadata with { SubPageName = SubPageName };

    public List<AttendanceDataViewModel> AttendanceData { get; set; } = null!;

    public override async Task<IActionResult> OnGetAsync()
    {
        var pageResult = await base.OnGetAsync();
        if (pageResult is NotFoundResult) return pageResult;

        var statistics = await schoolPupilService.GetAttendanceStatisticsAsync(
            Urn,
            CensusYear.Previous(dateTimeProvider, Census.Autumn, 3),
            CensusYear.Next(dateTimeProvider, Census.Autumn)
        );

        AttendanceData = statistics.OrderByDescending(kvp => kvp.Key.Value)
            .Select(kvp => AttendanceDataViewModel.FromAttendance(kvp.Key, kvp.Value))
            .ToList();

        return pageResult;
    }
}

public record AttendanceDataViewModel(
    CensusYear Year,
    string PercentageOfOverallAbsenceDisplay,
    string PercentageOfOverallAbsenceSort,
    string PercentageOfEnrolmentsWhoArePersistentAbsenteesDisplay,
    string PercentageOfEnrolmentsWhoArePersistentAbsenteesSort
)
{
    public static AttendanceDataViewModel FromAttendance(CensusYear censusYear, Attendance attendance)
    {
        return new AttendanceDataViewModel(
            censusYear,
            attendance.OverallAbsencePercentage.Compute(percentage => $"{percentage}%").DisplayValue(),
            attendance.OverallAbsencePercentage.SortValue(),
            attendance.EnrolmentsWhoArePersistentAbsenteesPercentage.Compute(percentage => $"{percentage}%").DisplayValue(),
            attendance.EnrolmentsWhoArePersistentAbsenteesPercentage.SortValue()
        );
    }
}
