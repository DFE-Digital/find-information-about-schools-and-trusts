using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.Services.Academy;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Export;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Schools.Ofsted.Older;

public abstract class OlderBaseRatingsModel(ISchoolService schoolService, ISchoolOverviewDetailsService schoolOverviewDetailsService, ITrustService trustService, IDataSourceService dataSourceService, IOfstedSchoolDataExportService ofstedSchoolDataExportService, IDateTimeProvider dateTimeProvider, IOtherServicesLinkBuilder otherServicesLinkBuilder, ISchoolNavMenu schoolNavMenu) : OfstedAreaModel(schoolService, schoolOverviewDetailsService, trustService, dataSourceService, ofstedSchoolDataExportService, dateTimeProvider, otherServicesLinkBuilder, schoolNavMenu)
{
    public OfstedRating? OfstedRating { get; set; }
    public string? SingleHeadlineRating { get; set; } = string.Empty;

    public BeforeOrAfterJoining InspectionBeforeOrAfterJoiningTrust { get; set; }

    public override async Task<IActionResult> OnGetAsync()
    {
        var pageResult = await base.OnGetAsync();
        
        if (pageResult is NotFoundResult) return pageResult;
        
        var ofstedRatings = await SchoolService.GetSchoolOfstedRatingsAsBeforeAndAfterSeptemberGradeAsync(Urn);
        OfstedRating = GetOfstedRating(ofstedRatings);
        InspectionBeforeOrAfterJoiningTrust = GetWhenInspectionHappened(ofstedRatings);
        SingleHeadlineRating = OfstedRating?.OverallEffectiveness.ToDisplayString(true);

        TabList =
        [
            GetTabFor<CurrentRatingsModel>("older", "After September 2024", "./CurrentRatings"),
            GetTabFor<PreviousRatingsModel>("older", "Before September 2024", "./PreviousRatings")
        ];

        return pageResult;
    }


    protected abstract OfstedRating? GetOfstedRating(SchoolOfstedServiceModel ofstedRatings);
    protected abstract BeforeOrAfterJoining GetWhenInspectionHappened(SchoolOfstedServiceModel ofstedRatings);

}
