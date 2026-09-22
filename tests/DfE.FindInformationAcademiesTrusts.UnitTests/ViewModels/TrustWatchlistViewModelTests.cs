using Dfe.FindInformationAcademiesTrusts.ViewModels;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.ViewModels
{
    public class TrustWatchlistViewModelTests
    {
        [Fact]
        public void TrustWatchlistViewModel_ShouldHaveDefaultValues()
        {
            // Arrange & Act
            var model = new TrustWatchlistViewModel();

            // Assert
            Assert.Equal(Guid.Empty, model.WatchlistId);
            Assert.Equal(default, model.CreatedOn);
            Assert.Null(model.Name);
            Assert.Null(model.ReferenceNumber);
            Assert.Null(model.GroupUid);
            Assert.Null(model.Region);
            Assert.Null(model.CompaniesHouseNumber);
        }

        [Fact]
        public void TrustWatchlistViewModel_ShouldSetAndGetValues()
        {
            // Arrange
            var watchlistId = Guid.NewGuid();
            var createdOn = DateTime.UtcNow;

            var model = new TrustWatchlistViewModel
            {
                WatchlistId = watchlistId,
                CreatedOn = createdOn,
                Name = "Test Trust",
                ReferenceNumber = "TR00001",
                GroupUid = "2001",
                Region = "South East",
                CompaniesHouseNumber = "01234567"
            };

            // Act & Assert
            Assert.Equal(watchlistId, model.WatchlistId);
            Assert.Equal(createdOn, model.CreatedOn);
            Assert.Equal("Test Trust", model.Name);
            Assert.Equal("TR00001", model.ReferenceNumber);
            Assert.Equal("2001", model.GroupUid);
            Assert.Equal("South East", model.Region);
            Assert.Equal("01234567", model.CompaniesHouseNumber);
        }

        [Fact]
        public void TrustWatchlistViewModel_ShouldAllowNullValues()
        {
            // Arrange
            var model = new TrustWatchlistViewModel
            {
                Name = null,
                ReferenceNumber = null,
                GroupUid = null,
                Region = null,
                CompaniesHouseNumber = null
            };

            // Act & Assert
            Assert.Null(model.Name);
            Assert.Null(model.ReferenceNumber);
            Assert.Null(model.GroupUid);
            Assert.Null(model.Region);
            Assert.Null(model.CompaniesHouseNumber);
        }
    }
}
