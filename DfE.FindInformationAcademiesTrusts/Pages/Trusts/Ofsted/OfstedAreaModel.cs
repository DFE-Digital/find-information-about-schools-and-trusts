using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Academy;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Export;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Trusts.Ofsted;

public class OfstedAreaModel(
    IDataSourceService dataSourceService,
    ITrustService trustService,
    IAcademyService academyService,
    IOfstedTrustDataExportService ofstedTrustDataExportService,
    IDateTimeProvider dateTimeProvider)
    : TrustsAreaModel(dataSourceService, trustService)
{
    public const string PageName = "Ofsted";

    public override PageMetadata PageMetadata => base.PageMetadata with { PageName = PageName };

    public AcademyOfstedServiceModel[] Academies { get; set; } = default!;
    private IAcademyService AcademyService { get; } = academyService;
    public IDateTimeProvider DateTimeProvider { get; } = dateTimeProvider;

    public override async Task<IActionResult> OnGetAsync()
    {
        var pageResult = await base.OnGetAsync();

        if (pageResult.GetType() == typeof(NotFoundResult)) return pageResult;

        Academies = await AcademyService.GetAcademiesInTrustOfstedAsync(Uid);

        // Add data sources
        var giasDataSource = await DataSourceService.GetAsync(Source.Gias);
        var misDataSource = await DataSourceService.GetAsync(Source.Mis);

        DataSourcesPerPage.AddRange([
            new DataSourcePageListEntry(SingleHeadlineGradesModel.SubPageName, [
                    new DataSourceListEntry(giasDataSource, "Date joined trust"),
                    new DataSourceListEntry(misDataSource, "All single headline grades"),
                    new DataSourceListEntry(misDataSource, "All inspection dates")
                ]
            ),
            new DataSourcePageListEntry(CurrentRatingsModel.SubPageName, [
                    new DataSourceListEntry(misDataSource, "Current Ofsted rating"),
                    new DataSourceListEntry(misDataSource, "Date of current inspection")
                ]
            ),
            new DataSourcePageListEntry(PreviousRatingsModel.SubPageName, [
                    new DataSourceListEntry(misDataSource, "Previous Ofsted rating"),
                    new DataSourceListEntry(misDataSource, "Date of previous inspection")
                ]
            ),
            new DataSourcePageListEntry(SafeguardingAndConcernsModel.SubPageName, [
                    new DataSourceListEntry(misDataSource, "Effective safeguarding"),
                    new DataSourceListEntry(misDataSource, "Category of concern"),
                    new DataSourceListEntry(misDataSource, "Date of current inspection")
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
