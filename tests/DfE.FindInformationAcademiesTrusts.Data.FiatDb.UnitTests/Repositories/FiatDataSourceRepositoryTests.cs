using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Data.FiatDb.Models;
using DfE.FindInformationAcademiesTrusts.Data.FiatDb.Repositories;
using FluentAssertions.Execution;

namespace DfE.FindInformationAcademiesTrusts.Data.FiatDb.UnitTests.Repositories;

public class FiatDataSourceRepositoryTests : BaseFiatDbTest
{
    private readonly FiatDataSourceRepository _sut;

    public FiatDataSourceRepositoryTests(FiatDbContainerFixture fixture) : base(fixture)
    {
        _sut = new FiatDataSourceRepository(FiatDbContext);
    }

    [Fact]
    public async Task GetSchoolContactDataSourceAsync_returns_empty_datasource_when_no_contact_exists_for_school()
    {
        var result =
            await _sut.GetSchoolContactDataSourceAsync(123456, SchoolContactRole.RegionsGroupLocalAuthorityLead);

        using (new AssertionScope())
        {
            result.Source.Should().Be(Source.FiatDb);
            result.LastUpdated.Should().BeNull();
            result.NextUpdated.Should().BeNull();
            result.UpdatedBy.Should().BeNull();
        }
    }

    [Theory]
    [InlineData(SchoolContactRole.RegionsGroupLocalAuthorityLead)]
    public async Task
        GetSchoolContactDataSourceAsync_returns_empty_datasource_when_no_contact_with_role_exists_for_school(
            SchoolContactRole role)
    {
        FiatDbContext.SchoolContacts.Add(new SchoolContact
        {
            Urn = 123456,
            Role = role,
            Name = "Regions Group Local Authority Lead",
            Email = "regions.group.local.authority.lead@education.gov.uk"
        });
        await FiatDbContext.SaveChangesAsync();

        var result = await _sut.GetSchoolContactDataSourceAsync(123456, (SchoolContactRole)(-1));

        using (new AssertionScope())
        {
            result.Source.Should().Be(Source.FiatDb);
            result.LastUpdated.Should().BeNull();
            result.NextUpdated.Should().BeNull();
            result.UpdatedBy.Should().BeNull();
        }
    }

    [Theory]
    [InlineData(SchoolContactRole.RegionsGroupLocalAuthorityLead)]
    public async Task
        GetSchoolContactDataSourceAsync_returns_datasource_with_expected_data_when_contact_with_role_exists_for_school(
            SchoolContactRole role)
    {
        FiatDbContext.SchoolContacts.Add(new SchoolContact
        {
            Urn = 123456,
            Role = role,
            Name = "Regions Group Local Authority Lead",
            Email = "regions.group.local.authority.lead@education.gov.uk"
        });
        await FiatDbContext.SaveChangesAsync();

        var result = await _sut.GetSchoolContactDataSourceAsync(123456, role);

        using (new AssertionScope())
        {
            result.Source.Should().Be(Source.FiatDb);
            result.LastUpdated.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            result.NextUpdated.Should().BeNull();
            result.UpdatedBy.Should().Be("user@defaulttest");
        }
    }

    [Theory]
    [InlineData(TrustContactRole.TrustRelationshipManager)]
    [InlineData(TrustContactRole.SfsoLead)]
    public async Task GetTrustContactDataSourceAsync_returns_empty_datasource_when_no_contact_exists_for_trust(
        TrustContactRole role)
    {
        var result = await _sut.GetTrustContactDataSourceAsync(4321, role);

        using (new AssertionScope())
        {
            result.Source.Should().Be(Source.FiatDb);
            result.LastUpdated.Should().BeNull();
            result.NextUpdated.Should().BeNull();
            result.UpdatedBy.Should().BeNull();
        }
    }

    [Theory]
    [InlineData(TrustContactRole.TrustRelationshipManager, "Trust Relationship Manager", "trm@education.gov.uk")]
    [InlineData(TrustContactRole.SfsoLead, "SFSO Lead", "sfso@education.gov.uk")]
    public async Task
        GetTrustContactDataSourceAsync_returns_empty_datasource_when_no_contact_with_role_exists_for_trust(
            TrustContactRole role, string name, string email)
    {
        FiatDbContext.TrustContacts.Add(new TrustContact
        {
            Uid = 4321,
            Role = role,
            Name = name,
            Email = email
        });
        await FiatDbContext.SaveChangesAsync();

        var result = await _sut.GetTrustContactDataSourceAsync(4321, (TrustContactRole)(-1));

        using (new AssertionScope())
        {
            result.Source.Should().Be(Source.FiatDb);
            result.LastUpdated.Should().BeNull();
            result.NextUpdated.Should().BeNull();
            result.UpdatedBy.Should().BeNull();
        }
    }

    [Theory]
    [InlineData(TrustContactRole.TrustRelationshipManager, "Trust Relationship Manager", "trm@education.gov.uk")]
    [InlineData(TrustContactRole.SfsoLead, "SFSO Lead", "sfso@education.gov.uk")]
    public async Task
        GetTrustContactDataSourceAsync_returns_datasource_with_expected_data_when_contact_with_role_exists_for_trust(
            TrustContactRole role, string name, string email)
    {
        FiatDbContext.TrustContacts.Add(new TrustContact
        {
            Uid = 4321,
            Role = role,
            Name = name,
            Email = email
        });
        await FiatDbContext.SaveChangesAsync();

        var result = await _sut.GetTrustContactDataSourceAsync(4321, role);

        using (new AssertionScope())
        {
            result.Source.Should().Be(Source.FiatDb);
            result.LastUpdated.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            result.NextUpdated.Should().BeNull();
            result.UpdatedBy.Should().Be("user@defaulttest");
        }
    }
}
