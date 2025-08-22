using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Extensions;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Extensions;

public class OfstedShortInspectionExtensionsTests
{
    [Theory]
    [InlineData(null, "Not available")]
    [InlineData("   \t\t    \t", "Not available")]
    [InlineData("School remains Good (Improving) - S5 Next", "School remains Good (Improving)")]
    [InlineData("School remains Outstanding", "School remains Outstanding")]
    public void ToOutcomeDisplayString_returns_correct_display_value(string? outcome, string expected)
    {
        var shortInspection = new OfstedShortInspection(DateTime.Now, outcome);

        var result = shortInspection.ToOutcomeDisplayString();
        
        result.Should().Be(expected);
    }
}
