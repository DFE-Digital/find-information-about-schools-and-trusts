using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Ofsted;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;

namespace DfE.FindInformationAcademiesTrusts.Pages.Schools.Ofsted.Older;

public class PreviousRatingsModel(
    ISchoolService schoolService,
    ISchoolOverviewDetailsService schoolOverviewDetailsService,
    ITrustService trustService,
    IDataSourceService dataSourceService,
    IOtherServicesLinkBuilder otherServicesLinkBuilder,
    ISchoolNavMenu schoolNavMenu,
    IOfstedService ofstedService,
    IPowerBiLinkBuilderService powerBiLinkBuilderService) : OlderBaseRatingsModel(schoolService, schoolOverviewDetailsService, trustService,
    dataSourceService, otherServicesLinkBuilder, schoolNavMenu, ofstedService, powerBiLinkBuilderService)
{
    public const string SubPageName = "Older inspections (before November 2025)";

    public override PageMetadata PageMetadata => base.PageMetadata with
    {
        SubPageName = SubPageName, TabName = "Before September 2024"
    };

    protected override List<OfstedRating> GetOfstedRating(OlderSchoolOfstedServiceModel ofstedRatings) =>
        ofstedRatings.RatingsWithSingleHeadlineGrade;

}
