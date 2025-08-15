using DfE.FindInformationAcademiesTrusts.Pages.ManageProjectsAndCases;
using Microsoft.Extensions.Primitives;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages;

public class ProjectListFiltersTests
{
    private readonly string[] _systems = ["Systems1", "Systems2"];
    private readonly string[] _projectList = ["Project1", "Project2", "Project3"];

    [Fact]
    public void Constructor_SetsEmptyValues_WhenInitialized()
    {
        // Arrange & Act
        var projectListFilters = new ProjectListFilters();

        // Assert
        projectListFilters.AvailableProjectTypes.Should().BeEmpty();
        projectListFilters.AvailableSystems.Should().BeEmpty();
    }

    [Fact]
    public void IsVisible_ReturnsTrue_WhenAnySelectedProjectTypeFiltersExist()
    {
        // Arrange
        var projectListFilters = new ProjectListFilters
        {
            SelectedProjectTypes = ["Type1"]
        };

        // Act
        var isVisible = projectListFilters.IsVisible;

        // Assert
        isVisible.Should().BeTrue();
    }

    [Fact]
    public void IsVisible_ReturnsTrue_WhenAnySelectedSystemFiltersExist()
    {
        // Arrange
        var projectListFilters = new ProjectListFilters
        {
            SelectedSystems = ["Type1"]
        };

        // Act
        var isVisible = projectListFilters.IsVisible;

        // Assert
        isVisible.Should().BeTrue();
    }

    [Fact]
    public void IsVisible_ReturnsFalse_WhenNoSelectedFiltersExist()
    {
        // Arrange
        var projectListFilters = new ProjectListFilters();

        // Act
        var isVisible = projectListFilters.IsVisible;

        // Assert
        isVisible.Should().BeFalse();
    }

    [Fact]
    public void PopulateFrom_ClearsFilters_WhenQueryStringContainsClearKey()
    {
        // Arrange
        var query = new Dictionary<string, StringValues>
        {
            { "clear", "true" }
        };

        var persist = new Dictionary<string, object?>
        {
            { ProjectListFilters.FilterProjectTypes, _systems },
            { ProjectListFilters.FilterSystems, _projectList }
        };

        var projectListFilters = new ProjectListFilters();
        projectListFilters.PersistUsing(persist);

        // Act
        projectListFilters.PopulateFrom(query);

        // Assert
        projectListFilters.SelectedProjectTypes.Should().BeEmpty();
        projectListFilters.SelectedSystems.Should().BeEmpty();
        persist.Should().BeEmpty();
    }

    [Fact]
    public void PopulateFrom_ClearsCache_WhenQueryStringHasNoValues()
    {
        // Arrange
        var query = new Dictionary<string, StringValues>();

        var store = new Dictionary<string, object?>
        {
            { ProjectListFilters.FilterSystems, _systems },
            { ProjectListFilters.FilterProjectTypes, _projectList }
        };
        var projectListFilters = new ProjectListFilters();
        projectListFilters.PersistUsing(store);

        // Act
        projectListFilters.PopulateFrom(query);

        // Assert
        projectListFilters.SelectedSystems.Should().BeEmpty();
    }

    [Fact]
    public void PopulateFrom_RemovesFilters_WhenQueryStringContainsRemoveKey()
    {
        // Arrange
        var query = new Dictionary<string, StringValues>
        {
            { "remove", "true" },
            { "SelectedSystems", new StringValues(["Systems1"]) }
        };
        var expectedSystem = new[] { "Systems2" };

        var store = new Dictionary<string, object?>
        {
            { ProjectListFilters.FilterSystems, _systems }
        };
        var projectListFilters = new ProjectListFilters();
        projectListFilters.PersistUsing(store);

        // Act
        projectListFilters.PopulateFrom(query);

        // Assert
        projectListFilters.SelectedSystems.Should().Equal(expectedSystem);
    }

    [Fact]
    public void PopulateFrom_RemovesFilters_WhenQueryStringContainsRemoveKey_WithNoValue()
    {
        // Arrange
        var query = new Dictionary<string, StringValues>
        {
            { "remove", "true" },
            { "SelectedSystems", new StringValues(["Systems2"]) }
        };

        var store = new Dictionary<string, object?>
        {
            { ProjectListFilters.FilterSystems, null }
        };
        var projectListFilters = new ProjectListFilters();
        projectListFilters.PersistUsing(store);

        // Act
        projectListFilters.PopulateFrom(query);

        // Assert
        projectListFilters.SelectedSystems.Should().BeEmpty();
    }
}
