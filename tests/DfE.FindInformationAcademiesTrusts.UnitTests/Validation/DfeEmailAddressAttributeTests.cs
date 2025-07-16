using System.ComponentModel.DataAnnotations;
using DfE.FindInformationAcademiesTrusts.Validation;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Validation;

public class DfeEmailAddressAttributeTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(true)]
    [InlineData('X')]
    public void GetValidationResult_ShouldReturnNull_WhenValidatedObjectIsNotAString(object o)
    {
        var attribute = new DfeEmailAddressAttribute();

        var result = attribute.GetValidationResult(o, new ValidationContext(o));

        result.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("@")]
    [InlineData("foo@bar.com")]
    [InlineData("foo@")]
    [InlineData("@education.gov.uk")]
    [InlineData(".@education.gov.uk")]
    [InlineData("joe..bloggs@education.gov.uk")]
    [InlineData(".joe@education.gov.uk")]
    [InlineData("joe.@education.gov.uk")]
    [InlineData("!\"£$%^&*()@education.gov.uk")]
    [InlineData("${7*7}@education.gov.uk")]
    [InlineData("\"\"@education.gov.uk")]
    public void GetValidationResult_ShouldReturnError_WhenEmailAddressIsInvalid(string email)
    {
        var attribute = new DfeEmailAddressAttribute();

        var result = attribute.GetValidationResult(email, new ValidationContext(email));

        result.Should().NotBeNull();
        result!.ErrorMessage.Should().Be("Enter a DfE email address in the correct format, e.g. joe.bloggs@education.gov.uk");
    }

    [Theory]
    [InlineData("joe@education.gov.uk")]
    [InlineData("joe.bloggs@education.gov.uk")]
    [InlineData("david.lloyd.george@education.gov.uk")]
    [InlineData("JOE.BLOGGS@EDUCATION.GOV.UK")]
    public void GetValidationResult_ShouldReturnSuccess_WhenEmailAddressIsValid(string email)
    {
        var attribute = new DfeEmailAddressAttribute();

        var result = attribute.GetValidationResult(email, new ValidationContext(email));

        result.Should().Be(ValidationResult.Success);
    }
}
