using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Ofsted;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Trusts.Ofsted.ReportCards;

public abstract class BaseReportCardsRatingsModel(
    IDataSourceService dataSourceService,
    ITrustService trustService,
    IOfstedService ofstedService,
    IPowerBiLinkBuilderService powerBiLinkBuilderService)
    : OfstedAreaModel(dataSourceService, trustService)
{
    public List<ReportCardViewModel> ReportCards { get; set; } = [];

    public override async Task<IActionResult> OnGetAsync()
    {
        var pageResult = await base.OnGetAsync();

        if (pageResult is NotFoundResult) return pageResult;

        var reportCardServiceModels = await ofstedService.GetEstablishmentsInTrustReportCardsAsync(Uid);

        ReportCards = GetReportCard(reportCardServiceModels);

        TabList = TrustNavMenu.GetTabLinksForReportCardsOfstedPage(this);

        PowerBiLink = powerBiLinkBuilderService.BuildReportCardsLinkForTrust(TrustReferenceNumber);

        return pageResult;
    }

    protected abstract List<ReportCardViewModel> GetReportCard(
        List<TrustOfstedReportServiceModel<ReportCardServiceModel>> reportCardServiceModel);
}