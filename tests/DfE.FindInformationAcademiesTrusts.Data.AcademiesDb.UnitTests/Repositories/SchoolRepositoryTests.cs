using System.Collections.ObjectModel;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.AcademiesDbServices;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Http;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Models.Gias;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Models.Tad;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Repositories;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.School;
using GovUK.Dfe.AcademiesApi.Client.Contracts;
using GovUK.Dfe.PersonsApi.Client.Contracts;
using Microsoft.Extensions.Logging;
using NSubstitute.ExceptionExtensions;
using EstablishmentDto = GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Establishments.EstablishmentDto;
using NameAndCodeDto = GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Establishments.NameAndCodeDto;

namespace DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.UnitTests.Repositories;

public class SchoolRepositoryTests
{
    private readonly SchoolRepository _sut;
    private readonly MockAcademiesDbContext _mockAcademiesDbContext = new();
    private readonly IGetEstablishments _mockGetEstablishments;

    private readonly IStringFormattingUtilities _stringFormattingUtilities = new StringFormattingUtilities();
    private readonly ILogger<SchoolRepository> _mockLogger = MockLogger.CreateLogger<SchoolRepository>();

    public SchoolRepositoryTests()
    {
        _mockGetEstablishments = Substitute.For<IGetEstablishments>();
        _sut = new SchoolRepository(_mockAcademiesDbContext.Object, _stringFormattingUtilities, _mockLogger,
            _mockGetEstablishments);
    }

    [Fact]
    public async Task GetSchoolContactsAsync_should_return_headteacher_from_tad()
    {
        var urn = 45678;

        _mockAcademiesDbContext.TadHeadTeacherContacts.Add(new TadHeadTeacherContact
        {
            Urn = urn,
            HeadFirstName = "Teacher",
            HeadLastName = "McTeacherson",
            HeadEmail = "a.teacher@school.com"
        });

        var result = await _sut.GetSchoolContactsAsync(urn);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Teacher McTeacherson");
        result.Email.Should().Be("a.teacher@school.com");
    }

    [Fact]
    public async Task GetSchoolContactsAsync_IfContactDoesNotExist_ShouldLogAndReturnNull()
    {
        var notFoundUrn = 1234;

        var result = await _sut.GetSchoolContactsAsync(notFoundUrn);

        result.Should().BeNull();
        _mockLogger.VerifyLogErrors($"Unable to find head teacher contact for school with URN {notFoundUrn}");
    }


    [Fact]
    public async Task GetSchoolContactsAsync_if_email_is_empty_string_email_should_be_null()
    {
        var urn = 45678;

        _mockAcademiesDbContext.TadHeadTeacherContacts.Add(new TadHeadTeacherContact
        {
            Urn = urn,
            HeadFirstName = "Teacher",
            HeadLastName = "McTeacherson",
            HeadEmail = string.Empty
        });

        var result = await _sut.GetSchoolContactsAsync(urn);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Teacher McTeacherson");
        result.Email.Should().BeNull();
    }

    [Fact]
    public async Task GetSchoolFederationDetailsAsync_should_return_correct_values()
    {
        var urn = 123456;
        var openedDate = "24/05/2024";
        var federationsCode = "12345";

        _mockAcademiesDbContext.GiasEstablishments.AddRange(
        [
            new GiasEstablishment
            {
                Urn = urn,
                EstablishmentStatusName = "Open",
                EstablishmentName = "cool school",
                EstablishmentTypeGroupName = "Local authority maintained schools",
                FederationsName = "Funky Federation",
                FederationsCode = federationsCode
            },
            new GiasEstablishment
            {
                Urn = urn + 1,
                EstablishmentStatusName = "Open",
                EstablishmentName = "super school",
                EstablishmentTypeGroupName = "Local authority maintained schools",
                FederationsName = "Funky Federation",
                FederationsCode = federationsCode
            },
            new GiasEstablishment
            {
                Urn = urn + 2,
                EstablishmentStatusName = "Open",
                EstablishmentName = "amazing school",
                EstablishmentTypeGroupName = "Local authority maintained schools",
                FederationsName = "Funky Federation",
                FederationsCode = federationsCode
            }
        ]);

        _mockAcademiesDbContext.GiasGroupLinks.AddRange(
        [
            new GiasGroupLink
            {
                Urn = urn.ToString(),
                GroupUid = federationsCode,
                GroupStatusCode = "OPEN",
                OpenDate = openedDate
            }
        ]);

        var result = await _sut.GetSchoolFederationDetailsAsync(urn);
        result.FederationName.Should().BeEquivalentTo("Funky Federation");
        result.FederationUid.Should().BeEquivalentTo(federationsCode);
        result.OpenedOnDate.Should().Be(new DateOnly(2024, 05, 24));
        result.Schools.Should().BeEquivalentTo(new Dictionary<string, string>
        {
            { urn.ToString(), "cool school" },
            { (urn + 1).ToString(), "super school" },
            { (urn + 2).ToString(), "amazing school" }
        });
    }

    [Fact]
    public async Task GetSchoolFederationDetailsAsync_should_return_null_values_if_no_federation()
    {
        var urn = 123456;

        _mockAcademiesDbContext.GiasEstablishments.AddRange(
        [
            new GiasEstablishment
            {
                Urn = urn,
                EstablishmentStatusName = "Open",
                EstablishmentName = "cool school",
                EstablishmentTypeGroupName = "Local authority maintained schools"
            }
        ]);

        var result = await _sut.GetSchoolFederationDetailsAsync(urn);
        result.Should().BeEquivalentTo(new FederationDetails(null, null));
    }

    [Fact]
    public async Task IsPartOfFederationAsync_should_return_false_if_not_part_of_federation()
    {
        var urn = 8489479;

        _mockAcademiesDbContext.GiasEstablishments.AddRange(
        [
            new GiasEstablishment
            {
                Urn = urn,
                EstablishmentStatusName = "Open",
                EstablishmentName = "cool school",
                EstablishmentTypeGroupName = "Local authority maintained schools",
                FederationsCode = null
            }
        ]);

        var result = await _sut.IsPartOfFederationAsync(urn);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsPartOfFederationAsync_should_return_true_if_has_federation_details()
    {
        var urn = 4589748;

        _mockAcademiesDbContext.GiasEstablishments.AddRange(
        [
            new GiasEstablishment
            {
                Urn = urn,
                EstablishmentStatusName = "Open",
                EstablishmentName = "cool school",
                EstablishmentTypeGroupName = "Local authority maintained schools",
                FederationsCode = "Fed1"
            }
        ]);

        var result = await _sut.IsPartOfFederationAsync(urn);

        result.Should().BeTrue();
    }
    
    [Fact]
    public async Task GetReligiousCharacteristicsAsync_should_return_religious_characteristics()
    {
        var urn = 123456;

        _mockGetEstablishments.GetEstablishment(urn)
            .Returns(new EstablishmentDto
            {
                Urn = urn.ToString(),
                Name = "Test School",
                Diocese = new NameAndCodeDto
                {
                    Name = "Diocese of Nottingham"
                },
                ReligiousCharacter = new NameAndCodeDto
                {
                    Name = "Roman Catholic"
                },
                ReligousEthos = "Church of England/Roman Catholic"
            });

        var result = await _sut.GetReligiousCharacteristicsAsync(urn);

        result.Should().BeEquivalentTo(new ReligiousCharacteristics("Diocese of Nottingham", "Roman Catholic",
            "Church of England/Roman Catholic"));
    }
}
