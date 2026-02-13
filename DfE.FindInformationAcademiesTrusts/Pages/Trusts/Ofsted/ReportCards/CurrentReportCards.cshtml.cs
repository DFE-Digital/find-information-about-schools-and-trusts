using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Export;
using DfE.FindInformationAcademiesTrusts.Services.Ofsted;
using DfE.FindInformationAcademiesTrusts.Services.Trust;

namespace DfE.FindInformationAcademiesTrusts.Pages.Trusts.Ofsted.ReportCards
{
    public class CurrentReportCardsModel(
        IDataSourceService dataSourceService,
        ITrustService trustService,
        IOfstedTrustDataExportService ofstedTrustDataExportService,
        IDateTimeProvider dateTimeProvider,
        IOfstedService ofstedService) : BaseReportCardsRatingsModel(dataSourceService, trustService,
        ofstedTrustDataExportService, dateTimeProvider, ofstedService)
    {
        public const string SubPageName = "Report cards";

        public override PageMetadata PageMetadata => base.PageMetadata with
        {
            SubPageName = SubPageName, TabName = "Current report card"
        };

        protected override List<ReportCardViewModel> GetReportCard(
            List<TrustOfstedReportServiceModel<ReportCardServiceModel>> reportCardServiceModel)
        {
            return reportCardServiceModel.Select(x => new ReportCardViewModel(
                x.Urn,
                x.SchoolName,
                x.ReportDetails?.LatestReportCard
            )).ToList();
        }
    }
}