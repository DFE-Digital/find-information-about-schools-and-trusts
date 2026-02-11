using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Services.Academy;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Export;
using DfE.FindInformationAcademiesTrusts.Services.Ofsted;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Trusts.Ofsted
{
    public class OlderInspectionsModel(IDataSourceService dataSourceService,
        ITrustService trustService,
        IAcademyService academyService,
        IOfstedTrustDataExportService ofstedTrustDataExportService,
        IDateTimeProvider dateTimeProvider,
        IOfstedService ofstedService) : OfstedAreaModel(dataSourceService, trustService,
        academyService, ofstedTrustDataExportService, dateTimeProvider)
    {
        public const string SubPageName = "Older inspections (before November 2025)";

        public override PageMetadata PageMetadata => base.PageMetadata with { SubPageName = SubPageName };

        public List<TrustOfstedReportServiceModel<OlderInspectionServiceModel>> OlderOfstedInspections { get; private set; } = [];

        public override async Task<IActionResult> OnGetAsync()
        {
            var pageResult = await base.OnGetAsync();

            if (pageResult.GetType() == typeof(NotFoundResult)) return pageResult;

            OlderOfstedInspections = await ofstedService.GetEstablishmentsInTrustOlderOfstedRatings(Uid);

            return pageResult;
        }
    }
}
