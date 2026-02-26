using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
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
    IOtherServicesLinkBuilder otherServicesLinkBuilder,
    ISchoolNavMenu schoolNavMenu,
    IOfstedService ofstedService,
    IPowerBiLinkBuilderService powerBiLinkBuilderService) : OfstedAreaModel(schoolService, schoolOverviewDetailsService, trustService,
    dataSourceService, otherServicesLinkBuilder, schoolNavMenu)
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

        PowerBiLink = powerBiLinkBuilderService.BuildOfstedPublishedLink(Urn);

        return pageResult;
    }


    protected abstract List<OfstedRating> GetOfstedRating(OlderSchoolOfstedServiceModel ofstedRatings);

}
