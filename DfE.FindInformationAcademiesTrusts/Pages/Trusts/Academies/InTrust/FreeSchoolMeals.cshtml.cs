using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Services.Academy;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Export;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Trusts.Academies.InTrust;

public class FreeSchoolMealsModel(
    IDataSourceService dataSourceService,
    ITrustService trustService,
    IAcademyService academyService,
    IAcademiesExportService academiesExportService,
    IDateTimeProvider dateTimeProvider
) : AcademiesInTrustAreaModel(
    dataSourceService,
    trustService,
    academyService,
    academiesExportService,
    dateTimeProvider
)
{
    public const string TabName = "Free school meals";

    public override PageMetadata PageMetadata => base.PageMetadata with { TabName = TabName };

    public AcademyFreeSchoolMealsServiceModel[] Academies { get; set; } = default!;

    public override async Task<IActionResult> OnGetAsync()
    {
        var pageResult = await base.OnGetAsync();
        if (pageResult is NotFoundResult) return pageResult;

        Academies = await AcademyService.GetAcademiesInTrustFreeSchoolMealsAsync(ReferenceNumber);


        return pageResult;
    }
}
