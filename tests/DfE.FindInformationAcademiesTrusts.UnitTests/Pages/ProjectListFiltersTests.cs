using DfE.FindInformationAcademiesTrusts.Pages.ManageProjectsAndCases;
using Microsoft.Extensions.Primitives;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages
{
    public class ProjectListFiltersTests
    {
        private readonly IDictionary<string, object?> _store;
        private readonly string[]? _systems;
        private readonly string[]? _projectList;

        public ProjectListFiltersTests()
        {
            _store = new Dictionary<string, object?>();
            _systems = ["Systems1", "Systems2"];
            _projectList = ["Project1", "Project2", "Project3"];
        }

        [Fact]
        public void Constructor_SetsEmptyValues_WhenInitialized()
        {
            // Arrange & Act
            var projectListFilters = new ProjectListFilters();

            // Assert
            Assert.Empty(projectListFilters.AvailableProjectTypes);
            Assert.Empty(projectListFilters.AvailableSystems);
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
            Assert.True(isVisible);
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
            Assert.True(isVisible);
        }

        [Fact]
        public void IsVisible_ReturnsFalse_WhenNoSelectedFiltersExist()
        {
            // Arrange
            var projectListFilters = new ProjectListFilters();

            // Act
            var isVisible = projectListFilters.IsVisible;

            // Assert
            Assert.False(isVisible);
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
            Assert.Empty(projectListFilters.SelectedProjectTypes);
            Assert.Empty(projectListFilters.SelectedSystems);
            Assert.Empty(persist);
        }

        [Fact]
        public void PopulateFrom_PullsFromCache_WhenQueryStringHasNoValues()
        {
            // Arrange
            var query = new Dictionary<string, StringValues>();

            var store = new Dictionary<string, object?>
            {
                { ProjectListFilters.FilterSystems, _systems },
            };
            var projectListFilters = new ProjectListFilters();
            projectListFilters.PersistUsing(store);

            // Act
            projectListFilters.PopulateFrom(query);

            // Assert
            Assert.Equal(_systems, projectListFilters.SelectedSystems);
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
            var expectedSystem = new string[] { "Systems2" };

            var store = new Dictionary<string, object?>
            {
                { ProjectListFilters.FilterSystems, _systems },
            };
            var projectListFilters = new ProjectListFilters();
            projectListFilters.PersistUsing(store);

            // Act
            projectListFilters.PopulateFrom(query);

            // Assert
            Assert.Equal(expectedSystem, projectListFilters.SelectedSystems);
        }
    }
}
