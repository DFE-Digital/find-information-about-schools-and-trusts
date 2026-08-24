using System.Diagnostics.CodeAnalysis;
using DfE.FindInformationAcademiesTrusts.Data;

namespace DfE.FindInformationAcademiesTrusts.Pages;

[ExcludeFromCodeCoverage]
public static class ViewConstants
{
    public const string ServiceName = "Find information about schools and trusts";

    public const string ReportAProblemMailToLink =
        "mailto:regionalservices.rg@education.gov.uk?subject=Report a problem with Find information about schools and trusts";

    public const string ReportNotFoundMailtoLink =
        "mailto:regionalservices.rg@education.gov.uk?subject=Page not found – Find information about schools and trusts (FAST)";

    public const string GetHelpFormLink =
        "https://forms.office.com/Pages/ResponsePage.aspx?id=yXfS-grGoU2187O4s0qC-X7F89QcWu5CjlJXwF0TVktUMTFEUVRCVVg4WlMyS1AzUEJSUDAySlhQTCQlQCN0PWcu";

    public const string FeedbackFormLink =
        "https://forms.office.com/Pages/ResponsePage.aspx?id=yXfS-grGoU2187O4s0qC-SZtRygfwTNOqcfRq-MXpv9UOTIyQlNYR0hJT1Q0TUFVSlJGVFhES01LVC4u";

    public const string SuggestChangeFormLink =
        "https://forms.office.com/Pages/ResponsePage.aspx?id=yXfS-grGoU2187O4s0qC-fkHK2JGo_BIpVChpLMaBFpUNUFDSzhQN0FHVklTV0JWTzFZTjNKWTNJUi4u";

    public const string AccessibilityStatementLink = "https://accessibility-statements.education.gov.uk/s/31";

    public const string NoDataText = "No data";
    public const string UnconfirmedDateText = "Unconfirmed";

    public static readonly List<ExternalServiceLink> ServiceLinks =
    [
        new("Reporting",
            "Tools to help you gather educational data and create reports.",
            "https://educationgovuk.sharepoint.com/sites/lvewp00299/SitePages/reportinganddata.aspx"),
        
        new("Data tools",
            "Approved data tools, datasets, and guidance produced by the Data Analysis Unit (DAU).",
            "https://educationgovuk.sharepoint.com/sites/lvewp00299/SitePages/DAU%20Data%20Tools%20and%20Sources.aspx?CID=c3424d47-3c89-45bc-a2e5-d905b8500610&csf=1&e=XFrLII&web=1"),

        new("High quality trust framework",
            "Process guidance and tools for making trust-related project decisions.",
            "https://educationgovuk.sharepoint.com/sites/lvewp00299/SitePages/RG%20high%20quality%20trust%20framework.aspx"),
        
        new("Regions mapping tool",
        "Create and view maps based on all school locations in England.",
        "https://app.powerbi.com/groups/me/reports/8ed3ef41-e67a-4408-b79a-80b66ee8b8f0/ReportSection421352c964403930930c?experience=power-bi"),
        
        new("Record concerns or support for trusts",
            "Add cases or interactions, record risks and log support and concerns for trusts.",
            "https://educationgovuk.sharepoint.com/sites/lvewp00299/SitePages/Record-concerns-and-support-for-trusts.aspx"),
        
        new("Prepare conversions and transfers",
            "Create a transfer or conversion project document for an advisory board.",
            "https://educationgovuk.sharepoint.com/sites/lvewp00299/SitePages/Prepare-Conversions-and-Transfers.aspx"),
    ];
}
