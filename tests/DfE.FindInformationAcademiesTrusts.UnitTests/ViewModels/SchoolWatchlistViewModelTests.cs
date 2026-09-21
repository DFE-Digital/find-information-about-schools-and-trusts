using Dfe.FindInformationAcademiesTrusts.ViewModels;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.ViewModels
{
    public class SchoolWatchlistViewModelTests
    {
        [Fact]
        public void SchoolWatchlistViewModel_ShouldHaveDefaultValues()
        {
            // Arrange & Act
            var model = new SchoolWatchlistViewModel();

            // Assert
            Assert.Equal(Guid.Empty, model.WatchlistId);
            Assert.Equal(default, model.CreatedOn);
            Assert.Null(model.Urn);
            Assert.Null(model.Name);
            Assert.Null(model.TrustName);
            Assert.Null(model.LocalAuthority);
        }

        [Fact]
        public void SchoolWatchlistViewModel_ShouldSetAndGetValues()
        {
            // Arrange
            var watchlistId = Guid.NewGuid();
            var createdOn = DateTime.UtcNow;

            var model = new SchoolWatchlistViewModel
            {
                WatchlistId = watchlistId,
                CreatedOn = createdOn,
                Urn = "100001",
                Name = "Test School",
                TrustName = "Test Trust",
                LocalAuthority = "Test Authority"
            };

            // Act & Assert
            Assert.Equal(watchlistId, model.WatchlistId);
            Assert.Equal(createdOn, model.CreatedOn);
            Assert.Equal("100001", model.Urn);
            Assert.Equal("Test School", model.Name);
            Assert.Equal("Test Trust", model.TrustName);
            Assert.Equal("Test Authority", model.LocalAuthority);
        }

        [Fact]
        public void SchoolWatchlistViewModel_ShouldAllowNullValues()
        {
            // Arrange
            var model = new SchoolWatchlistViewModel
            {
                Urn = null,
                Name = null,
                TrustName = null,
                LocalAuthority = null
            };

            // Act & Assert
            Assert.Null(model.Urn);
            Assert.Null(model.Name);
            Assert.Null(model.TrustName);
            Assert.Null(model.LocalAuthority);
        }
    }
}
