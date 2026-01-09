using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Export;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Schools.Ofsted;

public class SingleHeadlineGradesModel(
    ISchoolService schoolService,
    ISchoolOverviewDetailsService schoolOverviewDetailsService,
    ITrustService trustService,
    IDataSourceService dataSourceService,
    IOfstedSchoolDataExportService ofstedSchoolDataExportService,
    IDateTimeProvider dateTimeProvider,
    IOtherServicesLinkBuilder otherServicesLinkBuilder,
    ISchoolNavMenu schoolNavMenu) : OfstedAreaModel(schoolService, schoolOverviewDetailsService, trustService,
    dataSourceService, ofstedSchoolDataExportService, dateTimeProvider, otherServicesLinkBuilder, schoolNavMenu)
{
    public const string SubPageName = "Single headline grades";
    public override PageMetadata PageMetadata => base.PageMetadata with { SubPageName = SubPageName };
    public OfstedHeadlineGradesServiceModel HeadlineGrades { get; set; } = null!;

    public override async Task<IActionResult> OnGetAsync()
    {
        var pageResult = await base.OnGetAsync();
        if (pageResult is NotFoundResult) return pageResult;

        HeadlineGrades = await SchoolService.GetOfstedHeadlineGrades(Urn);

        return pageResult;
    }

    public BeforeOrAfterJoining GetBeforeOrAfterJoining(DateTime? inspectionDate)
    {
        if (DateJoinedTrust is null || inspectionDate is null)
        {
            return BeforeOrAfterJoining.NotApplicable;
        }

        return inspectionDate < DateJoinedTrust
            ? BeforeOrAfterJoining.Before
            : BeforeOrAfterJoining.After;
    }

    public string GetScreenReaderText(DateTime? inspectionDate)
    {
        var beforeOrAfter = GetBeforeOrAfterJoining(inspectionDate);
        return beforeOrAfter switch
        {
            BeforeOrAfterJoining.Before => "Inspected before joining the trust",
            BeforeOrAfterJoining.After => "Inspected after joining the trust",
            _ => ""
        };
    }
}
