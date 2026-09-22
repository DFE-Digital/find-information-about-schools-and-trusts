using Dfe.FindInformationAcademiesTrusts.ViewModels;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.ViewModels
{
    public class RadioButtonViewModelTests
    {
        [Fact]
        public void RadioButtonViewModel_ShouldHaveDefaultValues()
        {
            // Arrange & Act
            var model = new RadioButtonViewModel();

            // Assert
            Assert.Null(model.Heading);
            Assert.Null(model.HeadingStyle);
            Assert.Null(model.Hint);
            Assert.Null(model.ErrorMessage);
            Assert.Null(model.Name);
            Assert.Empty(model.RadioButtons);
            Assert.Null(model.Value);
            Assert.False(model.Inline);
        }

        [Fact]
        public void RadioButtonViewModel_ShouldSetAndGetValues()
        {
            // Arrange
            var model = new RadioButtonViewModel
            {
                Heading = "Select the type of establishment to add",
                HeadingStyle = "govuk-label govuk-label--l",
                Hint = "Please select an option",
                ErrorMessage = "Select an option",
                Name = "OrganisationType",
                Inline = true,
                RadioButtons =
                [
                    new RadioButtonsLabelViewModel
                    {
                        Name = "Option 1",
                        Id = "option1",
                        Value = "value1",
                        Hint = "First option hint",
                        DisplayAsOr = true,
                        Input = new TextFieldInputViewModel
                        {
                            Id = "textarea1",
                            ValidationMessage = "This is required.",
                            Paragraph = "Please provide input for option 1.",
                            Value = "Some text",
                            IsTextArea = true
                        }
                    },
                    new RadioButtonsLabelViewModel
                    {
                        Name = "Option 2",
                        Id = "option2",
                        Value = "value2"
                    }
                ],
                Value = "value1"
            };

            // Act & Assert
            Assert.Equal("Select the type of establishment to add", model.Heading);
            Assert.Equal("govuk-label govuk-label--l", model.HeadingStyle);
            Assert.Equal("Please select an option", model.Hint);
            Assert.Equal("Select an option", model.ErrorMessage);
            Assert.Equal("OrganisationType", model.Name);
            Assert.True(model.Inline);
            Assert.Equal(2, model.RadioButtons.Count);
            Assert.Equal("value1", model.Value);
        }

        [Fact]
        public void RadioButtonViewModel_ShouldAllowNullValues()
        {
            // Arrange
            var model = new RadioButtonViewModel
            {
                Heading = null,
                HeadingStyle = null,
                Hint = null,
                ErrorMessage = null,
                Name = null,
                RadioButtons = [],
                Value = null
            };

            // Act & Assert
            Assert.Null(model.Heading);
            Assert.Null(model.HeadingStyle);
            Assert.Null(model.Hint);
            Assert.Null(model.ErrorMessage);
            Assert.Null(model.Name);
            Assert.Empty(model.RadioButtons);
            Assert.Null(model.Value);
        }
    }

    public class RadioButtonsLabelViewModelTests
    {
        [Fact]
        public void RadioButtonsLabelViewModel_ShouldHaveDefaultValues()
        {
            // Arrange & Act
            var model = new RadioButtonsLabelViewModel
            {
                Name = "Test Option",
                Id = "testOption"
            };

            // Assert
            Assert.Equal("Test Option", model.Name);
            Assert.Equal("testOption", model.Id);
            Assert.Null(model.Value);
            Assert.Null(model.Hint);
            Assert.Null(model.Input);
            Assert.False(model.DisplayAsOr);
        }

        [Fact]
        public void RadioButtonsLabelViewModel_ShouldSetAndGetValues()
        {
            // Arrange
            var model = new RadioButtonsLabelViewModel
            {
                Name = "Option 1",
                Id = "option1",
                Value = "value1",
                Hint = "Option hint",
                DisplayAsOr = true,
                Input = new TextFieldInputViewModel
                {
                    Id = "textarea1",
                    ValidationMessage = "This is required.",
                    Paragraph = "Please provide input for option 1.",
                    Value = "Some text",
                    IsTextArea = true
                }
            };

            // Act & Assert
            Assert.Equal("Option 1", model.Name);
            Assert.Equal("option1", model.Id);
            Assert.Equal("value1", model.Value);
            Assert.Equal("Option hint", model.Hint);
            Assert.True(model.DisplayAsOr);
            Assert.NotNull(model.Input);
            Assert.Equal("textarea1", model.Input?.Id);
            Assert.Equal("Some text", model.Input?.Value);
        }

        [Fact]
        public void RadioButtonsLabelViewModel_DisplayAsOr_ShouldAllowNull()
        {
            // Arrange
            var model = new RadioButtonsLabelViewModel
            {
                Name = "Option 1",
                Id = "option1",
                DisplayAsOr = null
            };

            // Act & Assert
            Assert.Null(model.DisplayAsOr);
        }
    }

    public class TextFieldInputViewModelTests
    {
        [Fact]
        public void TextFieldInputViewModel_ShouldHaveDefaultValues()
        {
            // Arrange & Act
            var model = new TextFieldInputViewModel
            {
                Id = "textarea1",
                ValidationMessage = "This is required.",
                Paragraph = "Please provide input.",
                Value = "Some input"
            };

            // Assert
            Assert.Equal("textarea1", model.Id);
            Assert.Equal("This is required.", model.ValidationMessage);
            Assert.Equal("Please provide input.", model.Paragraph);
            Assert.Equal("Some input", model.Value);
            Assert.True(model.IsValid);
            Assert.True(model.IsTextArea);
        }

        [Fact]
        public void TextFieldInputViewModel_ShouldSetAndGetValues()
        {
            // Arrange
            var model = new TextFieldInputViewModel
            {
                Id = "textarea1",
                ValidationMessage = "This is required.",
                Paragraph = "Please provide input.",
                Value = "Test input",
                IsValid = false,
                IsTextArea = false
            };

            // Act & Assert
            Assert.Equal("textarea1", model.Id);
            Assert.Equal("This is required.", model.ValidationMessage);
            Assert.Equal("Please provide input.", model.Paragraph);
            Assert.Equal("Test input", model.Value);
            Assert.False(model.IsValid);
            Assert.False(model.IsTextArea);
        }

        [Fact]
        public void TextFieldInputViewModel_ShouldAllowNullValue()
        {
            // Arrange
            var model = new TextFieldInputViewModel
            {
                Id = "textarea1",
                ValidationMessage = "This is required.",
                Paragraph = "Please provide input.",
                Value = null
            };

            // Act & Assert
            Assert.Null(model.Value);
        }
    }
}
