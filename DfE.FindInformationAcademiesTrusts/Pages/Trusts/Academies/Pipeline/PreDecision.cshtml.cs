using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Services.Academy;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Export;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Trusts.Academies.Pipeline;

public class PreDecisionModel(
    IDataSourceService dataSourceService,
    ITrustService trustService,
    IAcademyService academyService,
    IPipelineAcademiesExportService pipelineAcademiesExportService,
    IDateTimeProvider dateTimeProvider)
    : PipelineAcademiesAreaModel(dataSourceService, trustService, academyService, pipelineAcademiesExportService,
        dateTimeProvider)
{
    public const string TabName = "Pre decision";

    public override PageMetadata PageMetadata => base.PageMetadata with { TabName = TabName };

    public AcademyPipelineServiceModel[] PreAdvisoryPipelineEstablishments { get; set; } = [];

    public override async Task<IActionResult> OnGetAsync()
    {
        var pageResult = await base.OnGetAsync();

        if (pageResult.GetType() == typeof(NotFoundResult)) return pageResult;

        PreAdvisoryPipelineEstablishments =
            await AcademyService.GetAcademiesPipelinePreAdvisoryAsync(TrustReferenceNumber);

        return pageResult;
    }
}
