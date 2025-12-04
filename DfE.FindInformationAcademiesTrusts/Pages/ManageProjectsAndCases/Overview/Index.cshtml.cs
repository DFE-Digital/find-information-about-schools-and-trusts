using Dfe.CaseAggregationService.Client.Contracts;
using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Services.ManageProjectsAndCases;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.ManageProjectsAndCases.Overview;

public class IndexModel : BasePageModel, IPaginationModel
{
    private readonly IGetCasesService _getCasesService;
    private readonly IUserDetailsProvider _userDetailsProvider;

    [BindProperty] public ProjectListFilters Filters { get; init; } = new();

    [BindProperty(SupportsGet = true)] public string Sorting { get; set; } = ResultSorting.CreatedDesc;

    [BindProperty] public int TotalProjects { get; set; }

    public string PageName => "ManageProjectsAndCases/Overview/Index";

    public IPageStatus PageStatus => Cases.PageStatus;

    public Dictionary<string, string> PaginationRouteData { get; set; } = new();

    [BindProperty(SupportsGet = true)] public int PageNumber { get; set; } = 1;

    public IPaginatedList<UserCaseInfo> Cases { get; set; } = PaginatedList<UserCaseInfo>.Empty();

    public IndexModel(IGetCasesService getCasesService,
        IUserDetailsProvider userDetailsProvider)
    {
        _getCasesService = getCasesService;
        _userDetailsProvider = userDetailsProvider;
    }


    public async Task<ActionResult> OnGetAsync()
    {
        //Need to do this here as the Authorize attribute is not working with the AutomationAuthorizationHandler
        if (!User.IsInRole("User.Role.MPCViewer"))
        {
            return StatusCode(403);
        }

        Filters.PersistUsing(TempData).PopulateFrom(Request.Query);

        Filters.AvailableProjectTypes =
        [
            "Conversion",
            "Form a MAT",
            "Governance capability",
            "Non-compliance",
            "Pre-opening",
            "Pre-opening - not included in figures",
            "Safeguarding non-compliance",
            "Transfer"
        ];

        Filters.AvailableSystems =
        [
            "Complete conversions, transfers and changes",
            "Manage free school projects",
            "Prepare conversions and transfers",
            "Record concerns and support for trusts"
        ];

        var (userName, userEmail) = _userDetailsProvider.GetUserDetails();

        Cases = await _getCasesService.GetCasesAsync(
            new GetCasesParameters
            (
                userName,
                userEmail,
                IncludePrepare(),
                IncludeComplete(),
                IncludeManageFreeSchools(),
                IncludeConcerns(),
                PageNumber,
                25,
                Filters.SelectedProjectTypes,
                ConvertSortCriteria()
            ));

        TotalProjects = Cases.Count;

        PaginationRouteData = new Dictionary<string, string> { { nameof(Sorting), Sorting } };

        return Page();
    }

    private bool IncludePrepare()
    {
        return Include("Prepare conversions and transfers");
    }

    private bool IncludeComplete()
    {
        return Include("Complete conversions, transfers and changes");
    }

    private bool IncludeManageFreeSchools()
    {
        return Include("Manage free school projects");
    }

    private bool IncludeConcerns()
    {
        return Include("Record concerns and support for trusts");
    }

    private bool Include(string system)
    {
        if (Filters.SelectedSystems.Length == 0)
        {
            return true;
        }

        return Filters.SelectedSystems.Contains(system);
    }

    private SortCriteria ConvertSortCriteria()
    {
        return Sorting switch
        {
            ResultSorting.CreatedAsc => SortCriteria.CreatedDateAscending,
            ResultSorting.CreatedDesc => SortCriteria.CreatedDateDescending,
            ResultSorting.UpdatedAsc => SortCriteria.UpdatedDateAscending,
            ResultSorting.UpdatedDesc => SortCriteria.UpdatedDateDescending,
            _ => SortCriteria.CreatedDateDescending
        };
    }
}
