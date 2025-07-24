using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.NavMenu;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Schools;

public class SchoolAreaModel(
    ISchoolService schoolService,
    ITrustService trustService,
    ISchoolNavMenu schoolNavMenu) : BasePageModel, ISchoolAreaModel
{
    [BindProperty(SupportsGet = true)] public int Urn { get; set; }

    public List<DataSourcePageListEntry> DataSourcesPerPage { get; set; } = [];
    public virtual PageMetadata PageMetadata => new(SchoolName, ModelState.IsValid);

    public SchoolSummaryServiceModel SchoolSummary { get; set; } = null!;
    public string SchoolName => SchoolSummary.Name;
    public string SchoolType => SchoolSummary.Type;
    public SchoolCategory SchoolCategory => SchoolSummary.Category;
    public bool IsPartOfAFederation { get; set; }

    public TrustSummaryServiceModel? TrustSummary { get; private set; }
    public NavLink[] ServiceNavLinks { get; set; } = null!;
    public NavLink[] SubNavLinks { get; set; } = null!;

    public virtual async Task<IActionResult> OnGetAsync()
    {
        var schoolSummary = await schoolService.GetSchoolSummaryAsync(Urn);

        if (schoolSummary == null)
        {
            return new NotFoundResult();
        }

        TrustSummary = await trustService.GetTrustSummaryAsync(schoolSummary.Urn);

        IsPartOfAFederation = await schoolService.IsPartOfFederationAsync(schoolSummary.Urn);

        SchoolSummary = schoolSummary;

        ServiceNavLinks = await schoolNavMenu.GetServiceNavLinksAsync(this);
        SubNavLinks = await schoolNavMenu.GetSubNavLinksAsync(this);

        return Page();
    }
}
