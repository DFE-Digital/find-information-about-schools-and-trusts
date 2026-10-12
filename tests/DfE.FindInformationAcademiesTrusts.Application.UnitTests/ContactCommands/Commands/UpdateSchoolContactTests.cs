using DfE.FindInformationAcademiesTrusts.Application.ContactCommands.Commands;
using DfE.FindInformationAcademiesTrusts.Domain.Entities;
using DfE.FindInformationAcademiesTrusts.Domain.Enums;
using DfE.FindInformationAcademiesTrusts.Domain.Interfaces.Repositories;
using Moq;

namespace DfE.FindInformationAcademiesTrusts.Application.UnitTests.ContactCommands.Commands;

public class UpdateSchoolContactTests
{
    private readonly Mock<ISchoolContactRepository> _mockContactRepository = new();
    private readonly UpdateSchoolContact.UpdateSchoolContactCommandHandler _handler;

    public UpdateSchoolContactTests()
    {
        _handler = new UpdateSchoolContact.UpdateSchoolContactCommandHandler(_mockContactRepository.Object);
    }

    [Fact]
    public async Task Handle_NoExistingContact_AddsContactAndReportsBothUpdated()
    {
        var command = new UpdateSchoolContactCommand(123456, SchoolContactRole.RegionsGroupLocalAuthorityLead, "Name", "name@education.gov.uk");

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.NameUpdated);
        Assert.True(result.EmailUpdated);
        _mockContactRepository.Verify(repo => repo.AddAsync(
            It.Is<SchoolContact>(c => c.Urn == 123456 && c.Role == SchoolContactRole.RegionsGroupLocalAuthorityLead && c.Name == "Name" &&
                                     c.Email == "name@education.gov.uk"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("Old", "old@education.gov.uk", false, false)]
    [InlineData("New", "old@education.gov.uk", true, false)]
    [InlineData("Old", "new@education.gov.uk", false, true)]
    [InlineData("New", "new@education.gov.uk", true, true)]
    public async Task Handle_ExistingContact_UpdatesAndReportsWhatChanged(string name, string email,
        bool expectedNameUpdated, bool expectedEmailUpdated)
    {
        var existing = new SchoolContact
            { Urn = 123456, Role = SchoolContactRole.RegionsGroupLocalAuthorityLead, Name = "Old", Email = "old@education.gov.uk" };
        _mockContactRepository
            .Setup(repo => repo.GetSchoolContactAsync(123456, SchoolContactRole.RegionsGroupLocalAuthorityLead, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var result = await _handler.Handle(
            new UpdateSchoolContactCommand(123456, SchoolContactRole.RegionsGroupLocalAuthorityLead, name, email), CancellationToken.None);

        Assert.Equal(expectedNameUpdated, result.NameUpdated);
        Assert.Equal(expectedEmailUpdated, result.EmailUpdated);
        Assert.Equal(name, existing.Name);
        Assert.Equal(email, existing.Email);
        _mockContactRepository.Verify(repo => repo.UpdateAsync(existing, It.IsAny<CancellationToken>()), Times.Once);
        _mockContactRepository.Verify(
            repo => repo.AddAsync(It.IsAny<SchoolContact>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_NullNameAndEmail_SavesEmptyStrings()
    {
        await _handler.Handle(new UpdateSchoolContactCommand(123456, SchoolContactRole.RegionsGroupLocalAuthorityLead, null, null),
            CancellationToken.None);

        _mockContactRepository.Verify(repo => repo.AddAsync(
            It.Is<SchoolContact>(c => c.Name == string.Empty && c.Email == string.Empty),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
