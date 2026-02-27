using System.Globalization;
using DfE.FindInformationAcademiesTrusts.Extensions;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Extensions
{
    using DfE.FindInformationAcademiesTrusts.Data.Enums;

    public class BeforeOrAfterJoiningExtensionsTests
    {
        [Theory]
        [InlineData(BeforeOrAfterJoining.Before, "Before joining")]
        [InlineData(BeforeOrAfterJoining.After, "After joining")]
        [InlineData(BeforeOrAfterJoining.NotYetInspected, "Unknown")]
        public void ToDisplayString_ReturnsCorrectString_ForDefinedEnumValues(BeforeOrAfterJoining beforeOrAfterJoining, string expected)
        {
            // Act
            var result = beforeOrAfterJoining.ToDisplayString();

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(null, null, BeforeOrAfterJoining.NotApplicable)]
        [InlineData(null, "2024-01-01", BeforeOrAfterJoining.NotApplicable)]
        [InlineData("2024-01-01", null, BeforeOrAfterJoining.NotYetInspected)]
        [InlineData("2024-01-02", "2024-01-01", BeforeOrAfterJoining.Before)]
        [InlineData("2024-01-01", "2024-01-01", BeforeOrAfterJoining.After)]
        [InlineData("2024-01-01", "2024-01-02", BeforeOrAfterJoining.After)]
        public void GetBeforeOrAfterJoiningTrust_DateTime_Overload_ReturnsExpected(
            string? dateJoinedStr,
            string? inspectionDateStr,
            BeforeOrAfterJoining expected)
        {
            DateTime? dateJoined = dateJoinedStr is null ? null : DateTime.Parse(dateJoinedStr, CultureInfo.CurrentCulture);
            DateOnly? inspectionDate = inspectionDateStr is null ? null : DateOnly.Parse(inspectionDateStr, CultureInfo.CurrentCulture);

            var result = dateJoined.GetBeforeOrAfterJoiningTrust(inspectionDate);
                
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(null, null, BeforeOrAfterJoining.NotApplicable)]
        [InlineData(null, "2024-01-01", BeforeOrAfterJoining.NotApplicable)]
        [InlineData("2024-01-01", null, BeforeOrAfterJoining.NotYetInspected)]
        [InlineData("2024-01-02", "2024-01-01", BeforeOrAfterJoining.Before)]
        [InlineData("2024-01-01", "2024-01-01", BeforeOrAfterJoining.After)]
        [InlineData("2024-01-01", "2024-01-02", BeforeOrAfterJoining.After)]
        public void GetBeforeOrAfterJoiningTrust_DateOnly_Overload_ReturnsExpected(
            string? dateJoinedStr,
            string? inspectionDateStr,
            BeforeOrAfterJoining expected)
        {
            DateOnly? dateJoined = dateJoinedStr is null ? null : DateOnly.Parse(dateJoinedStr, CultureInfo.CurrentCulture);
            DateOnly? inspectionDate = inspectionDateStr is null ? null : DateOnly.Parse(inspectionDateStr, CultureInfo.CurrentCulture);

            var result = dateJoined.GetBeforeOrAfterJoiningTrust(inspectionDate);

            result.Should().Be(expected);
        }
    }
}
