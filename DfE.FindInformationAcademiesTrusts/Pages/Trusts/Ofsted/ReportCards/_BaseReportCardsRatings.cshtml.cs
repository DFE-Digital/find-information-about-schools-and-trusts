using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Services.Academy;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Export;
using DfE.FindInformationAcademiesTrusts.Services.Ofsted;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Trusts.Ofsted.ReportCards;

public abstract class BaseReportCardsRatingsModel(IDataSourceService dataSourceService,
    ITrustService trustService,
    IAcademyService academyService,
    IOfstedTrustDataExportService ofstedTrustDataExportService,
    IDateTimeProvider dateTimeProvider,
    IOfstedService ofstedService) : OfstedAreaModel(dataSourceService, trustService,
    academyService, ofstedTrustDataExportService, dateTimeProvider)
{
    public List<ReportCardViewModel> ReportCards { get; set; } = [];

    public override async Task<IActionResult> OnGetAsync()
    {
        var pageResult = await base.OnGetAsync();

        if (pageResult is NotFoundResult) return pageResult;

        var reportCardServiceModels = await ofstedService.GetEstablishmentsInTrustReportCardsAsync(Uid);

        ReportCards = GetReportCard(reportCardServiceModels);

        TabList = TrustNavMenu.GetTabLinksForReportCardsOfstedPage(this);

        return pageResult;
    }

    protected abstract List<ReportCardViewModel> GetReportCard(List<TrustReportCardServiceModel> reportCardServiceModel);
}
