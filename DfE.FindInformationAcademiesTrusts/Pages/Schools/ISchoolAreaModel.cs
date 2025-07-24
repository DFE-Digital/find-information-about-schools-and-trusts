using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.NavMenu;
using DfE.FindInformationAcademiesTrusts.Services.Trust;

namespace DfE.FindInformationAcademiesTrusts.Pages.Schools;

public interface ISchoolAreaModel
{
    int Urn { get; }
    SchoolCategory SchoolCategory { get; }
    List<DataSourcePageListEntry> DataSourcesPerPage { get; }
    PageMetadata PageMetadata { get; }
    string SchoolName { get; }
    string SchoolType { get; }
    TrustSummaryServiceModel? TrustSummary { get; }
    bool IsPartOfAFederation { get; }
    NavLink[] ServiceNavLinks { get; }
    NavLink[] SubNavLinks { get; }
}
