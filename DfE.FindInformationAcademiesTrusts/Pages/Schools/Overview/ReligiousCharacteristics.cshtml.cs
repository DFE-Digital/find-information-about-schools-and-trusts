using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Schools.Overview;

public class ReligiousCharacteristicsModel(
    ISchoolService schoolService,
    ITrustService trustService,
    IDataSourceService dataSourceService,
    ISchoolNavMenu schoolNavMenu) : OverviewAreaModel(schoolService, trustService, dataSourceService, schoolNavMenu)
{
    private readonly ISchoolService _schoolService = schoolService;

    public const string SubPageName = "Religious characteristics";

    public override PageMetadata PageMetadata =>
        base.PageMetadata with { SubPageName = SubPageName };

    public string ReligiousAuthority { get; set; } = null!;
    public string ReligiousCharacter { get; set; } = null!;
    public string ReligiousEthos { get; set; } = null!;

    public override async Task<IActionResult> OnGetAsync()
    {
        var pageResult = await base.OnGetAsync();

        if (pageResult is NotFoundResult) return pageResult;

        var religiousCharacteristics = await _schoolService.GetReligiousCharacteristicsAsync(Urn);

        ReligiousAuthority = religiousCharacteristics.ReligiousAuthority;
        ReligiousCharacter = religiousCharacteristics.ReligiousCharacter;
        ReligiousEthos = religiousCharacteristics.ReligiousEthos;

        return pageResult;
    }
}
