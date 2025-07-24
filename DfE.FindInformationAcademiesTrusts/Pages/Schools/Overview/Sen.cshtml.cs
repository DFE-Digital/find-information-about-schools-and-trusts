using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Schools.Overview;

public class SenModel(
    ISchoolService schoolService,
    ITrustService trustService,
    ISchoolOverviewSenService schoolOverviewSenService,
    IDataSourceService dataSourceService,
    ISchoolNavMenu schoolNavMenu) : OverviewAreaModel(schoolService, trustService, dataSourceService, schoolNavMenu)
{
    public override PageMetadata PageMetadata => base.PageMetadata with
    {
        SubPageName = SubPageName
    };

    public const string SubPageName = "SEN (special educational needs)";

    public SchoolOverviewSenServiceModel SchoolOverviewSenServiceModel { get; private set; } = null!;

    public string? ResourcedProvisionOnRoll { get; set; }
    public string? ResourcedProvisionCapacity { get; set; }
    public string? SenOnRoll { get; set; }
    public string? SenCapacity { get; set; }
    public string? ResourcedProvisionType { get; set; }
    public List<string> SenProvisionTypes { get; set; } = new();

    private static readonly string NotAvailable = "Not available";

    public override async Task<IActionResult> OnGetAsync()
    {
        var pageResult = await base.OnGetAsync();
        if (pageResult is NotFoundResult) return pageResult;

        SchoolOverviewSenServiceModel = await schoolOverviewSenService.GetSchoolOverviewSenAsync(Urn);
        ResourcedProvisionOnRoll = SchoolOverviewSenServiceModel.ResourcedProvisionOnRoll ?? NotAvailable;
        ResourcedProvisionCapacity = SchoolOverviewSenServiceModel.ResourcedProvisionCapacity ?? NotAvailable;
        SenOnRoll = SchoolOverviewSenServiceModel.SenOnRoll ?? NotAvailable;
        SenCapacity = SchoolOverviewSenServiceModel.SenCapacity ?? NotAvailable;
        ResourcedProvisionType = SchoolOverviewSenServiceModel.ResourcedProvisionTypes ?? NotAvailable;
        SenProvisionTypes = SchoolOverviewSenServiceModel.SenProvisionTypes[0] != null
            ? SchoolOverviewSenServiceModel.SenProvisionTypes
            :
            [
                "Not available"
            ];

        return pageResult;
    }
}
