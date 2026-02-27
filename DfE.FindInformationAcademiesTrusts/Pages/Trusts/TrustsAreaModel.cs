using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Trusts;

public abstract class TrustsAreaModel(
    IDataSourceService dataSourceService,
    ITrustService trustService)
    : BasePageModel, ITrustsAreaModel
{
    protected readonly IDataSourceService DataSourceService = dataSourceService;
    protected readonly ITrustService TrustService = trustService;

    [BindProperty(SupportsGet = true)] public string Uid { get; set; } = "";
    public TrustSummaryServiceModel TrustSummary { get; set; } = default!;
    public string TrustReferenceNumber { get; set; } = "";

    public List<DataSourcePageListEntry> DataSourcesPerPage { get; set; } = [];
    public virtual PageMetadata PageMetadata => new(TrustSummary.Name, ModelState.IsValid);

    public virtual async Task<IActionResult> OnGetAsync()
    {
        var trustSummary = await TrustService.GetTrustSummaryAsync(Uid);

        if (trustSummary == null)
        {
            return new NotFoundResult();
        }

        TrustSummary = trustSummary;
        TrustReferenceNumber = await TrustService.GetTrustReferenceNumberAsync(TrustSummary.Uid);

        return Page();
    }
}
