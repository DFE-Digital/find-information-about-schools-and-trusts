using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Export;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;
using Source = DfE.FindInformationAcademiesTrusts.Data.Enums.Source;

namespace DfE.FindInformationAcademiesTrusts.Pages.Schools.Pupils;

public class PupilsAreaModel(
    IDateTimeProvider dateTimeProvider,
    IDataSourceService dataSourceService,
    ISchoolPupilsExportService schoolPupilsExportService,
    ISchoolService schoolService,
    ITrustService trustService,
    ISchoolNavMenu schoolNavMenu
) : SchoolAreaModel(schoolService, trustService, schoolNavMenu)
{
    private readonly ISchoolService _schoolService = schoolService;

    public const string PageName = "Pupils";

    public override PageMetadata PageMetadata => base.PageMetadata with { PageName = PageName };

    protected IDateTimeProvider DateTimeProvider => dateTimeProvider;

    public override async Task<IActionResult> OnGetAsync()
    {
        var pageResult = await base.OnGetAsync();
        if (pageResult is NotFoundResult) return pageResult;

        var populationDataSource = await dataSourceService.GetAsync(Source.CompareSchoolCollegePerformanceEnglandPopulation);
        var attendanceDataSource = await dataSourceService.GetAsync(Source.CompareSchoolCollegePerformanceEnglandAttendance);

        DataSourcesPerPage.AddRange([
            new DataSourcePageListEntry(PopulationModel.SubPageName, [new DataSourceListEntry(populationDataSource)]),
            new DataSourcePageListEntry(AttendanceModel.SubPageName, [new DataSourceListEntry(attendanceDataSource)]),
        ]);

        return Page();
    }

    public virtual async Task<IActionResult> OnGetExportAsync(int urn)
    {
        var schoolSummary = await _schoolService.GetSchoolSummaryAsync(urn);
        
        if (schoolSummary == null)
        {
            return new NotFoundResult();
        }
        
        var sanitisedSchoolName =
            string.Concat(schoolSummary.Name.Where(c => !Path.GetInvalidFileNameChars().Contains(c))).Trim();
        
        var fileContents = await schoolPupilsExportService.BuildAsync(urn);
        var fileName = $"Pupil population-{sanitisedSchoolName}-{dateTimeProvider.Now:yyyy-MM-dd}.xlsx";
        var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        
        return File(fileContents, contentType, fileName);
    }
}
