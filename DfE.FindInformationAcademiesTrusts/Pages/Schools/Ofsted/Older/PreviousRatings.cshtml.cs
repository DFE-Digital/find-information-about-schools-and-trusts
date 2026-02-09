using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Export;
using DfE.FindInformationAcademiesTrusts.Services.Ofsted;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;

namespace DfE.FindInformationAcademiesTrusts.Pages.Schools.Ofsted.Older;

public class PreviousRatingsModel(
    ISchoolService schoolService,
    ISchoolOverviewDetailsService schoolOverviewDetailsService,
    ITrustService trustService,
    IDataSourceService dataSourceService,
    IOfstedSchoolDataExportService ofstedSchoolDataExportService,
    IDateTimeProvider dateTimeProvider,
    IOtherServicesLinkBuilder otherServicesLinkBuilder,
    ISchoolNavMenu schoolNavMenu,
    IOfstedService ofstedService) : OlderBaseRatingsModel(schoolService, schoolOverviewDetailsService, trustService,
    dataSourceService, ofstedSchoolDataExportService, dateTimeProvider, otherServicesLinkBuilder, schoolNavMenu,
    ofstedService)
{
    public const string SubPageName = "Older inspections (before November 2025)";

    public override PageMetadata PageMetadata => base.PageMetadata with
    {
        SubPageName = SubPageName, TabName = "Before September 2024"
    };

    protected override List<OfstedRating> GetOfstedRating(OlderSchoolOfstedServiceModel ofstedRatings) =>
        ofstedRatings.RatingsWithSingleHeadlineGrade;

}
