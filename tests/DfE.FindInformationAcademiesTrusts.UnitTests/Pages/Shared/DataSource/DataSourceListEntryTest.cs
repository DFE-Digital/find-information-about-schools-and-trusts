using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Shared.DataSource;

public class DataSourceListEntryTest
{
    private readonly DataSourceServiceModel _dataSourceServiceModel = new(Source.Cdm, null, null);

    [Fact]
    public void LastUpdatedText_returns_unknown_when_LastUpdated_is_null()
    {
        var sut = new DataSourceListEntry(_dataSourceServiceModel with { LastUpdated = null });
        sut.LastUpdatedText.Should().Be("Unknown");
    }

    [Fact]
    public void LastUpdatedText_returns_correctString_when_LastUpdated_is_set()
    {
        var date = DateTime.Today;
        var sut = new DataSourceListEntry(_dataSourceServiceModel with { LastUpdated = date });
        sut.LastUpdatedText.Should().Be(date.ToString("dd MMM yyyy"));
    }

    [Fact]
    public void UpdatedByText_returns_null_when_UpdatedBy_is_null()
    {
        var sut = new DataSourceListEntry(_dataSourceServiceModel with { UpdatedBy = null });
        sut.UpdatedByText.Should().Be(null);
    }

    [Fact]
    public void UpdatedByText_returns_correctString_when_UpdatedBy_is_set()
    {
        var sut = new DataSourceListEntry(_dataSourceServiceModel with { UpdatedBy = "Updated by" });
        sut.UpdatedByText.Should().Be("Updated by");
    }

    [Fact]
    public void UpdatedByText_returns_correctString_when_UpdatedBy_is_set_to_TRAMs_Migration()
    {
        var sut = new DataSourceListEntry(_dataSourceServiceModel with { UpdatedBy = "TRAMs Migration" });
        sut.UpdatedByText.Should().Be("TRAMS Migration");
    }

    [Fact]
    public void DataField_default_is_correct()
    {
        var sut = new DataSourceListEntry(_dataSourceServiceModel);
        sut.DataField.Should().Be("All information was");
    }

    [Fact]
    public void DataField_is_correct_when_set()
    {
        var sut = new DataSourceListEntry(_dataSourceServiceModel, "Not all information");
        sut.DataField.Should().Be("Not all information");
    }

    [Theory]
    [InlineData(Source.Gias, "Get information about schools")]
    [InlineData(Source.Mstr, "Get information about schools (internal use only, do not share outside of DfE)")]
    [InlineData(Source.Cdm, "RSD (Regional Services Division) service support team")]
    [InlineData(Source.Mis, "State-funded school inspections and outcomes: management information")]
    [InlineData(Source.ExploreEducationStatistics, "Explore education statistics")]
    [InlineData(Source.FiatDb, "Find information about schools and trusts")]
    public void Name_should_return_the_correct_string_for_each_source(Source source, string expected)
    {
        var sut = new DataSourceListEntry(_dataSourceServiceModel with { Source = source });
        sut.Name.Should().Be(expected);
    }

    [Fact]
    public void Name_should_return_Unknown_when_source_is_not_recognised()
    {
        var sut = new DataSourceListEntry(_dataSourceServiceModel with { Source = (Source)100 });
        sut.Name.Should().Be("Unknown");
    }

    [Theory]
    [InlineData(Source.Gias, "All information was", "data-source-gias-all-information-was")]
    [InlineData(Source.Mstr, "Not all information", "data-source-mstr-not-all-information")]
    [InlineData(Source.Cdm, "", "data-source-cdm")]
    public void TestId_should_combine_source_and_data_field(Source source, string dataField, string expected)
    {
        var sut = new DataSourceListEntry(_dataSourceServiceModel with { Source = source }, dataField);
        sut.TestId.Should().Be(expected);
    }
}
