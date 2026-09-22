using DfE.FindInformationAcademiesTrusts.Data.Repositories.Search;
using DfE.FindInformationAcademiesTrusts.Services.Search;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Services;

public class SearchServiceTest
{
    private readonly SearchService _sut;

    private readonly ITrustSchoolSearchRepository _mockTrustSchoolSearchRepository =
        Substitute.For<ITrustSchoolSearchRepository>();

    private readonly int _pageSize = 20;

    public SearchServiceTest()
    {
        _sut = new SearchService(_mockTrustSchoolSearchRepository);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task GetSearchResultsForPageAsync_if_keywords_are_null_or_empty_should_return_empty_results(
        string? text)
    {
        var result = await _sut.GetSearchResultsForPageAsync(text, 1);

        result.ResultsList.Should().BeEmpty();
        result.ResultsOverview.Should().BeEquivalentTo(new SearchResultsOverview());
        await _mockTrustSchoolSearchRepository.Received(0).GetSearchResultsAsync(Arg.Any<string>(), Arg.Any<int>());
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task GetSearchResultsForAutocompleteAsync_if_keywords_are_null_or_empty_should_return_empty_results(
        string? text)
    {
        var result = await _sut.GetSearchResultsForAutocompleteAsync(text);

        result.Should().BeEmpty();
        await _mockTrustSchoolSearchRepository.Received(0).GetAutoCompleteSearchResultsAsync(Arg.Any<string>());
    }


    [Fact]
    public async Task GetSearchResultsForPageAsync_should_return_correct_mapped_models()
    {
        var searchText = "a";

        SearchResult trustResult = new("123456", "TR1234","A Cool Trust", "Multi-academy",
            "A street, Station Road, Town, GH1 8JH", true, "TR123");

        SearchResult schoolResult = new("65432", "TR2234","A Cool School", "Community school",
            "Another street, Station Road, Town, GH1 8JH", false, null);

        SearchResult[] results = [trustResult, schoolResult];

        var expectedTrustResult = new SearchResultServiceModel(trustResult.Id, trustResult.ReferenceNumber ,trustResult.Name, trustResult.Address,
            trustResult.TrustReferenceNumber, trustResult.Type, ResultType.Trust);

        var expectedSchoolResult = new SearchResultServiceModel(schoolResult.Id, schoolResult.ReferenceNumber,schoolResult.Name,
            schoolResult.Address, null, schoolResult.Type, ResultType.School);

        _mockTrustSchoolSearchRepository.GetSearchResultsAsync(searchText, _pageSize)
            .Returns((results, new SearchResultCount(results.Length, 1, 1)));

        var pagedSearchResults = await _sut.GetSearchResultsForPageAsync(searchText, 1);

        pagedSearchResults.ResultsList.Count.Should().Be(2);
        pagedSearchResults.ResultsList.PageStatus.TotalResults.Should().Be(2);
        pagedSearchResults.ResultsList.Should().Contain([expectedTrustResult, expectedSchoolResult]);
        pagedSearchResults.ResultsOverview.NumberOfSchools.Should().Be(1);
        pagedSearchResults.ResultsOverview.NumberOfTrusts.Should().Be(1);
    }

    [Fact]
    public async Task GetSearchResultsForAutocompleteAsync_should_return_correct_mapped_models()
    {
        var searchText = "a";

        SearchResult trustResult = new("123456", "TR1234","A Cool Trust", "Multi-academy",
            "A street, Station Road, Town, GH1 8JH", true, "TR123");

        SearchResult schoolResult = new("65432", null,"A Cool School", "Community school",
            "Another street, Station Road, Town, GH1 8JH", false, null);

        SearchResult[] results = [trustResult, schoolResult];

        var expectedTrustResult = new SearchResultServiceModel(trustResult.Id, trustResult.ReferenceNumber,trustResult.Name,
            "A street, Station Road, Town, GH1 8JH", trustResult.TrustReferenceNumber, trustResult.Type,
            ResultType.Trust);

        var expectedSchoolResult = new SearchResultServiceModel(schoolResult.Id, schoolResult.ReferenceNumber,schoolResult.Name,
            "Another street, Station Road, Town, GH1 8JH", null, schoolResult.Type,
            ResultType.School);

        _mockTrustSchoolSearchRepository.GetAutoCompleteSearchResultsAsync(searchText).Returns(results);

        var searchResults = await _sut.GetSearchResultsForAutocompleteAsync(searchText);

        searchResults.Length.Should().Be(2);
        searchResults.Should().Contain([expectedTrustResult, expectedSchoolResult]);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task GetSchoolSearchResultsForAutocompleteAsync_if_keywords_are_null_or_empty_should_return_empty_results(
        string? text)
    {
        var result = await _sut.GetSchoolSearchResultsForAutocompleteAsync(text);

        result.Should().BeEmpty();
        await _mockTrustSchoolSearchRepository.Received(0).GetSchoolAutoCompleteSearchResultsAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task GetSchoolSearchResultsForAutocompleteAsync_should_return_correct_mapped_models()
    {
        var searchText = "a";

        SearchResult schoolResult = new("65432", null, "A Cool School", "Community school",
            "Another street, Station Road, Town, GH1 8JH", false, "2001");

        SearchResult otherSchoolResult = new("65433", null, "Another Cool School", "Academy",
            "Third street, Station Road, Town, GH1 8JH", false, null);

        SearchResult[] results = [schoolResult, otherSchoolResult];

        var expectedSchoolResult = new SearchResultServiceModel(schoolResult.Id, schoolResult.ReferenceNumber,
            schoolResult.Name, "Another street, Station Road, Town, GH1 8JH", schoolResult.TrustReferenceNumber,
            schoolResult.Type, ResultType.School);

        var expectedOtherSchoolResult = new SearchResultServiceModel(otherSchoolResult.Id,
            otherSchoolResult.ReferenceNumber, otherSchoolResult.Name, "Third street, Station Road, Town, GH1 8JH",
            null, otherSchoolResult.Type, ResultType.School);

        _mockTrustSchoolSearchRepository.GetSchoolAutoCompleteSearchResultsAsync(searchText).Returns(results);

        var searchResults = await _sut.GetSchoolSearchResultsForAutocompleteAsync(searchText);

        searchResults.Length.Should().Be(2);
        searchResults.Should().Contain([expectedSchoolResult, expectedOtherSchoolResult]);
        await _mockTrustSchoolSearchRepository.DidNotReceive().GetAutoCompleteSearchResultsAsync(Arg.Any<string>());
        await _mockTrustSchoolSearchRepository.DidNotReceive().GetTrustAutoCompleteSearchResultsAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task GetSchoolSearchResultsForAutocompleteAsync_should_return_empty_when_repository_returns_no_results()
    {
        _mockTrustSchoolSearchRepository.GetSchoolAutoCompleteSearchResultsAsync("a").Returns([]);

        var searchResults = await _sut.GetSchoolSearchResultsForAutocompleteAsync("a");

        searchResults.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task GetTrustSearchResultsForAutocompleteAsync_if_keywords_are_null_or_empty_should_return_empty_results(
        string? text)
    {
        var result = await _sut.GetTrustSearchResultsForAutocompleteAsync(text);

        result.Should().BeEmpty();
        await _mockTrustSchoolSearchRepository.Received(0).GetTrustAutoCompleteSearchResultsAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task GetTrustSearchResultsForAutocompleteAsync_should_return_correct_mapped_models()
    {
        var searchText = "a";

        SearchResult trustResult = new("123456", "TR1234", "A Cool Trust", "Multi-academy",
            "A street, Station Road, Town, GH1 8JH", true, "TR123");

        SearchResult otherTrustResult = new("123457", "TR1235", "Another Cool Trust", "Single-academy",
            "Third street, Station Road, Town, GH1 8JH", true, "TR124");

        SearchResult[] results = [trustResult, otherTrustResult];

        var expectedTrustResult = new SearchResultServiceModel(trustResult.Id, trustResult.ReferenceNumber,
            trustResult.Name, "A street, Station Road, Town, GH1 8JH", trustResult.TrustReferenceNumber,
            trustResult.Type, ResultType.Trust);

        var expectedOtherTrustResult = new SearchResultServiceModel(otherTrustResult.Id,
            otherTrustResult.ReferenceNumber, otherTrustResult.Name, "Third street, Station Road, Town, GH1 8JH",
            otherTrustResult.TrustReferenceNumber, otherTrustResult.Type, ResultType.Trust);

        _mockTrustSchoolSearchRepository.GetTrustAutoCompleteSearchResultsAsync(searchText).Returns(results);

        var searchResults = await _sut.GetTrustSearchResultsForAutocompleteAsync(searchText);

        searchResults.Length.Should().Be(2);
        searchResults.Should().Contain([expectedTrustResult, expectedOtherTrustResult]);
        await _mockTrustSchoolSearchRepository.DidNotReceive().GetAutoCompleteSearchResultsAsync(Arg.Any<string>());
        await _mockTrustSchoolSearchRepository.DidNotReceive().GetSchoolAutoCompleteSearchResultsAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task GetTrustSearchResultsForAutocompleteAsync_should_return_empty_when_repository_returns_no_results()
    {
        _mockTrustSchoolSearchRepository.GetTrustAutoCompleteSearchResultsAsync("a").Returns([]);

        var searchResults = await _sut.GetTrustSearchResultsForAutocompleteAsync("a");

        searchResults.Should().BeEmpty();
    }
}
