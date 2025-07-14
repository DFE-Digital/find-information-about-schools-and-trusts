using DfE.FindInformationAcademiesTrusts.Pages.Shared;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;

namespace DfE.FindInformationAcademiesTrusts.Pages.Schools.Governance;

public class HistoricModel(
    ISchoolService schoolService,
    ITrustService trustService,
    IDataSourceService dataSourceService,
    ISchoolNavMenu schoolNavMenu)
    : GovernanceAreaModel(schoolService, trustService, dataSourceService, schoolNavMenu)
{
    public const string SubPageName = "Historic governors";

    public override PageMetadata PageMetadata => base.PageMetadata with { SubPageName = SubPageName };

    public static string NavTitle(int number)
    {
        return SubPageName + $"({number})";
    }
}
