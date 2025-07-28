using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages.Shared.DataSource;
using DfE.FindInformationAcademiesTrusts.Services.DataSource;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.Shared.DataSource;

public class DataSourceListEntryTest
{
    private readonly DataSourceServiceModel _dataSourceServiceModel = new(Source.Cdm, null, null);

    [Fact]
    public void GetLastUpdatedText_returns_unknown_when_LastUpdated_is_null()
    {
        DataSourceListEntry.GetLastUpdatedText(_dataSourceServiceModel with { LastUpdated = null }).Should()
            .Be("Unknown");
    }

    [Fact]
    public void GetLastUpdatedText_returns_correctString_when_LastUpdated_is_set()
    {
        var date = DateTime.Today;
        DataSourceListEntry.GetLastUpdatedText(_dataSourceServiceModel with { LastUpdated = date }).Should()
            .Be(date.ToString("dd MMM yyyy"));
    }

    [Fact]
    public void GetUpdatedByText_returns_null_when_UpdatedBy_is_null()
    {
        DataSourceListEntry.GetUpdatedByText(_dataSourceServiceModel with { UpdatedBy = null }).Should().Be(null);
    }

    [Fact]
    public void GetUpdatedByText_returns_correctString_when_UpdatedBy_is_set()
    {
        DataSourceListEntry.GetUpdatedByText(_dataSourceServiceModel with { UpdatedBy = "Updated by" }).Should()
            .Be("Updated by");
    }

    [Fact]
    public void GetUpdatedByText_returns_correctString_when_UpdatedBy_is_set_to_TRAMs_Migration()
    {
        DataSourceListEntry.GetUpdatedByText(_dataSourceServiceModel with { UpdatedBy = "TRAMs Migration" }).Should()
            .Be("TRAMS Migration");
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
    [InlineData(Source.MisFurtherEducation,
        "Further education and skills inspections and outcomes: management information")]
    [InlineData(Source.ExploreEducationStatistics, "Explore education statistics")]
    [InlineData(Source.FiatDb, "Find information about schools and trusts")]
    public void GetName_should_return_the_correct_string_for_each_source(Source source, string expected)
    {
        DataSourceListEntry.GetName(_dataSourceServiceModel with { Source = source }).Should().Be(expected);
    }

    [Fact]
    public void GetName_should_return_Unknown_when_source_is_not_recognised()
    {
        DataSourceListEntry.GetName(_dataSourceServiceModel with { Source = (Source)100 }).Should().Be("Unknown");
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

    [Theory]
    [InlineData("data-source-gias-mstr-all-information-was", Source.Gias, Source.Mstr)]
    [InlineData("data-source-mis-misfurthereducation-gias-all-information-was", Source.Mis, Source.MisFurtherEducation,
        Source.Gias)]
    public void TestId_should_combine_multiple_data_sources(string expected, params Source[] sources)
    {
        var sut = new DataSourceListEntry(sources.Select(s => _dataSourceServiceModel with { Source = s }).ToList());
        sut.TestId.Should().Be(expected);
    }

    [Theory]
    [InlineData(Source.Gias, "2001-02-03T04:05:06Z", "some.user@education.gov.uk", "All information was",
        "All information was updated by some.user@education.gov.uk on 03 Feb 2001")]
    [InlineData(Source.FiatDb, "2019-08-07T06:05:43Z", null, "School contacts",
        "School contacts taken from Find information about schools and trusts on 07 Aug 2019")]
    [InlineData((Source)100, "2025-07-07", null, "Miscellaneous information",
        "Miscellaneous information taken from Unknown on 07 Jul 2025")]
    [InlineData(Source.Mis, null, null, "Some other information",
        "Some other information taken from State-funded school inspections and outcomes: management information on Unknown")]
    public void ToString_should_return_expected_string_for_single_data_source(Source source, string? lastUpdatedString,
        string? updatedBy, string dataField, string expected)
    {
        var sut = new DataSourceListEntry(_dataSourceServiceModel with
        {
            Source = source, LastUpdated = lastUpdatedString is null ? null : DateTime.Parse(lastUpdatedString),
            UpdatedBy = updatedBy
        }, dataField);

        sut.ToString().Should().Be(expected);
    }

    [Fact]
    public void ToString_should_return_expected_string_for_two_data_sources()
    {
        List<DataSourceServiceModel> dataSources =
        [
            _dataSourceServiceModel with
            {
                Source = Source.ExploreEducationStatistics,
                LastUpdated = DateTime.Parse("2025-07-07")
            },
            _dataSourceServiceModel with
            {
                Source = Source.FiatDb,
                LastUpdated = DateTime.Parse("2025-07-01")
            }
        ];
        var sut = new DataSourceListEntry(dataSources, "Information from two sources");

        sut.ToString().Should()
            .Be(
                "Information from two sources taken from Explore education statistics on 07 Jul 2025 and Find information about schools and trusts on 01 Jul 2025");
    }

    [Fact]
    public void ToString_should_return_expected_string_for_three_data_sources()
    {
        List<DataSourceServiceModel> dataSources =
        [
            _dataSourceServiceModel with
            {
                Source = Source.ExploreEducationStatistics,
                LastUpdated = DateTime.Parse("2025-07-07")
            },
            _dataSourceServiceModel with
            {
                Source = Source.FiatDb,
                LastUpdated = DateTime.Parse("2025-07-01")
            },
            _dataSourceServiceModel with
            {
                Source = Source.Gias,
                LastUpdated = DateTime.Parse("2025-07-03")
            }
        ];
        var sut = new DataSourceListEntry(dataSources, "Information from three sources");

        sut.ToString().Should()
            .Be(
                "Information from three sources taken from Explore education statistics on 07 Jul 2025, Find information about schools and trusts on 01 Jul 2025, and Get information about schools on 03 Jul 2025");
    }
}
