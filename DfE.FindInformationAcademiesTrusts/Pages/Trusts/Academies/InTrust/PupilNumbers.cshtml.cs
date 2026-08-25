using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Services.Academy;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Export;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Trusts.Academies.InTrust;

public class PupilNumbersModel(
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
    public const string TabName = "Pupil numbers";

    public override PageMetadata PageMetadata => base.PageMetadata with { TabName = TabName };

    public AcademyPupilNumbersServiceModel[] Academies { get; set; } = default!;

    public override async Task<IActionResult> OnGetAsync()
    {
        var pageResult = await base.OnGetAsync();
        if (pageResult is NotFoundResult) return pageResult;

        Academies = await AcademyService.GetAcademiesInTrustPupilNumbersAsync(Uid,ReferenceNumber);

        return pageResult;
    }

    public static string PhaseAndAgeRangeSortValue(AcademyPupilNumbersServiceModel academy)
    {
        return $"{academy.PhaseOfEducation}{academy.AgeRange.ToDataSortValue()}";
    }
}
