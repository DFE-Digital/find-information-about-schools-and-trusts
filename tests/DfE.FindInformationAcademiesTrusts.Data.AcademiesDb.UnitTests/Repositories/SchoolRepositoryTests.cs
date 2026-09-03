using Dfe.AcademiesApi.Client.Contracts;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.AcademiesDbServices;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Http;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Models.Gias;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Models.Tad;
using DfE.FindInformationAcademiesTrusts.Data.AcademiesDb.Repositories;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.School;
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
    public async Task GetSchoolSummaryAsync_should_return_null_if_not_found()
    {
        var result = await _sut.GetSchoolSummaryAsync(999999);
        result.Should().BeNull();
    }

    [Theory]
    [InlineData(123456, "Academy converter", "Academies", SchoolCategory.Academy)]
    [InlineData(234567, "Sixth form centres", "Colleges", SchoolCategory.LaMaintainedSchool)]
    [InlineData(345678, "Free schools special", "Free Schools", SchoolCategory.LaMaintainedSchool)]
    [InlineData(456789, "Foundation school", "Local authority maintained schools", SchoolCategory.LaMaintainedSchool)]
    [InlineData(456789, "Non-maintained special school", "Special schools", SchoolCategory.LaMaintainedSchool)]
    public async Task GetSchoolSummaryAsync_should_return_schoolSummary_if_found(int urn, string type, string typeGroup,
        SchoolCategory expectedCategory)
    {
        var name = $"School {urn}";

        _mockGetEstablishments.GetEstablishment(urn)
            .Returns(new EstablishmentDto
            {
                Urn = urn.ToString(),
                Name = name,
                EstablishmentType = new ()
                {
                    Name = type
                },
                EstablishmentGroupType = new ()
                {
                    Name = typeGroup
                }
            });

        var result = await _sut.GetSchoolSummaryAsync(urn);
        result.Should().BeEquivalentTo(new SchoolSummary(name, type, expectedCategory));
    }

    // [Theory]
    // [InlineData("City technology college", "Independent schools")]
    // [InlineData("Online provider", "Online provider")]
    // [InlineData("Miscellaneous", "Other types")]
    // [InlineData("Higher education institutions", "Universities")]
    // public async Task GetSchoolSummaryAsync_should_not_return_schoolSummarys_for_unsupported_establishment_types(
    //     string type,
    //     string typeGroup)
    // {
    //     _mockAcademiesDbContext.GiasEstablishments.Add(new GiasEstablishment
    //     {
    //         Urn = 123456,
    //         EstablishmentName = "Unsupported Establishment",
    //         TypeOfEstablishmentName = type,
    //         EstablishmentTypeGroupName = typeGroup,
    //         EstablishmentStatusName = "Open"
    //     });
    //
    //     var result = await _sut.GetSchoolSummaryAsync(123456);
    //     result.Should().BeNull();
    // }

    [Fact]
    public async Task GetSchoolDetailsAsync_should_return_school_details()
    {
        var urn = 123456;

        _mockGetEstablishments.GetEstablishment(urn)
            .Returns(new EstablishmentDto
            {
                Urn = urn.ToString(),
                EstablishmentType = new()
                {
                    Name = "Foundation school"
                },
                EstablishmentGroupType = new()
                {
                    Name = "Local authority maintained schools"
                },
                Name = "cool school",
                Address = new()
                {
                    Street = "1st line",
                    Town = "Funky Town",
                    Postcode = "BBL 123",
                },
                Gor = new()
                {
                    Name = "Yorkshire"
                },
                LocalAuthorityName = "Leeds",
                PhaseOfEducation = new()
                {
                    Name = "Secondary"
                },
                StatutoryLowAge = "5",
                StatutoryHighAge = "16",
                NurseryProvision = "None",
                TrustName = null,
                DateJoinedTrust = null
            });

        var result = await _sut.GetSchoolDetailsAsync(urn);

        result.Should().BeEquivalentTo(new SchoolDetails("cool school", "1st line, Funky Town, BBL 123", "Yorkshire",
            "Leeds", "Secondary", new AgeRange(5, 16), "None", null, null));
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
    public async Task GetSchoolSenProvisionAsync_should_return_sen_provision()
    {
        var urn = 123456;

        _mockGetEstablishments.GetEstablishmentWithSenData(urn)
            .Returns(new EstablishmentResponse
            {
                Urn = urn.ToString(),
                EstablishmentName = "cool school",
                ResourcedProvisionOnRoll = "2",
                ResourcedProvisionOnCapacity = "3",
                SenUnitOnRoll = "22",
                SenUnitCapacity = "4",
                TypeOfResourcedProvision = "Resourced",
                SeN1 = "Sen1",
                SeN2 = "Sen2",
                SeN3 = "Sen3",
                SeN4 = "Sen4",
                SeN5 = "Sen5",
                SeN6 = "Sen6",
                SeN7 = "Sen7",
                SeN8 = "Sen8",
                SeN9 = "Sen9",
                SeN10 = "Sen10",
                SeN11 = "Sen11",
                SeN12 = "Sen12",
                SeN13 = "Sen13"
            });

        var result = await _sut.GetSchoolSenProvisionAsync(urn);

        result.Should().BeEquivalentTo(new SenProvision("2", "3", "22",
            "4", "Resourced", new List<string>
            {
                "Sen1", "Sen2", "Sen3", "Sen4", "Sen5", "Sen6", "Sen7", "Sen8", "Sen9", "Sen10", "Sen11", "Sen12",
                "Sen13"
            }));
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
    public async Task GetReferenceNumbersAsync_should_throw_if_not_found()
    {
        var urn = 123456;

        _mockGetEstablishments.GetEstablishment(urn)
            .ThrowsAsync(new ApiResponseException("Request to Api failed | StatusCode - 401"));

        var action = () => _sut.GetReferenceNumbersAsync(urn);

        await action.Should().ThrowAsync<ApiResponseException>()
            .WithMessage("Request to Api failed | StatusCode - 401");
    }

    [Theory]
    [InlineData(123456, "123", "4567", "10012345")]
    [InlineData(234567, "234", "5678", "10023456")]
    [InlineData(345678, "345", "6789", "10034567")]
    [InlineData(456789, "456", "7890", "10045678")]
    [InlineData(567890, "567", "8901", "10056789")]
    public async Task GetReferenceNumbersAsync_should_return_reference_numbers_if_found(int urn, string laCode,
        string establishmentNumber, string ukprn)
    {
        var name = $"School {urn}";

        _mockGetEstablishments.GetEstablishment(urn)
            .Returns(new EstablishmentDto
            {
                Urn = urn.ToString(),
                Name = name,
                LocalAuthorityCode = laCode,
                EstablishmentNumber = establishmentNumber,
                Ukprn = ukprn
            });

        var result = await _sut.GetReferenceNumbersAsync(urn);

        result.Should().NotBeNull();
        result!.LaCode.Should().Be(laCode);
        result.EstablishmentNumber.Should().Be(establishmentNumber);
        result.Ukprn.Should().Be(ukprn);
    }


    [Fact]
    public async Task GetGovernanceAsync_ShouldReturnEmpty_WithNoGovernanceSet()
    {
        var result = await _sut.GetGovernanceAsync(1234);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetGovernanceAsync_ShouldReturnExpectedData()
    {
        var urn = 123;

        var giasGovernance = new GiasGovernance
        {
            Urn = urn.ToString(),
            Forename1 = "testy",
            Forename2 = "mc",
            Surname = "testface",
            DateOfAppointment = DateTime.UtcNow.AddDays(-100).ToString("dd/MM/yyyy"),
            DateTermOfOfficeEndsEnded = DateTime.UtcNow.AddDays(100).ToString("dd/MM/yyyy"),
            AppointingBody = "trust members"
        };

        var giasGovernance2 = new GiasGovernance
        {
            Urn = "9876",
            Forename1 = "another",
            Surname = "govener",
            DateOfAppointment = DateTime.UtcNow.AddDays(-100).ToString("dd/MM/yyyy"),
            DateTermOfOfficeEndsEnded = DateTime.UtcNow.AddDays(100).ToString("dd/MM/yyyy"),
            AppointingBody = "trust members"
        };

        _mockAcademiesDbContext.GiasGovernances.AddRange([giasGovernance, giasGovernance2]);

        var result = await _sut.GetGovernanceAsync(urn);

        result.Length.Should().Be(1);
        result[0].FullName.Should().Be("testy mc testface");
        result[0].AppointingBody.Should().Be("trust members");
        result[0].DateOfAppointment.Should().Be(DateTime.UtcNow.AddDays(-100).Date);
        result[0].DateOfTermEnd.Should().Be(DateTime.UtcNow.AddDays(100).Date);
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
