using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;

namespace DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;

public record DataSourceListEntry(DataSourceServiceModel DataSource, string DataField = "All information was")
{
    public string LastUpdatedText => DataSource.LastUpdated is null
        ? "Unknown"
        : DataSource.LastUpdated.Value.ToString(StringFormatConstants.DisplayDateFormat);

    public string? UpdatedByText => DataSource.UpdatedBy == "TRAMs Migration"
        ? "TRAMS Migration"
        : DataSource.UpdatedBy;

    public string Name => DataSource.Source switch
    {
        Source.Gias => "Get information about schools",
        Source.Mstr => "Get information about schools (internal use only, do not share outside of DfE)",
        Source.Cdm => "RSD (Regional Services Division) service support team",
        Source.Mis => "State-funded school inspections and outcomes: management information",
        Source.MisFurtherEducation => "Further education and skills inspections and outcomes: management information",
        Source.ExploreEducationStatistics => "Explore education statistics",
        Source.FiatDb => "Find information about schools and trusts",
        Source.Prepare => "Prepare",
        Source.Complete => "Complete",
        Source.ManageFreeSchoolProjects => "Manage free school projects",
        _ => "Unknown"
    };

    public string TestId => $"data-source-{DataSource.Source.ToString()}-{DataField}".Kebabify();
}
