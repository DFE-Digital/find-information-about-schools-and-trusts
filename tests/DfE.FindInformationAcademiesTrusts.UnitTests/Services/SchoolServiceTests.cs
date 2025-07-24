using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Data.Repositories.School;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.UnitTests.Mocks;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Services;

public class SchoolServiceTests
{
    private readonly SchoolService _sut;
    private readonly ISchoolRepository _mockSchoolRepository = Substitute.For<ISchoolRepository>();
    private readonly MockMemoryCache _mockMemoryCache = new();

    public SchoolServiceTests()
    {
        _sut = new SchoolService(_mockMemoryCache.Object, _mockSchoolRepository);
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
}
