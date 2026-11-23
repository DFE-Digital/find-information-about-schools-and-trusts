using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.HttpServices;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.Pages.LocalAuthorities;

public class SearchModel : BasePageModel, IPageSearchFormModel
{
    private readonly IGetLocalAuthorities _getLocalAuthorities;

    public SearchModel(IGetLocalAuthorities getLocalAuthorities)
    {
        _getLocalAuthorities = getLocalAuthorities;
        ShowHeaderSearch = false;
    }

    [BindProperty] public string? LaCode { get; set; }

    public string PageSearchFormInputId => "la-search";
    public string LabelText => "Search for a local authority ";
    public string HintText => "Search by LA Code or name. Include any punctuation in names.";
    public string PlaceholderText => "e.g. Leeds City Council";

    public IReadOnlyList<ExternalServiceLink> ServiceLinks { get; } =
    [
        new("Local Authority Interactive Tool (LAIT)", "Tool to provide an overall view of an authority and its trends.", "https://department-for-education.shinyapps.io/local-authority-interactive-tool/"),
        new("Attainment Tool with Latest Available Schools Time-series (ATLAST)", "Provides a time series of school level attainment key headline measures for phonics, KS2, KS4 and KS5.", "https://rsconnect/rsc/connect/#/login?url=https://rsconnect/rsc/RG/"),
        new("Explore Education Statistics (EES)", "Provides official statistics, downloadable files and the ability to create tables. Suitable for those who need a more detailed analysis.", "https://explore-education-statistics.service.gov.uk/"),
        new("Financial Benchmarking and Insights Tool (FBIT)", "Tool for viewing and comparing financial information at school, trust and local-authority level.", "https://financial-benchmarking-and-insights-tool.education.gov.uk/"),
        new("Association of Directors of Children’s Services (ACDS)", "Tool for finding for finding Directors and Assistant Directors of Children's Services.", "https://www.adcs.org.uk/"),
        new("Data Tools", "Approved data tools, datasets, and guidance produced by the Data Analysis Unit (DAU).", "https://educationgovuk.sharepoint.com/sites/lvewp00299/SitePages/DAU%20Data%20Tools%20and%20Sources.aspx"),
    ];

    public IActionResult OnPost()
    {
        return RedirectToPage("/LocalAuthorities/Results", new { KeyWords, LaCode });
    }

    public async Task<IActionResult> OnGetPopulateAutocompleteAsync(string? keywords)
    {
        if (string.IsNullOrWhiteSpace(keywords)) return new JsonResult(Array.Empty<object>());

        var matches = await _getLocalAuthorities.Find(keywords);
        return new JsonResult(matches.Take(5).Select(la => new { code = la.Code, name = la.Name }));
    }
}
