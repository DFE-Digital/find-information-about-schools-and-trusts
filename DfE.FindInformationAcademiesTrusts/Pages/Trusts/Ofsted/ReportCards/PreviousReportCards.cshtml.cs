using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Ofsted;
using DfE.FindInformationAcademiesTrusts.Services.Trust;

namespace DfE.FindInformationAcademiesTrusts.Pages.Trusts.Ofsted.ReportCards
{
    public class PreviousReportCardsModel(
        IDataSourceService dataSourceService,
        ITrustService trustService,
        IOfstedService ofstedService,
        IPowerBiLinkBuilderService powerBiLinkBuilderService) : BaseReportCardsRatingsModel(dataSourceService, trustService, ofstedService, powerBiLinkBuilderService)
    {
        public const string SubPageName = "Report cards";

        public override PageMetadata PageMetadata => base.PageMetadata with
        {
            SubPageName = SubPageName, TabName = "Previous report card"
        };

        protected override List<ReportCardViewModel> GetReportCard(
            List<TrustOfstedReportServiceModel<ReportCardServiceModel>> reportCardServiceModel)
        {
            return reportCardServiceModel.Select(x => new ReportCardViewModel(
                x.Urn,
                x.SchoolName,
                x.ReportDetails?.PreviousReportCard
            )).ToList();
        }
    }
}
