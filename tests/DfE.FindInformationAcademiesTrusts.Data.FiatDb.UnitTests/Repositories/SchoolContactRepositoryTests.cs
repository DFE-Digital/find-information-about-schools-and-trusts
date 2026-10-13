using DfE.FindInformationAcademiesTrusts.Data.FiatDb.Repositories;
using DfE.FindInformationAcademiesTrusts.Domain.Entities;
using DfE.FindInformationAcademiesTrusts.Domain.Enums;

namespace DfE.FindInformationAcademiesTrusts.Data.FiatDb.UnitTests.Repositories;

public class SchoolContactRepositoryTests : BaseFiatDbTest
{
    private readonly SchoolContactRepository _sut;

    public SchoolContactRepositoryTests(FiatDbContainerFixture fiatDbContainerFixture) : base(fiatDbContainerFixture)
    {
        _sut = new SchoolContactRepository(DbContext);
    }

    [Fact]
    public async Task GetSchoolContactAsync_should_return_null_when_there_is_no_contact()
    {
        var result = await _sut.GetSchoolContactAsync(123456, SchoolContactRole.RegionsGroupLocalAuthorityLead, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetSchoolContactAsync_should_return_contact_for_urn_and_role()
    {
        DbContext.SchoolContacts.AddRange(
            new SchoolContact { Urn = 123456, Role = SchoolContactRole.RegionsGroupLocalAuthorityLead, Name = "SFSO", Email = "sfso@test.com" },
            new SchoolContact { Urn = 999999, Role = SchoolContactRole.RegionsGroupLocalAuthorityLead, Name = "Other", Email = "o@test.com" });
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetSchoolContactAsync(123456, SchoolContactRole.RegionsGroupLocalAuthorityLead, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Name.Should().Be("SFSO");
        result.Email.Should().Be("sfso@test.com");
    }

    [Fact]
    public async Task UpdateAsync_should_persist_changes_to_a_fetched_contact()
    {
        DbContext.SchoolContacts.Add(new SchoolContact
            { Urn = 123456, Role = SchoolContactRole.RegionsGroupLocalAuthorityLead, Name = "Old", Email = "old@test.com" });
        await DbContext.SaveChangesAsync();
        DbContext.ChangeTracker.Clear();

        var contact = await _sut.GetSchoolContactAsync(123456, SchoolContactRole.RegionsGroupLocalAuthorityLead, CancellationToken.None);
        contact!.Name = "New";
        await _sut.UpdateAsync(contact, CancellationToken.None);

        DbContext.ChangeTracker.Clear();
        DbContext.SchoolContacts.Single(c => c.Urn == 123456).Name.Should().Be("New");
    }
}
