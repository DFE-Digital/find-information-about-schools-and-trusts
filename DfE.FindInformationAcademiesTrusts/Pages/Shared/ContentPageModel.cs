using System.Diagnostics.CodeAnalysis;

namespace DfE.FindInformationAcademiesTrusts.Pages.Shared;

[ExcludeFromCodeCoverage(Justification = "Class with no behaviour and only used by Razor pages")]
public class ContentPageModel : BasePageModel
{
    public bool ShowBreadcrumb { get; set; } = true;
    public string? BannerHeading { get; set; }
}
