
using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Services.Academy;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Export;
using DfE.FindInformationAcademiesTrusts.Services.Ofsted;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Trusts.Ofsted
{
    public class OverviewModel(
        IDataSourceService dataSourceService,
        ITrustService trustService,
        IOfstedTrustDataExportService ofstedTrustDataExportService,
        IDateTimeProvider dateTimeProvider,
        IOfstedService ofstedService) : OfstedAreaModel(dataSourceService, trustService, ofstedTrustDataExportService,
        dateTimeProvider)
    {
        public const string SubPageName = "Overview";

        public override PageMetadata PageMetadata => base.PageMetadata with { SubPageName = SubPageName };

        public List<OfstedOverviewInspectionServiceModel> OverviewInspectionModels { get; set; } = [];

        public override async Task<IActionResult> OnGetAsync()
        {
            var pageResult = await base.OnGetAsync();

            if (pageResult.GetType() == typeof(NotFoundResult)) return pageResult;

            OverviewInspectionModels = await ofstedService.GetOfstedOverviewInspectionForTrustAsync(Uid);

            return pageResult;
        }
    }
}
