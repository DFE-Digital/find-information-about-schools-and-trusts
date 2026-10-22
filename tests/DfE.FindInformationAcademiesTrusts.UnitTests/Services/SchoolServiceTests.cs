using System.Collections.ObjectModel;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Data.Repositories;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.School;
using DfE.FindInformationAcademiesTrusts.HttpServices;
using DfE.FindInformationAcademiesTrusts.Services.Ofsted;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.UnitTests.Mocks;
using GovUK.Dfe.CoreLibs.Contracts.Academies.V4.Establishments;
using GovUK.Dfe.PersonsApi.Client.Contracts;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Services;

public class SchoolServiceTests
{
    private readonly SchoolService _sut;
    private readonly ISchoolRepository _mockSchoolRepository = Substitute.For<ISchoolRepository>();
    private readonly IReportCardsService _mockReportCardsService = Substitute.For<IReportCardsService>();
    private readonly IGetEstablishmentsTemp _mockGetEstablishments;
    private readonly IEstablishmentsClient _mockEstablishmentsClient;

    private readonly MockMemoryCache _mockMemoryCache = new();

    public SchoolServiceTests()
    {
        _mockGetEstablishments = Substitute.For<IGetEstablishmentsTemp>();
        _mockEstablishmentsClient = Substitute.For<IEstablishmentsClient>();
        _sut = new SchoolService(_mockMemoryCache.Object, _mockGetEstablishments, _mockSchoolRepository, _mockEstablishmentsClient);

        _mockReportCardsService.GetReportCardsAsync(Arg.Any<int>()).Returns(new ReportCardServiceModel());
    }

    [Fact]
    public async Task GetSchoolSummaryAsync_cached_should_return_cached_result()
    {
        const int urn = 123456;
        var key = $"{nameof(SchoolService.GetSchoolSummaryAsync)}:{urn}";
        var cachedResult = new SchoolSummaryServiceModel(urn, "Chill primary school", "Academy sponsor led",
            SchoolCategory.LaMaintainedSchool);
        _mockMemoryCache.AddMockCacheEntry(key, cachedResult);

        var result = await _sut.GetSchoolSummaryAsync(urn);
        result.Should().Be(cachedResult);
    }

    [Theory]
    [InlineData(280689, "My School", "Foundation school", "Local authority maintained schools", SchoolCategory.LaMaintainedSchool)]
    [InlineData(900855, "My Academy", "Academy converter", "Academies", SchoolCategory.Academy)]
    public async Task GetSchoolSummaryAsync_should_return_schoolSummary_if_found(int urn, string name, string type,
        string category, SchoolCategory resultCategory)
    {
        _mockGetEstablishments.GetEstablishment(urn).Returns(new EstablishmentDto
        {
            Urn = urn.ToString(),
            Name = name,
            EstablishmentType = new()
            {
                Name = type
            },
            EstablishmentGroupType = new()
            {
                Name = category
            }
        });

        var result = await _sut.GetSchoolSummaryAsync(urn);
        result.Should().BeEquivalentTo(new SchoolSummaryServiceModel(urn, name, type, resultCategory));
    }

    [Theory]
    [InlineData(280689, "My School", "Foundation school", "Local authority maintained schools", SchoolCategory.LaMaintainedSchool)]
    [InlineData(900855, "My Academy", "Academy converter", "Academies", SchoolCategory.Academy)]
    public async Task GetSchoolSummaryAsync_uncached_should_cache_result(int urn, string name, string type, string category,
        SchoolCategory resultCategory)
    {
        var key = $"{nameof(SchoolService.GetSchoolSummaryAsync)}:{urn}";

        _mockGetEstablishments.GetEstablishment(urn).Returns(new EstablishmentDto
        {
            Urn = urn.ToString(),
            Name = name,
            EstablishmentType = new()
            {
                Name = type
            },
            EstablishmentGroupType = new()
            {
                Name = category
            }
        });

        await _sut.GetSchoolSummaryAsync(urn);

        _mockMemoryCache.Object.Received(1).CreateEntry(key);

        var cachedEntry = _mockMemoryCache.MockCacheEntries[key];

        cachedEntry.Value.Should().BeEquivalentTo(new SchoolSummaryServiceModel(urn, name, type, resultCategory));
        cachedEntry.SlidingExpiration.Should().Be(TimeSpan.FromMinutes(10));
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, false)]
    public async Task IsPartOfFederationAsync_should_return_repository_result(bool repositoryResult,
        bool expectedReturnValue)
    {
        var urn = 123456;

        _mockSchoolRepository.IsPartOfFederationAsync(urn).Returns(repositoryResult);

        var result = await _sut.IsPartOfFederationAsync(urn);

        await _mockSchoolRepository.Received(1).IsPartOfFederationAsync(urn);
        result.Should().Be(expectedReturnValue);
    }
    

    [Theory]
    [InlineData("123", "4567", "123/4567")]
    [InlineData("234", "5678", "234/5678")]
    [InlineData("345", "6789", "345/6789")]
    public async Task GetReferenceNumbersAsync_should_set_laestab_when_la_code_and_establishment_number_are_present(
        string laCode, string establishmentNumber, string expectedLaestab)
    {
        var urn = 123456;
        var name = $"School {urn}";

        _mockGetEstablishments.GetEstablishment(urn)
            .Returns(new EstablishmentDto
            {
                Urn = urn.ToString(),
                Name = name,
                LocalAuthorityCode = laCode,
                EstablishmentNumber = establishmentNumber
            });

        var result = await _sut.GetReferenceNumbersAsync(urn);

        result.Laestab.Should().Be(expectedLaestab);
        await _mockGetEstablishments.Received(1).GetEstablishment(urn);
    }

    [Fact]
    public async Task GetReferenceNumbersAsync_should_not_include_laestab_when_la_code_is_missing()
    {
        const int urn = 123456;
        const string establishmentNumber = "4567";

        _mockGetEstablishments.GetEstablishment(urn)
            .Returns(new EstablishmentDto
            {
                Urn = urn.ToString(),
                EstablishmentNumber = establishmentNumber
            });

        var result = await _sut.GetReferenceNumbersAsync(urn);

        result.Laestab.Should().BeNull();
        await _mockGetEstablishments.Received(1).GetEstablishment(urn);
    }

    [Fact]
    public async Task GetReferenceNumbersAsync_should_not_include_laestab_when_establishment_number_is_missing()
    {
        const int urn = 123456;
        const string laCode = "123";

        _mockGetEstablishments.GetEstablishment(urn)
            .Returns(new EstablishmentDto
            {
                Urn = urn.ToString(),
                LocalAuthorityCode = laCode
            });

        var result = await _sut.GetReferenceNumbersAsync(urn);

        result.Laestab.Should().BeNull();
        await _mockGetEstablishments.Received(1).GetEstablishment(urn);
    }

    [Fact]
    public async Task GetReferenceNumbersAsync_should_include_ukprn_when_present()
    {
        const int urn = 123456;
        const string ukprn = "12345678";

        _mockGetEstablishments.GetEstablishment(urn)
            .Returns(new EstablishmentDto
            {
                Urn = urn.ToString(),
                Ukprn = ukprn
            });
        var result = await _sut.GetReferenceNumbersAsync(urn);

        result.Ukprn.Should().Be(ukprn);
        await _mockGetEstablishments.Received(1).GetEstablishment(urn);
    }

    [Fact]
    public async Task GetReferenceNumbersAsync_should_not_include_ukprn_when_missing()
    {
        const int urn = 123456;

        _mockGetEstablishments.GetEstablishment(urn)
            .Returns(new EstablishmentDto
            {
                Urn = urn.ToString(),
                LocalAuthorityCode = "123",
                EstablishmentNumber = "456"
            });
        var result = await _sut.GetReferenceNumbersAsync(urn);

        result.Ukprn.Should().BeNull();
        await _mockGetEstablishments.Received(1).GetEstablishment(urn);
    }

    [Fact]
    public async Task GetSchoolGovernanceAsync_should_return_governance_results()
    {
        const int urn = 123456;

        var startDate = DateTime.Today.AddYears(-3);
        var futureEndDate = DateTime.Today.AddYears(1);
        var historicEndDate = DateTime.Today.AddYears(-1);

        var governor = new Governor(
            Role: "Governor",
            FullName: "First Second Last",
            DateOfAppointment: startDate,
            DateOfTermEnd: futureEndDate,
            AppointingBody: "School board",
            Email: null
        );
        var chair = new Governor(
            Role: "Chair",
            FullName: "First Second Last",
            DateOfAppointment: startDate,
            DateOfTermEnd: futureEndDate,
            AppointingBody: "School board",
            Email: null
        );

        var historic = new Governor(
            Role: "Chair",
            FullName: "First Second Last",
            DateOfAppointment: startDate,
            DateOfTermEnd: historicEndDate,
            AppointingBody: "School board",
            Email: null
        );

        _mockEstablishmentsClient.GetAllPersonsAssociatedWithAcademyByUrnAsync(urn)
            .Returns(new ObservableCollection<AcademyGovernance>
            {
                new AcademyGovernance
                {
                    Ukprn = "456",
                    Urn = urn,
                    DisplayName = "First Second Last",
                    Roles = ["Governor"] ,
                    DateOfAppointment = startDate.ToString("dd/MM/yyyy"),
                    DateTermOfOfficeEndsEnded = futureEndDate.ToString("dd/MM/yyyy"),
                    Email = null,
                    AppointingBody = "School board"
                },
                new AcademyGovernance
                {
                    Ukprn = "789",
                    Urn = urn,
                    DisplayName = "First Second Last",
                    Roles = ["Chair"] ,
                    DateOfAppointment = startDate.ToString("dd/MM/yyyy"),
                    DateTermOfOfficeEndsEnded = futureEndDate.ToString("dd/MM/yyyy"),
                    Email = null,
                    AppointingBody = "School board"
                },
                new AcademyGovernance
                {
                    Ukprn = "789",
                    Urn = urn,
                    DisplayName = "First Second Last",
                    Roles = ["Chair"] ,
                    DateOfAppointment = startDate.ToString("dd/MM/yyyy"),
                    DateTermOfOfficeEndsEnded = historicEndDate.ToString("dd/MM/yyyy"),
                    Email = null,
                    AppointingBody = "School board"
                }
            });

        var result = await _sut.GetSchoolGovernanceAsync(urn);

        result.Historic.Should().ContainSingle().Which.Should().BeEquivalentTo(historic);
        result.Current.Should().BeEquivalentTo([chair, governor]);
    }

    [Theory]
    [InlineData(null, "Not available")]
    [InlineData("", "Not available")]
    [InlineData("Not applicable", "Does not apply")]
    [InlineData("Roman Catholic", "Roman Catholic")]
    public async Task GetReligiousCharacteristicsAsync_for_authority_should_return_correct_data(string? authority,
        string expectedResult)
    {
        const int urn = 123456;

        _mockSchoolRepository.GetReligiousCharacteristicsAsync(urn)
            .Returns(new ReligiousCharacteristics(authority, null, null));

        var result = await _sut.GetReligiousCharacteristicsAsync(urn);

        result.ReligiousAuthority.Should().BeEquivalentTo(expectedResult);
    }

    [Theory]
    [InlineData(null, "Not available")]
    [InlineData("", "Not available")]
    [InlineData("Not applicable", "Not applicable")]
    [InlineData("Diocese of Nottingham", "Diocese of Nottingham")]
    public async Task GetReligiousCharacteristicsAsync_for_character_should_return_correct_data(string? character,
        string expectedResult)
    {
        const int urn = 123456;

        _mockSchoolRepository.GetReligiousCharacteristicsAsync(urn)
            .Returns(new ReligiousCharacteristics(null, character, null));

        var result = await _sut.GetReligiousCharacteristicsAsync(urn);

        result.ReligiousCharacter.Should().BeEquivalentTo(expectedResult);
    }

    [Theory]
    [InlineData(null, "Not available")]
    [InlineData("", "Not available")]
    [InlineData("Not applicable", "Not applicable")]
    [InlineData("Church of England/Roman Catholic", "Church of England/Roman Catholic")]
    public async Task GetReligiousCharacteristicsAsync_for_ethos_should_return_correct_data(string? ethos,
        string expectedResult)
    {
        const int urn = 123456;

        _mockSchoolRepository.GetReligiousCharacteristicsAsync(urn)
            .Returns(new ReligiousCharacteristics(null, null, ethos));

        var result = await _sut.GetReligiousCharacteristicsAsync(urn);

        result.ReligiousEthos.Should().BeEquivalentTo(expectedResult);
    }
}
