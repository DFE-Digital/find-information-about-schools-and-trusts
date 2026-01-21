using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Export;
using DfE.FindInformationAcademiesTrusts.Services.Ofsted;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Schools.Ofsted.ReportCards
{
    public abstract class ReportCardsAreaModel(
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
        public const string SubPageName = "Report Cards";

        public ReportCardServiceModel ReportCards = null!;

        public override PageMetadata PageMetadata => base.PageMetadata with { SubPageName = SubPageName };

        public override async Task<IActionResult> OnGetAsync()
        {
            var pageResult = await base.OnGetAsync();
            if (pageResult is NotFoundResult) return pageResult;

            ReportCards = await reportCardsService.GetReportCardsAsync(Urn);

            TabList =
            [
                GetTabFor<CurrentModel>(SubPageName, "Current report card", "./Current"),
                GetTabFor<PreviousModel>(SubPageName, "Previous report card", "./Previous")
            ];

            return pageResult;
        }
    }
}
