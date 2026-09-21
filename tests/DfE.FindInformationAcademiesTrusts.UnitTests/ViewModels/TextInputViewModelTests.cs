using Dfe.FindInformationAcademiesTrusts.ViewModels;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.ViewModels
{
    public class TextInputViewModelTests
    {
        [Fact]
        public void TextInputViewModel_ShouldHaveDefaultValues()
        {
            // Arrange & Act
            var model = new TextInputViewModel();

            // Assert
            Assert.Null(model.Id);
            Assert.Null(model.Name);
            Assert.Null(model.Value);
            Assert.Null(model.Label);
            Assert.Null(model.ErrorMessage);
            Assert.Equal(0, model.Width);
            Assert.Null(model.Hint);
            Assert.False(model.HeadingLabel);
            Assert.Null(model.HeadingLabelClass);
        }

        [Fact]
        public void TextInputViewModel_ShouldSetAndGetValues()
        {
            // Arrange
            var model = new TextInputViewModel
            {
                Id = "search-input",
                Name = "SearchTerm",
                Value = "Test School",
                Label = "Search for a school",
                ErrorMessage = "Enter a school name",
                Width = 20,
                Hint = "For example, Oakwood Primary",
                HeadingLabel = true,
                HeadingLabelClass = "govuk-label--l"
            };

            // Act & Assert
            Assert.Equal("search-input", model.Id);
            Assert.Equal("SearchTerm", model.Name);
            Assert.Equal("Test School", model.Value);
            Assert.Equal("Search for a school", model.Label);
            Assert.Equal("Enter a school name", model.ErrorMessage);
            Assert.Equal(20, model.Width);
            Assert.Equal("For example, Oakwood Primary", model.Hint);
            Assert.True(model.HeadingLabel);
            Assert.Equal("govuk-label--l", model.HeadingLabelClass);
        }

        [Fact]
        public void TextInputViewModel_HeadingLabel_ShouldBeTogglable()
        {
            // Arrange
            var model = new TextInputViewModel { HeadingLabel = true };

            // Act
            model.HeadingLabel = false;

            // Assert
            Assert.False(model.HeadingLabel);
        }
    }
}
