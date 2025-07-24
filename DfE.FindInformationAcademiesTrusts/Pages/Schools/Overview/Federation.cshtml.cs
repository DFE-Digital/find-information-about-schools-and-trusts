using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Schools.Overview;

public class FederationModel(
    ISchoolService schoolService,
    ITrustService trustService,
    ISchoolOverviewFederationService schoolOverviewFederationService,
    IDataSourceService dataSourceService,
    ISchoolNavMenu schoolNavMenu) : OverviewAreaModel(schoolService, trustService, dataSourceService, schoolNavMenu)
{
    public override PageMetadata PageMetadata => base.PageMetadata with
    {
        SubPageName = SubPageName
    };

    public const string SubPageName = "Federation details";

    public SchoolOverviewFederationServiceModel SchoolOverviewFederationServiceModel { get; private set; } = null!;

    public string FederationName { get; set; } = string.Empty;
    public string FederationUid { get; set; } = string.Empty;
    public DateOnly? OpenedOnDate { get; set; }
    public Dictionary<string, string> Schools { get; set; } = [];

    public static readonly string NotAvailable = "Not available";

    public override async Task<IActionResult> OnGetAsync()
    {
        var pageResult = await base.OnGetAsync();
        if (pageResult is NotFoundResult) return pageResult;

        if (SchoolCategory == SchoolCategory.Academy)
        {
            return new NotFoundResult();
        }

        if (!IsPartOfAFederation)
        {
            return new NotFoundResult();
        }

        SchoolOverviewFederationServiceModel =
            await schoolOverviewFederationService.GetSchoolOverviewFederationAsync(Urn);

        FederationName = SchoolOverviewFederationServiceModel.FederationName ?? NotAvailable;
        FederationUid = SchoolOverviewFederationServiceModel.FederationUid ?? NotAvailable;
        OpenedOnDate = SchoolOverviewFederationServiceModel.OpenedOnDate;
        Schools = SchoolOverviewFederationServiceModel.Schools ?? new Dictionary<string, string>();

        return pageResult;
    }
}
