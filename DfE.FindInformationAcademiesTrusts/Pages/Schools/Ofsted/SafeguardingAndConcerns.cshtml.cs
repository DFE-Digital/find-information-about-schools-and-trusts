using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Export;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Schools.Ofsted;

public class SafeguardingAndConcernsModel(
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
    public const string SubPageName = "Safeguarding and concerns";
    public override PageMetadata PageMetadata => base.PageMetadata with { SubPageName = SubPageName };
    public OfstedRating OfstedRating { get; set; } = null!;
    public BeforeOrAfterJoining InspectionBeforeOrAfterJoiningTrust = BeforeOrAfterJoining.NotYetInspected;

    public override async Task<IActionResult> OnGetAsync()
    {
        var pageResult = await base.OnGetAsync();
        if (pageResult is NotFoundResult) return pageResult;

        var ofstedRatings = await SchoolService.GetSchoolOfstedRatingsAsync(Urn);

        InspectionBeforeOrAfterJoiningTrust = ofstedRatings.WhenDidCurrentInspectionHappen;
        OfstedRating = ofstedRatings.CurrentOfstedRating;

        return pageResult;
    }
}
