using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Services.Academy;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Export;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Schools.Ofsted;

public abstract class BaseRatingsModel(ISchoolService schoolService, ISchoolOverviewDetailsService schoolOverviewDetailsService, ITrustService trustService, IDataSourceService dataSourceService, IOfstedSchoolDataExportService ofstedSchoolDataExportService, IDateTimeProvider dateTimeProvider, IOtherServicesLinkBuilder otherServicesLinkBuilder, ISchoolNavMenu schoolNavMenu) : OfstedAreaModel(schoolService, schoolOverviewDetailsService, trustService, dataSourceService, ofstedSchoolDataExportService, dateTimeProvider, otherServicesLinkBuilder, schoolNavMenu)
{
    public OfstedRating OfstedRating { get; set; } = null!;
    public BeforeOrAfterJoining InspectionBeforeOrAfterJoiningTrust { get; set; }

    public override async Task<IActionResult> OnGetAsync()
    {
        var pageResult = await base.OnGetAsync();
        
        if (pageResult is NotFoundResult) return pageResult;
        
        var ofstedRatings = await SchoolService.GetSchoolOfstedRatingsAsync(Urn);
        OfstedRating = GetOfstedRating(ofstedRatings);
        InspectionBeforeOrAfterJoiningTrust = GetWhenInspectionHappened(ofstedRatings);
        
        return pageResult;
    }
    
    protected abstract OfstedRating GetOfstedRating(SchoolOfstedServiceModel ofstedRatings);
    protected abstract BeforeOrAfterJoining GetWhenInspectionHappened(SchoolOfstedServiceModel ofstedRatings);
}
