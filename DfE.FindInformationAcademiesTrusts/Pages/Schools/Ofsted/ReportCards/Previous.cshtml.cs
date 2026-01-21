using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Export;
using DfE.FindInformationAcademiesTrusts.Services.Ofsted;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Schools.Ofsted.ReportCards
{
    public class PreviousModel(
        ISchoolService schoolService,
        ISchoolOverviewDetailsService schoolOverviewDetailsService,
        ITrustService trustService,
        IDataSourceService dataSourceService,
        IOfstedSchoolDataExportService ofstedSchoolDataExportService,
        IDateTimeProvider dateTimeProvider,
        IOtherServicesLinkBuilder otherServicesLinkBuilder,
        ISchoolNavMenu schoolNavMenu,
        IReportCardsService reportCardsService) : ReportCardsAreaModel(schoolService, schoolOverviewDetailsService, trustService,
        dataSourceService, ofstedSchoolDataExportService, dateTimeProvider, otherServicesLinkBuilder, schoolNavMenu, reportCardsService)
    {
        public const string TabName = "Previous report card";

        public override PageMetadata PageMetadata => base.PageMetadata with { TabName = TabName };

        public ReportCardDetails? ReportCard;

        public BeforeOrAfterJoining WhenDidCurrentInspectionHappen { get; set; }

        public override async Task<IActionResult> OnGetAsync()
        {
            var pageResult = await base.OnGetAsync();
            if (pageResult is NotFoundResult) return pageResult;

            ReportCard = base.ReportCards.PreviousReportCard;

            WhenDidCurrentInspectionHappen = base.DateJoinedTrust.GetBeforeOrAfterJoiningTrust(ReportCard?.InspectionDate);

            return pageResult;
        }
    }
}
