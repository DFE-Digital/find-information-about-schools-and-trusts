using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Schools.Overview;

public class ReferenceNumbersModel(
    ISchoolService schoolService,
    ITrustService trustService,
    IDataSourceService dataSourceService,
    ISchoolNavMenu schoolNavMenu) : OverviewAreaModel(schoolService, trustService, dataSourceService, schoolNavMenu)
{
    public const string SubPageName = "Reference numbers";

    public override PageMetadata PageMetadata =>
        base.PageMetadata with { SubPageName = SubPageName };

    private readonly ISchoolService _schoolService = schoolService;

    public string Laestab { get; set; } = null!;
    public string Ukprn { get; set; } = null!;

    public override async Task<IActionResult> OnGetAsync()
    {
        var pageResult = await base.OnGetAsync();

        if (pageResult is NotFoundResult) return pageResult;

        var referenceNumbers = await _schoolService.GetReferenceNumbersAsync(Urn);

        Laestab = referenceNumbers.Laestab ?? "Not available";
        Ukprn = referenceNumbers.Ukprn ?? "Not available";

        return pageResult;
    }
}
