using Dfe.CaseAggregationService.Client.Contracts;
using DfE.FindInformationAcademiesTrusts.Data;

namespace DfE.FindInformationAcademiesTrusts.Services.ManageProjectsAndCases
{
    public record GetCasesParameters(
        string? UserName,
        string? UserEmail,
        bool IncludePrepare,
        bool IncludeComplete,
        bool IncludeManageFreeSchools,
        bool IncludeConcerns,
        int Page,
        int? RecordCount,
        IEnumerable<string> ProjectFilters,
        SortCriteria Sort);

    public interface IGetCasesService
    {
        Task<IPaginatedList<UserCaseInfo>> GetCasesAsync(GetCasesParameters parameters);
    }
    public class GetCasesService: IGetCasesService
    {
        private readonly ICasesClient _caseAggregationServiceClient;
        public GetCasesService(ICasesClient caseAggregationServiceClient)
        {
            _caseAggregationServiceClient = caseAggregationServiceClient;
        }
        public async Task<IPaginatedList<UserCaseInfo>> GetCasesAsync(GetCasesParameters parameters)
        {
            var cases = await _caseAggregationServiceClient.GetCasesByUserAsync(
                parameters.UserName,
                parameters.UserEmail,
                false,
                parameters.IncludePrepare,
                parameters.IncludeComplete,
                parameters.IncludeManageFreeSchools,
                parameters.IncludeConcerns,
                false,
                parameters.ProjectFilters,
                "",
                parameters.Sort,
                parameters.Page,
                parameters.RecordCount,
                "1");
            
            return new PaginatedList<UserCaseInfo>(cases.CaseInfos, cases.TotalRecordCount, parameters.Page, parameters.RecordCount ?? 0);
        }
    }
}
