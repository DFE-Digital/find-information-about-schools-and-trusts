using DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Models;

namespace DfE.FindInformationAcademiesTrusts.Application.UnitTests.ContactCommands.Models;

public class ContactUpdatedTests
{
    [Theory]
    [InlineData(true, true, "Changes made to the SFSO lead name and email were updated.")]
    [InlineData(true, false, "Changes made to the SFSO lead name were updated.")]
    [InlineData(false, true, "Changes made to the SFSO lead email were updated.")]
    [InlineData(false, false, "")]
    public void ToMessage_DescribesWhatChanged(bool nameUpdated, bool emailUpdated, string expected)
    {
        Assert.Equal(expected, new ContactUpdated(nameUpdated, emailUpdated).ToMessage("SFSO lead"));
    }
}
