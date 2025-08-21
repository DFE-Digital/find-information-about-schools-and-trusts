using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Data.Repositories;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.Ofsted;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.School;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.UnitTests.Mocks;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Services;

public class SchoolServiceTests
{
    private readonly SchoolService _sut;
    private readonly ISchoolRepository _mockSchoolRepository = Substitute.For<ISchoolRepository>();
    private readonly IOfstedRepository _mockOfstedRepository = Substitute.For<IOfstedRepository>();
    private readonly MockMemoryCache _mockMemoryCache = new();

    public SchoolServiceTests()
    {
        _sut = new SchoolService(_mockMemoryCache.Object, _mockSchoolRepository, _mockOfstedRepository);
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

        await _mockSchoolRepository.DidNotReceive().GetSchoolSummaryAsync(urn);
    }

    [Fact]
    public async Task GetSchoolSummaryAsync_should_return_null_if_not_found()
    {
        _mockSchoolRepository.GetSchoolSummaryAsync(999999).Returns((SchoolSummary?)null);

        var result = await _sut.GetSchoolSummaryAsync(999999);
        result.Should().BeNull();
    }

    [Theory]
    [InlineData(280689, "My School", "Foundation school", SchoolCategory.LaMaintainedSchool)]
    [InlineData(900855, "My Academy", "Academy converter", SchoolCategory.Academy)]
    public async Task GetSchoolSummaryAsync_should_return_schoolSummary_if_found(int urn, string name, string type,
        SchoolCategory category)
    {
        _mockSchoolRepository.GetSchoolSummaryAsync(urn).Returns(new SchoolSummary(name, type, category));

        var result = await _sut.GetSchoolSummaryAsync(urn);
        result.Should().BeEquivalentTo(new SchoolSummaryServiceModel(urn, name, type, category));
    }

    [Theory]
    [InlineData(280689, "My School", "Foundation school", SchoolCategory.LaMaintainedSchool)]
    [InlineData(900855, "My Academy", "Academy converter", SchoolCategory.Academy)]
    public async Task GetSchoolSummaryAsync_uncached_should_cache_result(int urn, string name, string type,
        SchoolCategory category)
    {
        var key = $"{nameof(SchoolService.GetSchoolSummaryAsync)}:{urn}";

        _mockSchoolRepository.GetSchoolSummaryAsync(urn).Returns(new SchoolSummary(name, type, category));

        await _sut.GetSchoolSummaryAsync(urn);

        _mockMemoryCache.Object.Received(1).CreateEntry(key);

        var cachedEntry = _mockMemoryCache.MockCacheEntries[key];

        cachedEntry.Value.Should().BeEquivalentTo(new SchoolSummaryServiceModel(urn, name, type, category));
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

    [Fact]
    public async Task GetReferenceNumbersAsync_should_return_only_urn_when_repository_returns_null()
    {
        const int urn = 123456;
        _mockSchoolRepository.GetReferenceNumbersAsync(urn).Returns((SchoolReferenceNumbers?)null);

        var result = await _sut.GetReferenceNumbersAsync(urn);

        result.Urn.Should().Be(urn);
        result.Laestab.Should().BeNull();
        result.Ukprn.Should().BeNull();
        await _mockSchoolRepository.Received(1).GetReferenceNumbersAsync(urn);
    }

    [Theory]
    [InlineData("123", "4567", "123/4567")]
    [InlineData("234", "5678", "234/5678")]
    [InlineData("345", "6789", "345/6789")]
    public async Task GetReferenceNumbersAsync_should_include_laestab_when_la_code_and_establishment_number_are_present(
        string laCode, string establishmentNumber, string expectedLaestab)
    {
        _mockSchoolRepository.GetReferenceNumbersAsync(123456)
            .Returns(new SchoolReferenceNumbers(laCode, establishmentNumber, null));

        var result = await _sut.GetReferenceNumbersAsync(123456);

        result.Laestab.Should().Be(expectedLaestab);
        await _mockSchoolRepository.Received(1).GetReferenceNumbersAsync(123456);
    }

    [Fact]
    public async Task GetReferenceNumbersAsync_should_not_include_laestab_when_la_code_is_missing()
    {
        const int urn = 123456;
        const string? laCode = null;
        const string establishmentNumber = "4567";

        _mockSchoolRepository.GetReferenceNumbersAsync(urn)
            .Returns(new SchoolReferenceNumbers(laCode, establishmentNumber, null));

        var result = await _sut.GetReferenceNumbersAsync(urn);

        result.Laestab.Should().BeNull();
        await _mockSchoolRepository.Received(1).GetReferenceNumbersAsync(urn);
    }

    [Fact]
    public async Task GetReferenceNumbersAsync_should_not_include_laestab_when_establishment_number_is_missing()
    {
        const int urn = 123456;
        const string laCode = "123";
        const string? establishmentNumber = null;

        _mockSchoolRepository.GetReferenceNumbersAsync(urn)
            .Returns(new SchoolReferenceNumbers(laCode, establishmentNumber, null));

        var result = await _sut.GetReferenceNumbersAsync(urn);

        result.Laestab.Should().BeNull();
        await _mockSchoolRepository.Received(1).GetReferenceNumbersAsync(urn);
    }

    [Fact]
    public async Task GetReferenceNumbersAsync_should_include_ukprn_when_present()
    {
        const int urn = 123456;
        const string ukprn = "12345678";

        _mockSchoolRepository.GetReferenceNumbersAsync(urn).Returns(new SchoolReferenceNumbers(null, null, ukprn));

        var result = await _sut.GetReferenceNumbersAsync(urn);

        result.Ukprn.Should().Be(ukprn);
        await _mockSchoolRepository.Received(1).GetReferenceNumbersAsync(urn);
    }

    [Fact]
    public async Task GetReferenceNumbersAsync_should_not_include_ukprn_when_missing()
    {
        const int urn = 123456;
        const string? ukprn = null;

        _mockSchoolRepository.GetReferenceNumbersAsync(urn).Returns(new SchoolReferenceNumbers(null, null, ukprn));

        var result = await _sut.GetReferenceNumbersAsync(urn);

        result.Ukprn.Should().BeNull();
        await _mockSchoolRepository.Received(1).GetReferenceNumbersAsync(urn);
    }

    [Fact]
    public async Task GetSchoolGovernanceAsync_should_return_governance_results()
    {
        const int urn = 123456;

        var startDate = DateTime.Today.AddYears(-3);
        var futureEndDate = DateTime.Today.AddYears(1);
        var historicEndDate = DateTime.Today.AddYears(-1);

        var governor = new Governor(
            "9999",
            "1234",
            Role: "Governor",
            FullName: "First Second Last",
            DateOfAppointment: startDate,
            DateOfTermEnd: futureEndDate,
            AppointingBody: "School board",
            Email: null
        );
        var chair = new Governor(
            "9998",
            "1234",
            Role: "Chair",
            FullName: "First Second Last",
            DateOfAppointment: startDate,
            DateOfTermEnd: futureEndDate,
            AppointingBody: "School board",
            Email: null
        );

        var historic = new Governor(
            "9999",
            "1234",
            Role: "Chair",
            FullName: "First Second Last",
            DateOfAppointment: startDate,
            DateOfTermEnd: historicEndDate,
            AppointingBody: "School board",
            Email: null
        );

        _mockSchoolRepository.GetGovernanceAsync(urn).Returns([governor, chair, historic]);

        var result = await _sut.GetSchoolGovernanceAsync(urn);

        result.Historic.Should().ContainSingle().Which.Should().BeEquivalentTo(historic);
        result.Current.Should().BeEquivalentTo([chair, governor]);
    }

    [Fact]
    public async Task GetOfstedHeadlineGradesAsync_returns_data_from_ofsted_repository()
    {
        const int urn = 123456;

        var expectedShortInspection = new OfstedShortInspection(new DateTime(2022, 1, 1), "School remains Good");
        var expectedCurrentInspection =
            new OfstedFullInspectionSummary(new DateTime(2021, 1, 1), OfstedRatingScore.Good);
        var expectedPreviousInspection =
            new OfstedFullInspectionSummary(new DateTime(2011, 1, 1), OfstedRatingScore.RequiresImprovement);

        _mockOfstedRepository.GetOfstedShortInspectionAsync(urn).Returns(expectedShortInspection);
        _mockOfstedRepository.GetOfstedInspectionHistorySummaryAsync(urn)
            .Returns(new OfstedInspectionHistorySummary(expectedCurrentInspection, expectedPreviousInspection));

        var result = await _sut.GetOfstedHeadlineGrades(urn);

        result.ShortInspection.Should().BeEquivalentTo(expectedShortInspection);
        result.CurrentInspection.Should().BeEquivalentTo(expectedCurrentInspection);
        result.PreviousInspection.Should().BeEquivalentTo(expectedPreviousInspection);
    }

    [Fact]
    public async Task GetSchoolOfstedRatingsAsync_returns_data_from_ofsted_repository()
    {
        const int urn = 123456;

        var expected = new SchoolOfsted("1", "Academy 1", new DateTime(2022, 12, 1),
            new OfstedShortInspection(new DateTime(2025, 7, 1), "School remains Good"),
            new OfstedRating((int)OfstedRatingScore.Good, new DateTime(2023, 1, 1)),
            new OfstedRating((int)OfstedRatingScore.RequiresImprovement, new DateTime(2023, 2, 1)));

        _mockOfstedRepository.GetSchoolOfstedRatingsAsync(urn).Returns(expected);

        var result = await _sut.GetSchoolOfstedRatingsAsync(urn);

        result.Should().BeEquivalentTo(expected);
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
