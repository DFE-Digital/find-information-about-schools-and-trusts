using DfE.FindInformationAcademiesTrusts.Domain.ValueObjects;

namespace DfE.FindInformationAcademiesTrusts.Domain.UnitTests.ValueObjects
{
    public class WatchlistIdTests
    {
        [Fact]
        public void Constructor_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var expectedValue = Guid.NewGuid();

            // Act
            var watchlistId = new WatchlistId(expectedValue);

            // Assert
            Assert.Equal(expectedValue, watchlistId.Value);
        }

        [Fact]
        public void ToString_ShouldReturnCorrectFormat()
        {
            // Arrange
            var value = Guid.NewGuid();
            var watchlistId = new WatchlistId(value);

            // Act
            var result = watchlistId.ToString();

            // Assert
            Assert.Equal($"WatchlistId {{ Value = {value} }}", result);
        }

        [Fact]
        public void Equals_ShouldBeTrue_WhenValuesAreTheSame()
        {
            // Arrange
            var value = Guid.NewGuid();

            // Act
            var first = new WatchlistId(value);
            var second = new WatchlistId(value);

            // Assert
            Assert.Equal(first, second);
            Assert.True(first == second);
            Assert.Equal(first.GetHashCode(), second.GetHashCode());
        }

        [Fact]
        public void Equals_ShouldBeFalse_WhenValuesAreDifferent()
        {
            // Arrange
            var first = new WatchlistId(Guid.NewGuid());
            var second = new WatchlistId(Guid.NewGuid());

            // Assert
            Assert.NotEqual(first, second);
            Assert.True(first != second);
        }
    }
}
