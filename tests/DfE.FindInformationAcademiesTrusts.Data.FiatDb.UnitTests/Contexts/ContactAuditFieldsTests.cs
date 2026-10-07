using DfE.FindInformationAcademiesTrusts.Domain.Enums;
using DfE.FindInformationAcademiesTrusts.Domain.Entities;

namespace DfE.FindInformationAcademiesTrusts.Data.FiatDb.UnitTests.Contexts;

public class ContactAuditFieldsTests(FiatDbContainerFixture fiatDbContainerFixture)
    : BaseFiatDbTest(fiatDbContainerFixture)
{
    [Theory]
    [InlineData("Test User", "user@test")]
    [InlineData("Another User", "another@test")]
    public async Task SaveChangesAsync_should_set_lastmodified_from_userdetailsprovider_on_new_contact(
        string username, string email)
    {
        MockUserDetailsProvider.GetUserDetails().Returns((username, email));

        var entry = DbContext.TrustContacts.Add(new TrustContact
        {
            Name = "My TrustRelationshipManager",
            Email = "my.TrustRelationshipManager@education.gov.uk",
            Uid = 1234,
            Role = TrustContactRole.TrustRelationshipManager
        }).Entity;

        await DbContext.SaveChangesAsync();

        entry.LastModifiedByName.Should().Be(username);
        entry.LastModifiedByEmail.Should().Be(email);
    }

    [Theory]
    [InlineData("Test User", "user@test")]
    [InlineData("Another User", "another@test")]
    public void SaveChanges_should_set_lastmodified_from_userdetailsprovider_on_new_contact(string username,
        string email)
    {
        MockUserDetailsProvider.GetUserDetails().Returns((username, email));

        var entry = DbContext.SchoolContacts.Add(new SchoolContact
        {
            Name = "My RegionsGroupLocalAuthorityLead",
            Email = "my.RegionsGroupLocalAuthorityLead@education.gov.uk",
            Urn = 101234,
            Role = SchoolContactRole.RegionsGroupLocalAuthorityLead
        }).Entity;

        DbContext.SaveChanges();

        entry.LastModifiedByName.Should().Be(username);
        entry.LastModifiedByEmail.Should().Be(email);
    }

    [Fact]
    public async Task SaveChangesAsync_should_set_lastmodified_from_userdetailsprovider_on_updated_contact()
    {
        var entry = DbContext.TrustContacts.Add(new TrustContact
        {
            Name = "My TrustRelationshipManager",
            Email = "my.TrustRelationshipManager@education.gov.uk",
            Uid = 1234,
            Role = TrustContactRole.TrustRelationshipManager
        }).Entity;
        await DbContext.SaveChangesAsync();

        MockUserDetailsProvider.GetUserDetails().Returns(("Editing User", "editor@test"));
        entry.Name = "New Name";
        await DbContext.SaveChangesAsync();

        entry.LastModifiedByName.Should().Be("Editing User");
        entry.LastModifiedByEmail.Should().Be("editor@test");
    }

    [Fact]
    public async Task SaveChangesAsync_should_not_change_lastmodified_on_unchanged_contact()
    {
        var entry = DbContext.TrustContacts.Add(new TrustContact
        {
            Name = "My TrustRelationshipManager",
            Email = "my.TrustRelationshipManager@education.gov.uk",
            Uid = 1234,
            Role = TrustContactRole.TrustRelationshipManager
        }).Entity;
        await DbContext.SaveChangesAsync();

        MockUserDetailsProvider.GetUserDetails().Returns(("Other User", "other@test"));
        await DbContext.SaveChangesAsync();

        entry.LastModifiedByName.Should().Be("Default TestUser");
        entry.LastModifiedByEmail.Should().Be("user@defaulttest");
    }
}
