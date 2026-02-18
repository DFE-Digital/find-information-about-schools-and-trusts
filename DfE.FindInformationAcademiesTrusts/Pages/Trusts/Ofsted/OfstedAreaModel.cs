using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.NavMenu;
using DfE.FindInformationAcademiesTrusts.Pages.Trusts.Ofsted.ReportCards;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Export;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Trusts.Ofsted;

public class OfstedAreaModel(
    IDataSourceService dataSourceService,
    ITrustService trustService,
    IOfstedTrustDataExportService ofstedTrustDataExportService,
    IDateTimeProvider dateTimeProvider)
    : TrustsAreaModel(dataSourceService, trustService)
{
    public const string PageName = "Ofsted";

    public override PageMetadata PageMetadata => base.PageMetadata with { PageName = PageName };

    public IDateTimeProvider DateTimeProvider { get; } = dateTimeProvider;

    public NavLink[] TabList { get; set; } = [];

    public override async Task<IActionResult> OnGetAsync()
    {
        var pageResult = await base.OnGetAsync();

        if (pageResult.GetType() == typeof(NotFoundResult)) return pageResult;
        
        // Add data sources
        var giasDataSource = await DataSourceService.GetAsync(Source.Gias);
        var misDataSource = await DataSourceService.GetAsync(Source.Mis);

        DataSourcesPerPage.AddRange([
            new DataSourcePageListEntry(OverviewModel.SubPageName, [
                    new DataSourceListEntry(giasDataSource, "Date joined trust"),
                    new DataSourceListEntry(misDataSource, "All inspection types"),
                    new DataSourceListEntry(misDataSource, "All inspection dates")
                ]
            ),
            new DataSourcePageListEntry(CurrentReportCardsModel.SubPageName, [
                    new DataSourceListEntry(misDataSource, "Current report card ratings"),
                    new DataSourceListEntry(misDataSource, "Previous report card ratings")
                ]
            ),
            new DataSourcePageListEntry(OlderInspectionsModel.SubPageName, [
                    new DataSourceListEntry(misDataSource, "Inspection ratings after September 24"),
                    new DataSourceListEntry(misDataSource, "Inspection ratings before September 24")
                ]
            ),
            new DataSourcePageListEntry(SafeguardingAndConcernsModel.SubPageName, [
                    new DataSourceListEntry(misDataSource, "Effective safeguarding and category of concern")
                ]
            )
        ]);

        return pageResult;
    }

    public virtual async Task<IActionResult> OnGetExportAsync(string uid)
    {
        var trustSummary = await TrustService.GetTrustSummaryAsync(uid);

        if (trustSummary == null)
        {
            return new NotFoundResult();
        }

        // Sanitize the trust name to remove any illegal characters
        var sanitizedTrustName =
            string.Concat(trustSummary.Name.Where(c => !Path.GetInvalidFileNameChars().Contains(c)));

        var fileContents = await ofstedTrustDataExportService.BuildAsync(uid);
        var fileName = $"Ofsted-{sanitizedTrustName}-{DateTimeProvider.Now:yyyy-MM-dd}.xlsx";
        var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        return File(fileContents, contentType, fileName);
    }
}
