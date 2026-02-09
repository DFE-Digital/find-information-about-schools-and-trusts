using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Services.Academy;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Export;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Trusts.Ofsted.ReportCards;

public abstract class BaseReportCardsRatingsModel(IDataSourceService dataSourceService,
    ITrustService trustService,
    IAcademyService academyService,
    IOfstedTrustDataExportService ofstedTrustDataExportService,
    IDateTimeProvider dateTimeProvider) : OfstedAreaModel(dataSourceService, trustService,
    academyService, ofstedTrustDataExportService, dateTimeProvider)
{
    public override async Task<IActionResult> OnGetAsync()
    {
        var pageResult = await base.OnGetAsync();

        TabList = TrustNavMenu.GetTabLinksForReportCardsOfstedPage(this);

        return pageResult;
    }
}
