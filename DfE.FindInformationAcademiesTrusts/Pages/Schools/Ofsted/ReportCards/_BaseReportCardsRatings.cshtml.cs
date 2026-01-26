using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.Pages.Schools.Ofsted.Older;
using DfE.FindInformationAcademiesTrusts.Services.Academy;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Export;
using DfE.FindInformationAcademiesTrusts.Services.Ofsted;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Schools.Ofsted.ReportCards;

public abstract class BaseReportCardsRatingsModel(
    ISchoolService schoolService,
    ISchoolOverviewDetailsService schoolOverviewDetailsService,
    ITrustService trustService,
    IDataSourceService dataSourceService,
    IOfstedSchoolDataExportService ofstedSchoolDataExportService,
    IDateTimeProvider dateTimeProvider,
    IOtherServicesLinkBuilder otherServicesLinkBuilder,
    ISchoolNavMenu schoolNavMenu, 
    IReportCardsService reportCardsService) : OfstedAreaModel(schoolService, schoolOverviewDetailsService, trustService,
    dataSourceService, ofstedSchoolDataExportService, dateTimeProvider, otherServicesLinkBuilder, schoolNavMenu)
{

    public ReportCardDetails? ReportCard { get; set; }

    public BeforeOrAfterJoining WhenDidCurrentInspectionHappen { get; private set; }

    public override async Task<IActionResult> OnGetAsync()
    {
        var pageResult = await base.OnGetAsync();

        if (pageResult is NotFoundResult) return pageResult;

        var reportCards = await reportCardsService.GetReportCardsAsync(Urn);
        ReportCard = GetReportCard(reportCards);

        WhenDidCurrentInspectionHappen = GetWhenInspectionHappened(ReportCard, reportCards.DateJoinedTrust?.ToDateTime(TimeOnly.MinValue));

        TabList =
        [
            GetTabFor<CurrentReportCardsModel>("Report cards", "Current report card", "./CurrentReportCards"),
            GetTabFor<PreviousReportCardsModel>("Report cards", "Previous report card", "./PreviousReportCards")
        ];

        return pageResult;
    }

    protected abstract ReportCardDetails? GetReportCard(ReportCardServiceModel reportCardServiceModel);

    protected abstract BeforeOrAfterJoining GetWhenInspectionHappened(ReportCardDetails? reportCardDetails, DateTime? dateJoinedTrust);
}
