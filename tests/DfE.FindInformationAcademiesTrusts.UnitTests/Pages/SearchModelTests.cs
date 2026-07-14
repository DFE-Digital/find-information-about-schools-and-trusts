using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Search;
using DfE.FindInformationAcademiesTrusts.Services.Trust;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NSubstitute.ReturnsExtensions;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages;

public class SearchModelTests
{
    private const string SearchTermThatMatchesAllFakeTrusts = "trust";
    private readonly SearchModel _sut;
    private readonly ISearchService _mockSearchService;

    private static readonly TrustSummaryServiceModel _fakeTrust = new("1234", "12122","My Trust", "Multi-academy trust", 3);
    private static readonly SchoolSummaryServiceModel _fakeSchool = new(5678,"A school", "Community school", SchoolCategory.LaMaintainedSchool);

    private ITrustService _mockTrustService = Substitute.For<ITrustService>();
    private ISchoolService _mockSchoolService = Substitute.For<ISchoolService>();

    private readonly SearchResultServiceModel[] _fakeResults =
    [
        new("111", "12212","trust 1", "Dorthy Inlet, Kingston upon Hull, City of, JY36 9VC", "1033", "Multi-academy trust", ResultType.Trust),
        new("222", "12221","school 1", "12 Halifax road, Whitby, WH96 9WV", null, "Community school", ResultType.School),
        new("333", "122121","trust 2", "Grant Course, North East Lincolnshire, QH96 9WV", "1022", "Single-academy trust", ResultType.Trust),
        new("444", "122112","trust 3", "Abbott Turnpike, East Riding of Yorkshire, BI86 4LZ", "1011", "Multi-academy trust", ResultType.Trust)
    ];

    public SearchModelTests()
    {
        _mockSearchService = Substitute.For<ISearchService>();
        
        _mockTrustService.GetTrustSummaryAsync(_fakeTrust.Uid).Returns(_fakeTrust);
        _mockSchoolService.GetSchoolSummaryAsync(_fakeSchool.Urn).Returns(_fakeSchool);

        _mockSearchService.GetSearchResultsForPageAsync(Arg.Any<string?>(), Arg.Any<int>()).Returns(new PagedSearchResults(PaginatedList<SearchResultServiceModel>.Empty(), new SearchResultsOverview()));
        _mockSearchService.GetSearchResultsForPageAsync(SearchTermThatMatchesAllFakeTrusts, Arg.Any<int>()).Returns(new PagedSearchResults(new PaginatedList<SearchResultServiceModel>(_fakeResults, _fakeResults.Length, 1, 1), new SearchResultsOverview(3, 1)));
        _mockSearchService.GetSearchResultsForAutocompleteAsync(Arg.Any<string>()).Returns([]);
        _mockSearchService.GetSearchResultsForAutocompleteAsync(SearchTermThatMatchesAllFakeTrusts).Returns(_fakeResults);

        _sut = new SearchModel(_mockSearchService, _mockTrustService, _mockSchoolService);
    }

    [Fact]
    public async Task OnGetAsync_should_search_with_query_parameter_provided()
    {
        _sut.KeyWords = SearchTermThatMatchesAllFakeTrusts;

        await _sut.OnGetAsync();

        _sut.SearchResults.Should().BeEquivalentTo(_fakeResults);
    }

    [Fact]
    public async Task OnGetAsync_set_keywords()
    {
        _sut.KeyWords = "no matching trusts";
        await _sut.OnGetAsync();

        _sut.SearchResults.Should().BeEmpty();
        _sut.PaginationRouteData["Keywords"].Should().Be("no matching trusts");
    }

    [Fact]
    public async Task OnGetAsync_should_return_no_results_page_if_no_keyword_given()
    {
        _sut.KeyWords = null;
        await _sut.OnGetAsync();

        _sut.SearchResults.Should().BeEmpty();
        _sut.PaginationRouteData["Keywords"].Should().Be(string.Empty);
    }
    
    [Theory]
    [InlineData(ResultType.Trust)]
    [InlineData(ResultType.School)]
    public async Task OnGetAsync_should_redirect_to_overview_if_given_identifier_and_query_is_trust_name(ResultType resultType)
    {
        _sut.SearchResultType = resultType;

        if (resultType == ResultType.Trust)
        {
            _sut.Id = _fakeTrust.Uid;
            _sut.Ukprn = _fakeTrust.Ukprn;
            _sut.KeyWords = _fakeTrust.Name;
        }
        else
        {
            _sut.Id = _fakeSchool.Urn.ToString();
            _sut.KeyWords = _fakeSchool.Name;
        }

        var result = await _sut.OnGetAsync();

        var redirectResult = result.Should().BeOfType<RedirectToPageResult>().Subject;

        if (resultType == ResultType.Trust)
        {
            redirectResult.PageName.Should().Be("/trusts/overview/trustdetails");
            redirectResult.RouteValues.Should().ContainKey("Uid").WhoseValue.Should().Be(_fakeTrust.Uid);
        }
        else
        {
            redirectResult.PageName.Should().Be("/schools/overview/details");
            redirectResult.RouteValues.Should().ContainKey("urn").WhoseValue.Should().Be(_fakeSchool.Urn);
        }
    }

    [Fact]
    public async Task OnGetAsync_should_not_redirect_to_trust_overview_if_trustId_does_not_match_query()
    {
        var differentFakeTrust = new SearchResultServiceModel("123", "121","other trust", "Some address", "TR0987", "Single-academy trust", ResultType.Trust);
        _mockSearchService.GetSearchResultsForPageAsync(differentFakeTrust.Name, Arg.Any<int>())
            .Returns(new PagedSearchResults(new PaginatedList<SearchResultServiceModel>([differentFakeTrust], 1, 1, 1), new SearchResultsOverview(1)));

        _sut.KeyWords = differentFakeTrust.Name;
        _sut.Id = _fakeTrust.Uid;

        var result = await _sut.OnGetAsync();

        result.Should().BeOfType<PageResult>();
        _sut.SearchResults.Should().ContainSingle(t => t == differentFakeTrust);
    }

    [Fact]
    public async Task OnGetPopulateAutocompleteAsync_should_return_results_matching_keyword()
    {
        _sut.KeyWords = SearchTermThatMatchesAllFakeTrusts;

        var result = await _sut.OnGetPopulateAutocompleteAsync();

        var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
        jsonResult.Value.Should().BeEquivalentTo(_fakeResults.Select(r =>
            new SearchModel.AutocompleteEntry(
                r.Id,
                r.ukprn,
                r.Address,
                r.Name,
                r.ResultType
            )
        ));
    }

    [Theory]
    [InlineData("no matching trusts")]
    [InlineData("")]
    [InlineData(null)]
    public async Task OnGetPopulateAutocompleteAsync_should_return_empty_json_when_no_matching_keyword(string? keywords)
    {
        _sut.KeyWords = keywords;

        var result = await _sut.OnGetPopulateAutocompleteAsync();

        using var _ = new AssertionScope();

        var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
        jsonResult.Value.Should()
            .BeAssignableTo<IEnumerable<SearchModel.AutocompleteEntry>>()
            .Which.Should().BeEmpty();
    }

    [Fact]
    public void OnPost_should_always_redirect_to_onGetAsync()
    {
        var result = (RedirectToPageResult)_sut.OnPost();

        result.PageName.Should().BeEquivalentTo("/Search");
        result.RouteValues?["KeyWords"].Should().BeEquivalentTo(string.Empty);
        result.RouteValues?["Id"].Should().BeEquivalentTo(string.Empty);
        result.RouteValues?["SearchResultType"].Should().BeNull();
    }

    [Fact]
    public void ShowHeaderSearch_should_be_false()
    {
        _sut.ShowHeaderSearch.Should().BeFalse();
    }

    [Fact]
    public void KeyWords_property_is_empty_by_default()
    {
        _sut.KeyWords.Should().Be("");
    }

    [Fact]
    public void Identifier_property_is_empty_by_default()
    {
        _sut.Id.Should().Be("");
    }

    [Fact]
    public void SearchResultType_property_is_null_by_default()
    {
        _sut.SearchResultType.Should().BeNull();
    }

    [Fact]
    public void InputId_should_have_a_fixed_value()
    {
        _sut.PageSearchFormInputId.Should().Be("search");
    }

    [Fact]
    public void SearchPageNumber_should_default_to_1()
    {
        _sut.PageNumber.Should().Be(1);
    }

    [Fact]
    public void PageName_should_be_search()
    {
        _sut.PageName.Should().Be("Search");
    }

    [Fact]
    public void PaginationRouteData_should_default_to_empty()
    {
        _sut.PaginationRouteData.Should().BeEquivalentTo(new Dictionary<string, string>());
    }

    [Fact]
    public void PageStatus_should_default_to_empty()
    {
        _sut.PageStatus.Should().BeEquivalentTo(new PageStatus(0, 0, 0));
    }

    [Fact]
    public async Task When_a_different_page_is_requested_return_a_different_page()
    {
        _mockSearchService.GetSearchResultsForPageAsync(SearchTermThatMatchesAllFakeTrusts, 1).Returns(
                new PagedSearchResults(new PaginatedList<SearchResultServiceModel>(_fakeResults, 4, 1, 3), new SearchResultsOverview(3, 1)));
        
        var differentFakeResult = new SearchResultServiceModel("111", "21312213",SearchTermThatMatchesAllFakeTrusts, "Some address", "TR0987", "Single-academy trust", ResultType.Trust);
        _mockSearchService.GetSearchResultsForPageAsync(differentFakeResult.Name, 2).Returns(new PagedSearchResults(new PaginatedList<SearchResultServiceModel>([differentFakeResult], 4, 2, 3), new SearchResultsOverview(1)));

        _sut.KeyWords = SearchTermThatMatchesAllFakeTrusts;
        _sut.PageNumber = 1;

        var result = await _sut.OnGetAsync();
        result.Should().BeOfType<PageResult>();
        _sut.SearchResults.Should().BeEquivalentTo(_fakeResults);
        _sut.PaginationRouteData["Keywords"].Should().Be(SearchTermThatMatchesAllFakeTrusts);


        _sut.PageNumber = 2;
        result = await _sut.OnGetAsync();
        result.Should().BeOfType<PageResult>();
        _sut.SearchResults.Should().ContainSingle(t => t == differentFakeResult);
        _sut.PaginationRouteData["Keywords"].Should().Be(SearchTermThatMatchesAllFakeTrusts);
    }

    [Fact]
    public void Title_Should_Be_Search_When_Keywords_Are_Empty()
    {
        _sut.Title.Should().BeEquivalentTo("Search");
    }

    [Fact]
    public void Title_Should_Include_The_Keywords_When_they_Are_Set()
    {
        _sut.KeyWords = "Test";
        _sut.Title.Should().BeEquivalentTo("Search - Test");
    }

    [Fact]
    public void Title_Should_Include_The_Keywords_When_they_Are_Set_And_There_Is_Only_one_page()
    {
        _sut.KeyWords = "Test";
        _sut.SearchResults = new PaginatedList<SearchResultServiceModel>(Array.Empty<SearchResultServiceModel>(), 1, 1, 1);
        _sut.Title.Should().BeEquivalentTo("Search - Test");
    }

    [Fact]
    public void Title_Should_Include_The_Keywords_And_Page_Number_When_They_Are_Set()
    {
        _sut.KeyWords = "Test";
        _sut.SearchResults = new PaginatedList<SearchResultServiceModel>(Array.Empty<SearchResultServiceModel>(), 46, 2, 20);
        _sut.Title.Should().BeEquivalentTo("Search (page 2 of 3) - Test");
    }

    [Fact]
    public async Task IfSchoolSelected_ButSchoolSummaryIsNull_ShouldRedirectToSearchPage()
    {
        _sut.KeyWords = "a school";
        _sut.SearchResultType = ResultType.School;
        _sut.Id = "123";

        _mockSchoolService.GetSchoolSummaryAsync(int.Parse(_sut.Id)).ReturnsNull();

        var result = await _sut.OnGetAsync();
        result.Should().BeOfType<PageResult>();

        await _mockSearchService.Received(1).GetSearchResultsForPageAsync(_sut.KeyWords, Arg.Any<int>());
    }

    [Fact]
    public async Task IfSchoolSelected_ButCannotParseId_for_URN_ShouldRedirectToSearchPage()
    {
        _sut.KeyWords = "a school";
        _sut.SearchResultType = ResultType.School;
        _sut.Id = "bad id";

        var result = await _sut.OnGetAsync();
        result.Should().BeOfType<PageResult>();

        await _mockSchoolService.DidNotReceiveWithAnyArgs().GetSchoolSummaryAsync(Arg.Any<int>());
        await _mockSearchService.Received(1).GetSearchResultsForPageAsync(_sut.KeyWords, 1);
    }

    [Fact]
    public async Task IfTrustSelected_ButTrustSummaryIsNull_ShouldRedirectToSearchPage()
    {
        _sut.KeyWords = "a trust";
        _sut.SearchResultType = ResultType.Trust;
        _sut.Id = "trustid";

        _mockTrustService.GetTrustSummaryAsync(_sut.Id).ReturnsNull();

        var result = await _sut.OnGetAsync();
        result.Should().BeOfType<PageResult>();

        await _mockSearchService.Received(1).GetSearchResultsForPageAsync(_sut.KeyWords, 1);
    }

    [Fact]
    public async Task IfPageNumber_IsGreaterThanNumberOfResultsPages_Should404()
    {
        _sut.PageNumber = 99;
        _sut.KeyWords = SearchTermThatMatchesAllFakeTrusts;

        var result = await _sut.OnGetAsync();

        result.Should().BeOfType<NotFoundResult>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task IfPageNumber_IsLessThanOrEqualTo0_Should404(int pageNumber)
    {
        _sut.PageNumber = pageNumber;
        _sut.KeyWords = SearchTermThatMatchesAllFakeTrusts;

        var result = await _sut.OnGetAsync();

        result.Should().BeOfType<NotFoundResult>();
    }
}
