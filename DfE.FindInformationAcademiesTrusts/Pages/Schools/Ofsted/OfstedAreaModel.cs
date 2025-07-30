using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Export;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Schools.Ofsted;

public class OfstedAreaModel(
    ISchoolService schoolService,
    ISchoolOverviewDetailsService schoolOverviewDetailsService,
    ITrustService trustService,
    IDataSourceService dataSourceService,
    IOfstedSchoolDataExportService ofstedSchoolDataExportService,
    IDateTimeProvider dateTimeProvider,
    IOtherServicesLinkBuilder otherServicesLinkBuilder,
    ISchoolNavMenu schoolNavMenu)
    : SchoolAreaModel(schoolService, trustService, schoolNavMenu)
{
    private readonly ISchoolService _schoolService = schoolService;

    public const string PageName = "Ofsted";
    public override PageMetadata PageMetadata => base.PageMetadata with { PageName = PageName };

    public DateTime? DateJoinedTrust { get; set; }
    public string OfstedReportUrl { get; set; } = null!;

    public override async Task<IActionResult> OnGetAsync()
    {
        var pageResult = await base.OnGetAsync();
        if (pageResult is NotFoundResult) return pageResult;

        var schoolOverview = await schoolOverviewDetailsService.GetSchoolOverviewDetailsAsync(Urn, SchoolCategory);
        if (schoolOverview.DateJoinedTrust is not null)
        {
            DateJoinedTrust = schoolOverview.DateJoinedTrust.Value.ToDateTime(new TimeOnly());
        }

        OfstedReportUrl = otherServicesLinkBuilder.OfstedReportLinkForSchool(Urn);

        List<DataSourceServiceModel> dataSources =
        [
            await dataSourceService.GetAsync(Source.Mis),
            await dataSourceService.GetAsync(Source.MisFurtherEducation)
        ];

        DataSourcesPerPage =
        [
            new DataSourcePageListEntry(SingleHeadlineGradesModel.SubPageName, [
                new DataSourceListEntry(dataSources, "Single headline grades were"),
                new DataSourceListEntry(dataSources, "All inspection dates were")
            ])
        ];

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

        var fileContents = await ofstedSchoolDataExportService.BuildAsync(urn);
        var fileName = $"Ofsted-{sanitisedSchoolName}-{dateTimeProvider.Now:yyyy-MM-dd}.xlsx";
        var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        return File(fileContents, contentType, fileName);
    }
}
