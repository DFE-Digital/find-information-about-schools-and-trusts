using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
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
        IOtherServicesLinkBuilder otherServicesLinkBuilder,
        ISchoolNavMenu schoolNavMenu,
        IReportCardsService reportCardsService,
        IPowerBiLinkBuilderService powerBiLinkBuilderService) : BaseReportCardsRatingsModel(schoolService,
        schoolOverviewDetailsService, trustService, dataSourceService, otherServicesLinkBuilder, schoolNavMenu,
        reportCardsService, powerBiLinkBuilderService)
    {
        public const string SubPageName = "Report cards";

        public override PageMetadata PageMetadata => base.PageMetadata with
        {
            SubPageName = SubPageName, TabName = "Current report card"
        };

        protected override ReportCardDetails? GetReportCard(ReportCardServiceModel reportCardServiceModel) => reportCardServiceModel.LatestReportCard;

        protected override BeforeOrAfterJoining GetWhenInspectionHappened(ReportCardDetails? reportCardDetails, DateOnly? dateJoinedTrust) => dateJoinedTrust.GetBeforeOrAfterJoiningTrust(reportCardDetails?.InspectionDate);
    }

}
