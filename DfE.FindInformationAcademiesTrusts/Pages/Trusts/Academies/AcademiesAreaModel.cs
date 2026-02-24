using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.NavMenu;
using DfE.FindInformationAcademiesTrusts.Services.Academy;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Trusts.Academies;

public abstract class AcademiesAreaModel(
    IDataSourceService dataSourceService,
    ITrustService trustService,
    IAcademyService academyService,
    IDateTimeProvider dateTimeProvider
) : TrustsAreaModel(dataSourceService, trustService)
{
    public const string PageName = "Academies";
    public override PageMetadata PageMetadata => base.PageMetadata with { PageName = PageName };

    internal readonly IAcademyService AcademyService = academyService;
    public IDateTimeProvider DateTimeProvider { get; } = dateTimeProvider;

    public AcademyPipelineSummaryServiceModel PipelineSummary { get; set; } = null!;
    public NavLink[] TabList { get; set; } = null!;

    public override async Task<IActionResult> OnGetAsync()
    {
        var pageResult = await base.OnGetAsync();

        if (pageResult.GetType() == typeof(NotFoundResult)) return pageResult;
 
        PipelineSummary = await AcademyService.GetAcademiesPipelineSummaryAsync(TrustReferenceNumber);

        return pageResult;
    }

    public NavLink GetTabFor<T>(string subPageName, string linkDisplayText, string aspPage)
    {
        return new NavLink(this is T, "Academies", linkDisplayText, aspPage,
            $"{subPageName}-{linkDisplayText}-tab".Kebabify(),
            new Dictionary<string, string> { { "uid", Uid } });
    }
}
