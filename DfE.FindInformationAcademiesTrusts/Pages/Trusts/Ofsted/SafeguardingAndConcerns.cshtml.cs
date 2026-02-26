using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.Ofsted;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.Trusts.Ofsted
{
    public class SafeguardingAndConcernsModel(
        IDataSourceService dataSourceService,
        ITrustService trustService,
        IOfstedService ofstedService) : OfstedAreaModel(dataSourceService, trustService)
    {
        public const string SubPageName = "Safeguarding and concerns";

        public override PageMetadata PageMetadata => base.PageMetadata with { SubPageName = SubPageName };

        public List<TrustOfstedReportServiceModel<SafeGuardingAndConcernsServiceModel>> SafeGuardingInspectionModels
        {
            get;
            set;
        } = [];

        public override async Task<IActionResult> OnGetAsync()
        {
            var pageResult = await base.OnGetAsync();

            if (pageResult.GetType() == typeof(NotFoundResult)) return pageResult;

            SafeGuardingInspectionModels = await ofstedService.GetOfstedOverviewSafeguardingAndConcerns(Uid);

            return pageResult;
        }
    }
}
