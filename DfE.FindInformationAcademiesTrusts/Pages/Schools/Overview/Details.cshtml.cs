using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Schools.Overview;

public class DetailsModel(
    ISchoolService schoolService,
    ITrustService trustService,
    ISchoolOverviewDetailsService schoolOverviewDetailsService,
    IOtherServicesLinkBuilder otherServicesLinkBuilder,
    IDataSourceService dataSourceService,
    ISchoolNavMenu schoolNavMenu) : OverviewAreaModel(schoolService, trustService, dataSourceService, schoolNavMenu)
{
    public override PageMetadata PageMetadata => base.PageMetadata with
    {
        SubPageName = SubPageName(SchoolCategory)
    };

    public static string SubPageName(SchoolCategory schoolCategory)
    {
        return schoolCategory switch
        {
            SchoolCategory.LaMaintainedSchool => "School details",
            SchoolCategory.Academy => "Academy details",
            _ => throw new ArgumentOutOfRangeException(nameof(schoolCategory))
        };
    }

    public SchoolOverviewServiceModel SchoolOverviewModel { get; private set; } = null!;

    public string GetInformationAboutSchoolsLink { get; private set; } = null!;
    public string FinancialBenchmarkingInsightsToolLink { get; private set; } = null!;
    public string FindSchoolPerformanceLink { get; private set; } = null!;

    public bool TrustInformationIsAvailable { get; private set; } = true;
    public bool TrustSummaryIsAvailable { get; private set; }

    public override async Task<IActionResult> OnGetAsync()
    {
        var pageResult = await base.OnGetAsync();
        if (pageResult is NotFoundResult) return pageResult;

        SchoolOverviewModel = await schoolOverviewDetailsService.GetSchoolOverviewDetailsAsync(Urn, SchoolCategory);

        GetInformationAboutSchoolsLink =
            otherServicesLinkBuilder.GetInformationAboutSchoolsListingLinkForSchool(Urn.ToString());
        FinancialBenchmarkingInsightsToolLink = otherServicesLinkBuilder.FinancialBenchmarkingLinkForSchool(Urn);
        FindSchoolPerformanceLink = otherServicesLinkBuilder.FindSchoolPerformanceDataListingLink(Urn);

        TrustSummaryIsAvailable = TrustSummary is not null;
        TrustInformationIsAvailable = SchoolOverviewModel.DateJoinedTrust is not null && TrustSummary is not null;

        return pageResult;
    }
}
