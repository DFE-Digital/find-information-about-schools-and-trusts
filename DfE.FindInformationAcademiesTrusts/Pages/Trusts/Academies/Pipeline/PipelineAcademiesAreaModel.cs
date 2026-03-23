using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Academy;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Export;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Trusts.Academies.Pipeline;

public abstract class PipelineAcademiesAreaModel(
    IDataSourceService dataSourceService,
    ITrustService trustService,
    IAcademyService academyService,
    IPipelineAcademiesExportService pipelineAcademiesExportService,
    IDateTimeProvider dateTimeProvider
) : AcademiesAreaModel(
    dataSourceService,
    trustService,
    academyService,
    dateTimeProvider
)
{
    public const string SubPageName = "Pipeline academies";

    public override PageMetadata PageMetadata => base.PageMetadata with { SubPageName = SubPageName };

    public override async Task<IActionResult> OnGetAsync()
    {
        var pageResult = await base.OnGetAsync();

        if (pageResult.GetType() == typeof(NotFoundResult)) return pageResult;

        TabList =
        [
            GetTabFor<PreDecisionModel>("Pipeline",
                $"{PreDecisionModel.TabName} ({PipelineSummary.PreAdvisoryCount})",
                "./PreDecision"),
            GetTabFor<PostDecisionModel>("Pipeline",
                $"{PostDecisionModel.TabName} ({PipelineSummary.PostAdvisoryCount})",
                "./PostDecision"),
            GetTabFor<FreeSchoolsModel>("Pipeline", $"{FreeSchoolsModel.TabName} ({PipelineSummary.FreeSchoolsCount})",
                "./FreeSchools")
        ];

        var prepareSource = await DataSourceService.GetAsync(Source.Prepare);
        var completeSource = await DataSourceService.GetAsync(Source.Complete);
        var manageFreeSchoolSource = await DataSourceService.GetAsync(Source.ManageFreeSchoolProjects);
        DataSourcesPerPage.AddRange([
            new DataSourcePageListEntry(PreDecisionModel.TabName, [new DataSourceListEntry(prepareSource)]),
            new DataSourcePageListEntry(PostDecisionModel.TabName, [new DataSourceListEntry(completeSource)]),
            new DataSourcePageListEntry(FreeSchoolsModel.TabName, [new DataSourceListEntry(manageFreeSchoolSource)])
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

        var fileContents = await pipelineAcademiesExportService.BuildAsync(uid);
        var fileName = $"pipeline-{sanitizedTrustName}-{DateTimeProvider.Now:yyyy-MM-dd}.xlsx";
        var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        return File(fileContents, contentType, fileName);
    }
}
