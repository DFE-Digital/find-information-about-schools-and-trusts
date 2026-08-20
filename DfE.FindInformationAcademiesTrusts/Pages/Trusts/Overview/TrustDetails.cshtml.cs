using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Trusts.Overview;

public class TrustDetailsModel(
    IDataSourceService dataSourceService,
    ITrustService trustService,
    IOtherServicesLinkBuilder otherServicesLinkBuilder)
    : OverviewAreaModel(dataSourceService, trustService)
{
    public const string SubPageName = "Trust details";

    public override PageMetadata PageMetadata => base.PageMetadata with { SubPageName = SubPageName };

    public string? CompaniesHouseLink { get; set; }
    public string? GetInformationAboutSchoolsLink { get; set; }
    public string? FinancialBenchmarkingInsightsToolLink { get; set; }
    public string? FindSchoolPerformanceLink { get; set; }
    public string SharepointLink { get; set; } = string.Empty;

    public override async Task<IActionResult> OnGetAsync()
    {
        var pageResult = await base.OnGetAsync();
        if (pageResult is NotFoundResult) return pageResult;

        // Setup external links
        CompaniesHouseLink = otherServicesLinkBuilder.CompaniesHouseListingLink(TrustOverview.CompaniesHouseNumber);
        GetInformationAboutSchoolsLink =
            otherServicesLinkBuilder.GetInformationAboutSchoolsListingLinkForTrust(TrustOverview.Uid);
        FinancialBenchmarkingInsightsToolLink =
            otherServicesLinkBuilder.FinancialBenchmarkingInsightsToolListingLink(TrustOverview.CompaniesHouseNumber);
        FindSchoolPerformanceLink =
            otherServicesLinkBuilder.FindSchoolPerformanceDataListingLink(TrustOverview.Uid, TrustOverview.Type,
                TrustOverview.SingleAcademyTrustAcademyUrn);
        SharepointLink = otherServicesLinkBuilder.SharepointFolderLink(TrustOverview.TrustReferenceNumber);

        return Page();
    }
}
