using DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Queries;
using DfE.FindInformationAcademiesTrusts.Domain.Entities;
using DfE.FindInformationAcademiesTrusts.Domain.Enums;
using DfE.FindInformationAcademiesTrusts.Domain.Interfaces.Repositories;
using Moq;

namespace DfE.FindInformationAcademiesTrusts.Application.UnitTests.ContactCommands.Queries;

public class GetContactQueriesTests
{
    private readonly Mock<ISchoolContactRepository> _mockSchoolContactRepository = new();
    private readonly Mock<ITrustContactRepository> _mockTrustContactRepository = new();

    private readonly SchoolContact _regionsGroupLocalAuthorityLead = new()
        { Urn = 123456, Role = SchoolContactRole.RegionsGroupLocalAuthorityLead, Name = "Lead", Email = "l@test.com" };

    private readonly TrustContact _trustRelationshipManager = new()
        { Uid = 1234, Role = TrustContactRole.TrustRelationshipManager, Name = "TRM", Email = "trm@test.com" };

    private readonly TrustContact _sfsoLead = new()
        { Uid = 1234, Role = TrustContactRole.SfsoLead, Name = "SFSO", Email = "sfso@test.com" };

    public GetContactQueriesTests()
    {
        _mockSchoolContactRepository.Setup(repo => repo.GetSchoolContactAsync(123456,
                SchoolContactRole.RegionsGroupLocalAuthorityLead, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_regionsGroupLocalAuthorityLead);
        _mockTrustContactRepository.Setup(repo => repo.GetTrustContactAsync(1234,
                TrustContactRole.TrustRelationshipManager, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_trustRelationshipManager);
        _mockTrustContactRepository.Setup(repo =>
                repo.GetTrustContactAsync(1234, TrustContactRole.SfsoLead, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_sfsoLead);
    }

    [Fact]
    public async Task GetTrustContact_ReturnsContactFromRepository()
    {
        var handler = new GetTrustContact.GetTrustContactQueryHandler(_mockTrustContactRepository.Object);

        var result = await handler.Handle(new GetTrustContactQuery(1234, TrustContactRole.SfsoLead),
            CancellationToken.None);

        Assert.Same(_sfsoLead, result);
    }

    [Fact]
    public async Task GetTrustContact_ReturnsNullWhenNoContact()
    {
        var handler = new GetTrustContact.GetTrustContactQueryHandler(_mockTrustContactRepository.Object);

        var result = await handler.Handle(new GetTrustContactQuery(9999, TrustContactRole.SfsoLead),
            CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetSchoolContact_ReturnsContactFromRepository()
    {
        var handler = new GetSchoolContact.GetSchoolContactQueryHandler(_mockSchoolContactRepository.Object);

        var result = await handler.Handle(
            new GetSchoolContactQuery(123456, SchoolContactRole.RegionsGroupLocalAuthorityLead),
            CancellationToken.None);

        Assert.Same(_regionsGroupLocalAuthorityLead, result);
    }

    [Fact]
    public async Task GetSchoolDfeContacts_WithTrust_ReturnsSchoolAndTrustContacts()
    {
        var handler = new GetSchoolDfeContacts.GetSchoolDfeContactsQueryHandler(_mockSchoolContactRepository.Object,
            _mockTrustContactRepository.Object);

        var result = await handler.Handle(new GetSchoolDfeContactsQuery(123456, 1234), CancellationToken.None);

        Assert.Same(_regionsGroupLocalAuthorityLead, result.RegionsGroupLocalAuthorityLead);
        Assert.Same(_trustRelationshipManager, result.TrustRelationshipManager);
        Assert.Same(_sfsoLead, result.SfsoLead);
    }

    [Fact]
    public async Task GetSchoolDfeContacts_WithoutTrust_ReturnsOnlySchoolContact()
    {
        var handler = new GetSchoolDfeContacts.GetSchoolDfeContactsQueryHandler(_mockSchoolContactRepository.Object,
            _mockTrustContactRepository.Object);

        var result = await handler.Handle(new GetSchoolDfeContactsQuery(123456, null), CancellationToken.None);

        Assert.Same(_regionsGroupLocalAuthorityLead, result.RegionsGroupLocalAuthorityLead);
        Assert.Null(result.TrustRelationshipManager);
        Assert.Null(result.SfsoLead);
        _mockTrustContactRepository.Verify(repo => repo.GetTrustContactAsync(It.IsAny<int>(),
            It.IsAny<TrustContactRole>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
