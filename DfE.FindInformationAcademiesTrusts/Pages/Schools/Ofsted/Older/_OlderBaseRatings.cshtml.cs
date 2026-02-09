using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Export;
using DfE.FindInformationAcademiesTrusts.Services.Ofsted;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Schools.Ofsted.Older;

public abstract class OlderBaseRatingsModel(
    ISchoolService schoolService,
    ISchoolOverviewDetailsService schoolOverviewDetailsService,
    ITrustService trustService,
    IDataSourceService dataSourceService,
    IOfstedSchoolDataExportService ofstedSchoolDataExportService,
    IDateTimeProvider dateTimeProvider,
    IOtherServicesLinkBuilder otherServicesLinkBuilder,
    ISchoolNavMenu schoolNavMenu,
    IOfstedService ofstedService) : OfstedAreaModel(schoolService, schoolOverviewDetailsService, trustService,
    dataSourceService, ofstedSchoolDataExportService, dateTimeProvider, otherServicesLinkBuilder, schoolNavMenu)
{
    private readonly ISchoolNavMenu _schoolNavMenu = schoolNavMenu;
    public List<OfstedRating> OfstedRatings { get; set; } = [];

    public override async Task<IActionResult> OnGetAsync()
    {
        var pageResult = await base.OnGetAsync();

        if (pageResult is NotFoundResult) return pageResult;

        var ofstedRatings = await ofstedService.GetSchoolOfstedRatingsAsBeforeAndAfterSeptemberGradeAsync(Urn);

        OfstedRatings = GetOfstedRating(ofstedRatings);

        TabList = _schoolNavMenu.GetTabLinksForOlderOfstedPages(this);

        return pageResult;
    }


    protected abstract List<OfstedRating> GetOfstedRating(OlderSchoolOfstedServiceModel ofstedRatings);

}
