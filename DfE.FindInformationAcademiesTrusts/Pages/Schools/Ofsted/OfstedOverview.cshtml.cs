using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Export;
using DfE.FindInformationAcademiesTrusts.Services.Ofsted;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Schools.Ofsted
{
    public class OfstedOverviewModel(
        ISchoolService schoolService,
        ISchoolOverviewDetailsService schoolOverviewDetailsService,
        ITrustService trustService,
        IDataSourceService dataSourceService,
        IOfstedSchoolDataExportService ofstedSchoolDataExportService,
        IDateTimeProvider dateTimeProvider,
        IOtherServicesLinkBuilder otherServicesLinkBuilder,
        ISchoolNavMenu schoolNavMenu,
        IOfstedService ofstedService) : OfstedAreaModel(schoolService, schoolOverviewDetailsService,
        trustService,
        dataSourceService, ofstedSchoolDataExportService, dateTimeProvider, otherServicesLinkBuilder, schoolNavMenu)
    {
        public const string SubPageName = "Overview";

        public OfstedOverviewInspectionServiceModel OverviewInspectionModel { get; set; } = new(null, null, null);

        public override PageMetadata PageMetadata => base.PageMetadata with { SubPageName = SubPageName };

        public override async Task<IActionResult> OnGetAsync()
        {
            var pageResult = await base.OnGetAsync();

            if (pageResult is NotFoundResult) return pageResult;

            OverviewInspectionModel = await ofstedService.GetOfstedOverviewInspectionAsync(Urn);


            return pageResult;
        }
    }
}
