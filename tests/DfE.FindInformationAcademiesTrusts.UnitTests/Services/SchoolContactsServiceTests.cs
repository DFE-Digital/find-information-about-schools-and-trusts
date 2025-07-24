using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Data.FiatDb.Repositories;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Contacts;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.School;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Trust;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using FluentAssertions.Execution;
using NSubstitute.ReturnsExtensions;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Services;

public class SchoolContactsServiceTests
{
    private readonly int _urn = 100123;
    private readonly string _uid = "1234";
    private readonly SchoolContactsService _sut;
    private readonly ITrustService _mockTrustService = Substitute.For<ITrustService>();
    private readonly ISchoolRepository _mockSchoolRepository = Substitute.For<ISchoolRepository>();
    private readonly IContactRepository _mockContactsRepository = Substitute.For<IContactRepository>();

    private readonly SchoolContact _dummySchoolContact = new("Teacher McTeacherson", "a.teacher@school.com");

    private readonly SchoolInternalContacts _dummyInternalContacts =
        new(new InternalContact("Regions Group Local Authority Lead",
            "regions.group.local.authority.lead@education.gov.uk", DateTime.Now, "some.user@education.gov.uk"));

    public SchoolContactsServiceTests()
    {
        _sut = new SchoolContactsService(_mockTrustService, _mockSchoolRepository, _mockContactsRepository);
    }

    [Fact]
    public async Task GetInSchoolContactsAsync_should_set_contact_details_from_repository()
    {
        var expectedResult = new Person("Teacher McTeacherson", "a.teacher@school.com");

        _mockSchoolRepository.GetSchoolContactsAsync(_urn).Returns(_dummySchoolContact);

        var result = await _sut.GetInSchoolContactsAsync(_urn);

        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task GetInSchoolContactsAsync_should_default_null_name_to_empty_string()
    {
        var expectedResult = new Person(string.Empty, "a.teacher@school.com");

        _mockSchoolRepository.GetSchoolContactsAsync(_urn).Returns(_dummySchoolContact with { Name = null });

        var result = await _sut.GetInSchoolContactsAsync(_urn);

        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task GetInSchoolContactsAsync_ShouldReturnNull_IfDataIsNullFromRepository()
    {
        _mockSchoolRepository.GetSchoolContactsAsync(_urn).ReturnsNull();

        var result = await _sut.GetInSchoolContactsAsync(_urn);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetInternalContactsAsync_should_set_RegionsGroupLocalAuthorityLead_details_from_repository()
    {
        var regionsGroupLocalAuthorityLead = new Person("Regions Group Local Authority Lead",
            "regions.group.local.authority.lead@education.gov.uk");

        _mockContactsRepository.GetSchoolInternalContactsAsync(_urn).Returns(_dummyInternalContacts);

        var result = await _sut.GetInternalContactsAsync(_urn);

        result.RegionsGroupLocalAuthorityLead.Should().BeEquivalentTo(regionsGroupLocalAuthorityLead);
    }

    [Fact]
    public async Task
        GetInternalContactsAsync_should_set_RegionsGroupLocalAuthorityLead_to_null_when_repository_does_not_provide_one()
    {
        _mockContactsRepository.GetSchoolInternalContactsAsync(_urn).Returns(new SchoolInternalContacts(null));

        var result = await _sut.GetInternalContactsAsync(_urn);

        result.RegionsGroupLocalAuthorityLead.Should().BeNull();
    }

    [Fact]
    public async Task
        GetInternalContactsAsync_should_set_TrustRelationshipManager_and_SfsoLead_details_from_TrustService()
    {
        var trmContact = new InternalContact("Trust Relationship Manager", "trm@education.gov.uk",
            default, string.Empty);
        var trmPerson = new Person("Trust Relationship Manager", "trm@education.gov.uk");
        var sfsoLeadContact = new InternalContact("SFSO Lead", "sfso@education.gov.uk", default, string.Empty);
        var sfsoLeadPerson = new Person("SFSO Lead", "sfso@education.gov.uk");

        _mockContactsRepository.GetSchoolInternalContactsAsync(_urn).Returns(_dummyInternalContacts);
        _mockTrustService.GetTrustSummaryAsync(_urn)
            .Returns(new TrustSummaryServiceModel(_uid, "Some Trust", "Some Trust Type", 1));
        _mockContactsRepository.GetTrustInternalContactsAsync(_uid)
            .Returns(new TrustInternalContacts(trmContact, sfsoLeadContact));

        var result = await _sut.GetInternalContactsAsync(_urn);

        using (new AssertionScope())
        {
            result.TrustRelationshipManager.Should().BeEquivalentTo(trmPerson);
            result.SfsoLead.Should().BeEquivalentTo(sfsoLeadPerson);
        }
    }

    [Fact]
    public async Task
        GetInternalContactsAsync_should_set_null_TrustRelationshipManager_when_TrustService_does_not_provide_one()
    {
        var sfsoLeadContact = new InternalContact("SFSO Lead", "sfso@education.gov.uk", default, string.Empty);
        var sfsoLeadPerson = new Person("SFSO Lead", "sfso@education.gov.uk");

        _mockContactsRepository.GetSchoolInternalContactsAsync(_urn).Returns(_dummyInternalContacts);
        _mockTrustService.GetTrustSummaryAsync(_urn)
            .Returns(new TrustSummaryServiceModel(_uid, "Some Trust", "Some Trust Type", 1));
        _mockContactsRepository.GetTrustInternalContactsAsync(_uid)
            .Returns(new TrustInternalContacts(null, sfsoLeadContact));

        var result = await _sut.GetInternalContactsAsync(_urn);

        using (new AssertionScope())
        {
            result.TrustRelationshipManager.Should().BeNull();
            result.SfsoLead.Should().BeEquivalentTo(sfsoLeadPerson);
        }
    }

    [Fact]
    public async Task GetInternalContactsAsync_should_set_null_SfsoLead_when_TrustService_does_not_provide_one()
    {
        var trmContact = new InternalContact("Trust Relationship Manager", "trm@education.gov.uk",
            default, string.Empty);
        var trmPerson = new Person("Trust Relationship Manager", "trm@education.gov.uk");

        _mockContactsRepository.GetSchoolInternalContactsAsync(_urn).Returns(_dummyInternalContacts);
        _mockTrustService.GetTrustSummaryAsync(_urn)
            .Returns(new TrustSummaryServiceModel(_uid, "Some Trust", "Some Trust Type", 1));
        _mockContactsRepository.GetTrustInternalContactsAsync(_uid)
            .Returns(new TrustInternalContacts(trmContact, null));

        var result = await _sut.GetInternalContactsAsync(_urn);

        using (new AssertionScope())
        {
            result.TrustRelationshipManager.Should().BeEquivalentTo(trmPerson);
            result.SfsoLead.Should().BeNull();
        }
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public async Task UpdateContactsAsync_returns_the_correct_values_changed(bool emailUpdated, bool nameUpdated)
    {
        var expected = new InternalContactUpdated(emailUpdated, nameUpdated);
        _mockContactsRepository
            .UpdateSchoolInternalContactsAsync(123456, "Name", "Email",
                SchoolContactRole.RegionsGroupLocalAuthorityLead).Returns(Task.FromResult(expected));

        var result =
            await _sut.UpdateContactAsync(123456, "Name", "Email", SchoolContactRole.RegionsGroupLocalAuthorityLead);

        result.Should().BeEquivalentTo(expected);
    }
}
