using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Export;
using DfE.FindInformationAcademiesTrusts.Services.Ofsted;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;

namespace DfE.FindInformationAcademiesTrusts.Pages.Schools.Ofsted.ReportCards
{
    public class CurrentReportCardsModel(
        ISchoolService schoolService,
        ISchoolOverviewDetailsService schoolOverviewDetailsService,
        ITrustService trustService,
        IDataSourceService dataSourceService,
        IOfstedSchoolDataExportService ofstedSchoolDataExportService,
        IDateTimeProvider dateTimeProvider,
        IOtherServicesLinkBuilder otherServicesLinkBuilder,
        ISchoolNavMenu schoolNavMenu,
        IReportCardsService reportCardsService) : BaseReportCardsRatingsModel(schoolService,
        schoolOverviewDetailsService, trustService,
        dataSourceService, ofstedSchoolDataExportService, dateTimeProvider, otherServicesLinkBuilder, schoolNavMenu,
        reportCardsService)
    {
        public const string SubPageName = "Report cards";
        public override PageMetadata PageMetadata => base.PageMetadata with { SubPageName = SubPageName, TabName = "Current report card" };

        protected override ReportCardDetails? GetReportCard(ReportCardServiceModel reportCardServiceModel) => reportCardServiceModel.LatestReportCard;

        protected override BeforeOrAfterJoining GetWhenInspectionHappened(ReportCardDetails? reportCardDetails, DateTime? dateJoinedTrust) => dateJoinedTrust.GetBeforeOrAfterJoiningTrust(reportCardDetails?.InspectionDate);
    }

}
