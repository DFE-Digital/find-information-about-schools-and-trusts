using DfE.FindInformationAcademiesTrusts.Pages.ManageProjectsAndCases;
using Microsoft.Extensions.Primitives;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages
{
    public class ProjectListFiltersTests
    {
        private readonly IDictionary<string, object?> _store;
        private readonly string[]? _systems;

        public ProjectListFiltersTests()
        {
            _store = new Dictionary<string, object?>();
            _systems = ["Systems1", "Systems2"];
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
        public void IsVisible_ReturnsTrue_WhenAnySelectedFiltersExist()
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

            var projectListFilters = new ProjectListFilters();
            projectListFilters.PersistUsing(new Dictionary<string, object?>
            {
                { ProjectListFilters.FilterSystems, _systems }
            });

            // Act
            projectListFilters.PopulateFrom(query);

            // Assert
            Assert.Empty(projectListFilters.SelectedProjectTypes);
            Assert.Empty(projectListFilters.SelectedSystems);
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
                { ProjectListFilters.FilterSystems, _systems }
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
