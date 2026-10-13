using DfE.FindInformationAcademiesTrusts.Data.FiatDb.Repositories;
using DfE.FindInformationAcademiesTrusts.Domain.Entities;
using DfE.FindInformationAcademiesTrusts.Domain.Enums;

namespace DfE.FindInformationAcademiesTrusts.Data.FiatDb.UnitTests.Repositories;

public class TrustContactRepositoryTests : BaseFiatDbTest
{
    private readonly TrustContactRepository _sut;

    public TrustContactRepositoryTests(FiatDbContainerFixture fiatDbContainerFixture) : base(fiatDbContainerFixture)
    {
        _sut = new TrustContactRepository(DbContext);
    }

    [Fact]
    public async Task GetTrustContactAsync_should_return_null_when_there_is_no_contact()
    {
        var result = await _sut.GetTrustContactAsync(1234, TrustContactRole.SfsoLead, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetTrustContactAsync_should_return_contact_for_uid_and_role()
    {
        DbContext.TrustContacts.AddRange(
            new TrustContact
            {
                Uid = 1234, Role = TrustContactRole.TrustRelationshipManager, Name = "TRM", Email = "trm@test.com"
            },
            new TrustContact { Uid = 1234, Role = TrustContactRole.SfsoLead, Name = "SFSO", Email = "sfso@test.com" },
            new TrustContact { Uid = 9999, Role = TrustContactRole.SfsoLead, Name = "Other", Email = "o@test.com" });
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetTrustContactAsync(1234, TrustContactRole.SfsoLead, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Name.Should().Be("SFSO");
        result.Email.Should().Be("sfso@test.com");
    }

    [Fact]
    public async Task UpdateAsync_should_persist_changes_to_a_fetched_contact()
    {
        DbContext.TrustContacts.Add(new TrustContact
            { Uid = 1234, Role = TrustContactRole.SfsoLead, Name = "Old", Email = "old@test.com" });
        await DbContext.SaveChangesAsync();
        DbContext.ChangeTracker.Clear();

        var contact = await _sut.GetTrustContactAsync(1234, TrustContactRole.SfsoLead, CancellationToken.None);
        contact!.Name = "New";
        await _sut.UpdateAsync(contact, CancellationToken.None);

        DbContext.ChangeTracker.Clear();
        DbContext.TrustContacts.Single(c => c.Uid == 1234).Name.Should().Be("New");
    }
}
