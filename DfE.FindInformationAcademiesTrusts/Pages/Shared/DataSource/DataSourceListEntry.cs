using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;

namespace DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;

public record DataSourceListEntry(List<DataSourceServiceModel> DataSources, string DataField = "All information was")
{
    public DataSourceListEntry(DataSourceServiceModel dataSource, string DataField = "All information was") : this(
        [dataSource], DataField)
    {
    }

    public static string GetLastUpdatedText(DataSourceServiceModel dataSource)
    {
        return dataSource.LastUpdated is null
            ? "Unknown"
            : dataSource.LastUpdated.Value.ToString(StringFormatConstants.DisplayDateFormat);
    }

    public static string? GetUpdatedByText(DataSourceServiceModel dataSource)
    {
        return dataSource.UpdatedBy == "TRAMs Migration"
            ? "TRAMS Migration"
            : dataSource.UpdatedBy;
    }

    public string TestId =>
        $"data-source-{string.Join('-', DataSources.Select(ds => ds.Source))}-{DataField}".Kebabify();

    public static string GetName(DataSourceServiceModel dataSource)
    {
        return dataSource.Source switch
        {
            Source.Gias => "Get information about schools",
            Source.Mstr => "Get information about schools (internal use only, do not share outside of DfE)",
            Source.Cdm => "RSD (Regional Services Division) service support team",
            Source.Mis => "State-funded school inspections and outcomes: management information",
            Source.MisFurtherEducation =>
                "Further education and skills inspections and outcomes: management information",
            Source.ExploreEducationStatistics => "Explore education statistics",
            Source.FiatDb => "Find information about schools and trusts",
            Source.Prepare => "Prepare",
            Source.Complete => "Complete",
            Source.ManageFreeSchoolProjects => "Manage free school projects",
            Source.CompareSchoolCollegePerformanceEngland => "Compare school and college performance in England",
            _ => "Unknown"
        };
    }

    public override string ToString()
    {
        if (DataSources.Count == 1)
        {
            var dataSource = DataSources[0];
            return dataSource.UpdatedBy is null
                ? $"{DataField} taken from {GetDataSourceText(dataSource)}"
                : $"{DataField} updated by {GetDataSourceText(dataSource)}";
        }

        if (DataSources.Count == 2)
        {
            var firstDataSource = DataSources[0];
            var secondDataSource = DataSources[1];
            return
                $"{DataField} taken from {GetDataSourceText(firstDataSource)} and {GetDataSourceText(secondDataSource)}";
        }

        var lastDataSource = DataSources[^1];
        var rest = DataSources.Take(DataSources.Count - 1);

        return
            $"{DataField} taken from {string.Join(", ", rest.Select(GetDataSourceText))}, and {GetDataSourceText(lastDataSource)}";
    }

    private static string GetDataSourceText(DataSourceServiceModel dataSource)
    {
        return dataSource.UpdatedBy is null
            ? $"{GetName(dataSource)} on {GetLastUpdatedText(dataSource)}"
            : $"{GetUpdatedByText(dataSource)} on {GetLastUpdatedText(dataSource)}";
    }
}
