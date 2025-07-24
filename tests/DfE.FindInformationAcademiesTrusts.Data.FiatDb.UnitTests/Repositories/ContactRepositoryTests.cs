using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Data.FiatDb.Models;
using DfE.FindInformationAcademiesTrusts.Data.FiatDb.Repositories;
using FluentAssertions.Execution;

namespace DfE.FindInformationAcademiesTrusts.Data.FiatDb.UnitTests.Repositories;

public class ContactRepositoryTests : BaseFiatDbTest
{
    private readonly ContactRepository _sut;

    public ContactRepositoryTests(FiatDbContainerFixture fiatDbContainerFixture) : base(fiatDbContainerFixture)
    {
        _sut = new ContactRepository(FiatDbContext);
    }

    [Fact]
    public async Task GetTrustInternalContactsAsync_should_return_nulls_when_there_are_no_contacts_for_trust()
    {
        var result = await _sut.GetTrustInternalContactsAsync("1234");

        using (new AssertionScope())
        {
            result.TrustRelationshipManager.Should().BeNull();
            result.SfsoLead.Should().BeNull();
        }
    }

    [Fact]
    public async Task GetTrustInternalContactsAsync_should_return_TrustRelationshipManager_when_there_is_no_SfsoLead()
    {
        FiatDbContext.TrustContacts.Add(new TrustContact
        {
            Email = "trm@testemail.com",
            Name = "Trust Relationship Manager",
            Role = TrustContactRole.TrustRelationshipManager,
            Uid = 1234
        });
        await FiatDbContext.SaveChangesAsync();

        var result = await _sut.GetTrustInternalContactsAsync("1234");

        using (new AssertionScope())
        {
            result.TrustRelationshipManager.Should().NotBeNull();
            result.TrustRelationshipManager!.FullName.Should().Be("Trust Relationship Manager");
            result.TrustRelationshipManager!.Email.Should().Be("trm@testemail.com");

            result.SfsoLead.Should().BeNull();
        }
    }

    [Fact]
    public async Task GetTrustInternalContactsAsync_should_return_SfsoLead_when_there_is_no_TrustRelationshipManager()
    {
        FiatDbContext.TrustContacts.Add(new TrustContact
        {
            Email = "sfsolead@testemail.com",
            Name = "SFSO Lead",
            Role = TrustContactRole.SfsoLead,
            Uid = 1234
        });
        await FiatDbContext.SaveChangesAsync();

        var result = await _sut.GetTrustInternalContactsAsync("1234");

        using (new AssertionScope())
        {
            result.SfsoLead.Should().NotBeNull();
            result.SfsoLead!.FullName.Should().Be("SFSO Lead");
            result.SfsoLead!.Email.Should().Be("sfsolead@testemail.com");

            result.TrustRelationshipManager.Should().BeNull();
        }
    }

    [Fact]
    public async Task
        GetTrustInternalContactsAsync_should_return_SfsoLead_and_TrustRelationshipManager_when_both_are_present()
    {
        await FiatDbContext.TrustContacts.AddRangeAsync(
            new TrustContact
            {
                Email = "trm@testemail.com",
                Name = "Trust Relationship Manager",
                Role = TrustContactRole.TrustRelationshipManager,
                Uid = 1234
            },
            new TrustContact
            {
                Email = "sfsolead@testemail.com",
                Name = "SFSO Lead",
                Role = TrustContactRole.SfsoLead,
                Uid = 1234
            }
        );
        await FiatDbContext.SaveChangesAsync();

        var result = await _sut.GetTrustInternalContactsAsync("1234");

        using (new AssertionScope())
        {
            result.TrustRelationshipManager.Should().NotBeNull();
            result.TrustRelationshipManager!.FullName.Should().Be("Trust Relationship Manager");
            result.TrustRelationshipManager!.Email.Should().Be("trm@testemail.com");

            result.SfsoLead.Should().NotBeNull();
            result.SfsoLead!.FullName.Should().Be("SFSO Lead");
            result.SfsoLead!.Email.Should().Be("sfsolead@testemail.com");
        }
    }

    [Theory]
    [InlineData(TrustContactRole.TrustRelationshipManager)]
    [InlineData(TrustContactRole.SfsoLead)]
    public async Task UpdateTrustInternalContactsAsync_should_be_able_to_add_new_contact(TrustContactRole role)
    {
        var result = await _sut.UpdateTrustInternalContactsAsync(1234, "New Name", "new@email.com", role);

        using (new AssertionScope())
        {
            result.NameUpdated.Should().BeTrue();
            result.EmailUpdated.Should().BeTrue();

            FiatDbContext.ChangeTracker.HasChanges().Should().BeFalse();

            var contact = FiatDbContext.TrustContacts.Single(c => c.Uid == 1234);
            contact.Name.Should().Be("New Name");
            contact.Email.Should().Be("new@email.com");
            contact.Role.Should().Be(role);
        }
    }

    [Theory]
    [InlineData(TrustContactRole.TrustRelationshipManager)]
    [InlineData(TrustContactRole.SfsoLead)]
    public async Task UpdateTrustInternalContactsAsync_should_be_able_to_update_both_email_and_name(
        TrustContactRole role)
    {
        var contact = new TrustContact
        {
            Email = "oldemail@testemail.com",
            Name = "Old Name",
            Role = role,
            Uid = 1234
        };
        FiatDbContext.TrustContacts.Add(contact);
        await FiatDbContext.SaveChangesAsync();

        var result = await _sut.UpdateTrustInternalContactsAsync(1234, "New Name", "new@email.com", role);

        using (new AssertionScope())
        {
            result.NameUpdated.Should().BeTrue();
            result.EmailUpdated.Should().BeTrue();

            FiatDbContext.ChangeTracker.HasChanges().Should().BeFalse();
            contact.Name.Should().Be("New Name");
            contact.Email.Should().Be("new@email.com");
        }
    }

    [Theory]
    [InlineData(TrustContactRole.TrustRelationshipManager)]
    [InlineData(TrustContactRole.SfsoLead)]
    public async Task UpdateTrustInternalContactsAsync_should_be_able_to_update_name_only(TrustContactRole role)
    {
        var contact = new TrustContact
        {
            Email = "oldemail@testemail.com",
            Name = "Old Name",
            Role = role,
            Uid = 1234
        };
        FiatDbContext.TrustContacts.Add(contact);
        await FiatDbContext.SaveChangesAsync();

        var result =
            await _sut.UpdateTrustInternalContactsAsync(1234, "New Name", contact.Email, role);

        using (new AssertionScope())
        {
            result.NameUpdated.Should().BeTrue();
            result.EmailUpdated.Should().BeFalse();

            FiatDbContext.ChangeTracker.HasChanges().Should().BeFalse();
            contact.Name.Should().Be("New Name");
            contact.Email.Should().Be("oldemail@testemail.com");
        }
    }

    [Theory]
    [InlineData(TrustContactRole.TrustRelationshipManager)]
    [InlineData(TrustContactRole.SfsoLead)]
    public async Task UpdateTrustInternalContactsAsync_should_be_able_to_update_email_only(TrustContactRole role)
    {
        var contact = new TrustContact
        {
            Email = "oldemail@testemail.com",
            Name = "Old Name",
            Role = role,
            Uid = 1234
        };
        FiatDbContext.TrustContacts.Add(contact);
        await FiatDbContext.SaveChangesAsync();

        var result =
            await _sut.UpdateTrustInternalContactsAsync(1234, contact.Name, "new@email.com", role);

        using (new AssertionScope())
        {
            result.NameUpdated.Should().BeFalse();
            result.EmailUpdated.Should().BeTrue();

            FiatDbContext.ChangeTracker.HasChanges().Should().BeFalse();
            contact.Name.Should().Be("Old Name");
            contact.Email.Should().Be("new@email.com");
        }
    }

    [Fact]
    public async Task UpdateTrustInternalContactsAsync_should_default_null_fields_to_empty_string_on_update()
    {
        var contact = new TrustContact
        {
            Email = "oldemail@testemail.com",
            Name = "Old Name",
            Role = TrustContactRole.SfsoLead,
            Uid = 1234
        };
        FiatDbContext.TrustContacts.Add(contact);
        await FiatDbContext.SaveChangesAsync();

        var result =
            await _sut.UpdateTrustInternalContactsAsync(1234, null, null, contact.Role);

        using (new AssertionScope())
        {
            result.NameUpdated.Should().BeTrue();
            result.EmailUpdated.Should().BeTrue();

            FiatDbContext.ChangeTracker.HasChanges().Should().BeFalse();
            contact.Name.Should().BeEmpty();
            contact.Email.Should().BeEmpty();
        }
    }

    [Fact]
    public async Task UpdateTrustInternalContactsAsync_should_default_null_fields_to_empty_string_on_new_contact()
    {
        var result =
            await _sut.UpdateTrustInternalContactsAsync(1234, null, null, TrustContactRole.TrustRelationshipManager);

        using (new AssertionScope())
        {
            result.NameUpdated.Should().BeTrue();
            result.EmailUpdated.Should().BeTrue();

            FiatDbContext.ChangeTracker.HasChanges().Should().BeFalse();
            var contact = FiatDbContext.TrustContacts.Single(c => c.Uid == 1234);
            contact.Name.Should().BeEmpty();
            contact.Email.Should().BeEmpty();
        }
    }

    [Fact]
    public async Task GetSchoolInternalContactsAsync_should_return_nulls_when_there_are_no_contacts_for_school()
    {
        var result = await _sut.GetSchoolInternalContactsAsync(123456);

        using (new AssertionScope())
        {
            result.RegionsGroupLocalAuthorityLead.Should().BeNull();
        }
    }

    [Fact]
    public async Task GetSchoolInternalContactsAsync_should_return_RegionsGroupLocalAuthorityLead_when_available()
    {
        FiatDbContext.SchoolContacts.Add(new SchoolContact
        {
            Email = "regions.group.local.authority.lead@education.gov.uk",
            Name = "Regions Group Local Authority Lead",
            Role = SchoolContactRole.RegionsGroupLocalAuthorityLead,
            Urn = 123456
        });
        await FiatDbContext.SaveChangesAsync();

        var result = await _sut.GetSchoolInternalContactsAsync(123456);

        using (new AssertionScope())
        {
            result.RegionsGroupLocalAuthorityLead.Should().NotBeNull();
            result.RegionsGroupLocalAuthorityLead!.FullName.Should().Be("Regions Group Local Authority Lead");
            result.RegionsGroupLocalAuthorityLead.Email.Should()
                .Be("regions.group.local.authority.lead@education.gov.uk");
        }
    }

    [Theory]
    [InlineData(SchoolContactRole.RegionsGroupLocalAuthorityLead)]
    public async Task UpdateSchoolInternalContactsAsync_should_be_able_to_add_new_contact(SchoolContactRole role)
    {
        var result = await _sut.UpdateSchoolInternalContactsAsync(123456, "New Name", "new@email.com", role);

        using (new AssertionScope())
        {
            result.NameUpdated.Should().BeTrue();
            result.EmailUpdated.Should().BeTrue();

            FiatDbContext.ChangeTracker.HasChanges().Should().BeFalse();

            var contact = FiatDbContext.SchoolContacts.Single(c => c.Urn == 123456);
            contact.Name.Should().Be("New Name");
            contact.Email.Should().Be("new@email.com");
            contact.Role.Should().Be(role);
        }
    }

    [Theory]
    [InlineData(SchoolContactRole.RegionsGroupLocalAuthorityLead)]
    public async Task UpdateSchoolInternalContactsAsync_should_be_able_to_update_both_email_and_name(
        SchoolContactRole role)
    {
        var contact = new SchoolContact
        {
            Email = "oldemail@testemail.com",
            Name = "Old Name",
            Role = role,
            Urn = 123456
        };
        FiatDbContext.SchoolContacts.Add(contact);
        await FiatDbContext.SaveChangesAsync();

        var result = await _sut.UpdateSchoolInternalContactsAsync(123456, "New Name", "new@email.com", role);

        using (new AssertionScope())
        {
            result.NameUpdated.Should().BeTrue();
            result.EmailUpdated.Should().BeTrue();

            FiatDbContext.ChangeTracker.HasChanges().Should().BeFalse();
            contact.Name.Should().Be("New Name");
            contact.Email.Should().Be("new@email.com");
        }
    }

    [Theory]
    [InlineData(SchoolContactRole.RegionsGroupLocalAuthorityLead)]
    public async Task UpdateSchoolInternalContactsAsync_should_be_able_to_update_name_only(SchoolContactRole role)
    {
        var contact = new SchoolContact
        {
            Email = "oldemail@testemail.com",
            Name = "Old Name",
            Role = role,
            Urn = 123456
        };
        FiatDbContext.SchoolContacts.Add(contact);
        await FiatDbContext.SaveChangesAsync();

        var result =
            await _sut.UpdateSchoolInternalContactsAsync(123456, "New Name", contact.Email, role);

        using (new AssertionScope())
        {
            result.NameUpdated.Should().BeTrue();
            result.EmailUpdated.Should().BeFalse();

            FiatDbContext.ChangeTracker.HasChanges().Should().BeFalse();
            contact.Name.Should().Be("New Name");
            contact.Email.Should().Be("oldemail@testemail.com");
        }
    }

    [Theory]
    [InlineData(SchoolContactRole.RegionsGroupLocalAuthorityLead)]
    public async Task UpdateSchoolInternalContactsAsync_should_be_able_to_update_email_only(SchoolContactRole role)
    {
        var contact = new SchoolContact
        {
            Email = "oldemail@testemail.com",
            Name = "Old Name",
            Role = role,
            Urn = 123456
        };
        FiatDbContext.SchoolContacts.Add(contact);
        await FiatDbContext.SaveChangesAsync();

        var result =
            await _sut.UpdateSchoolInternalContactsAsync(123456, contact.Name, "new@email.com", role);

        using (new AssertionScope())
        {
            result.NameUpdated.Should().BeFalse();
            result.EmailUpdated.Should().BeTrue();

            FiatDbContext.ChangeTracker.HasChanges().Should().BeFalse();
            contact.Name.Should().Be("Old Name");
            contact.Email.Should().Be("new@email.com");
        }
    }

    [Fact]
    public async Task UpdateSchoolInternalContactsAsync_should_default_null_fields_to_empty_string_on_update()
    {
        var contact = new SchoolContact
        {
            Email = "oldemail@testemail.com",
            Name = "Old Name",
            Role = SchoolContactRole.RegionsGroupLocalAuthorityLead,
            Urn = 123456
        };
        FiatDbContext.SchoolContacts.Add(contact);
        await FiatDbContext.SaveChangesAsync();

        var result =
            await _sut.UpdateSchoolInternalContactsAsync(123456, null, null, contact.Role);

        using (new AssertionScope())
        {
            result.NameUpdated.Should().BeTrue();
            result.EmailUpdated.Should().BeTrue();

            FiatDbContext.ChangeTracker.HasChanges().Should().BeFalse();
            contact.Name.Should().BeEmpty();
            contact.Email.Should().BeEmpty();
        }
    }

    [Fact]
    public async Task UpdateSchoolInternalContactsAsync_should_default_null_fields_to_empty_string_on_new_contact()
    {
        var result =
            await _sut.UpdateSchoolInternalContactsAsync(123456, null, null,
                SchoolContactRole.RegionsGroupLocalAuthorityLead);

        using (new AssertionScope())
        {
            result.NameUpdated.Should().BeTrue();
            result.EmailUpdated.Should().BeTrue();

            FiatDbContext.ChangeTracker.HasChanges().Should().BeFalse();
            var contact = FiatDbContext.SchoolContacts.Single(c => c.Urn == 123456);
            contact.Name.Should().BeEmpty();
            contact.Email.Should().BeEmpty();
        }
    }
}
