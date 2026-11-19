using DfE.FindInformationAcademiesTrusts.Data;
using DfE.FindInformationAcademiesTrusts.HttpServices;
using DfE.FindInformationAcademiesTrusts.Pages.LocalAuthorities;
using GovUK.Dfe.AcademiesApi.Client.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.LocalAuthorities;

public class ResultsModelTests
{
    private readonly IGetLocalAuthorities _mockGetLocalAuthorities = Substitute.For<IGetLocalAuthorities>();
    private readonly ResultsModel _sut;

    public ResultsModelTests()
    {
        _sut = new ResultsModel(_mockGetLocalAuthorities);
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
    public void LaCode_property_is_null_by_default()
    {
        _sut.LaCode.Should().BeNull();
    }

    [Fact]
    public void SearchPageNumber_should_default_to_1()
    {
        _sut.PageNumber.Should().Be(1);
    }

    [Fact]
    public void PageName_should_be_local_authorities_results()
    {
        _sut.PageName.Should().Be("LocalAuthorities/Results");
    }

    [Fact]
    public void PaginationRouteData_should_default_to_empty()
    {
        _sut.PaginationRouteData.Should().BeEmpty();
    }

    [Fact]
    public void PageStatus_should_default_to_empty()
    {
        _sut.PageStatus.Should().BeEquivalentTo(new PageStatus(0, 0, 0));
        _sut.SearchResults.Should().BeEmpty();
    }

    [Fact]
    public void InputId_LabelText_HintText_and_PlaceholderText_should_have_fixed_values()
    {
        _sut.PageSearchFormInputId.Should().Be("la-search");
        _sut.LabelText.Should().Be("Search for a local authority ");
        _sut.HintText.Should().Be("Search by LA code or name. Include any punctuation in names.");
        _sut.PlaceholderText.Should().Be("");
    }

    [Theory]
    [InlineData("Leeds", null)]
    [InlineData("Leeds", "383")]
    [InlineData("", null)]
    [InlineData(null, null)]
    public void OnPost_should_redirect_to_onGetAsync_with_keywords_and_la_code(string? keywords, string? laCode)
    {
        _sut.KeyWords = keywords;
        _sut.LaCode = laCode;

        var result = _sut.OnPost();

        var redirectResult = result.Should().BeOfType<RedirectToPageResult>().Subject;
        redirectResult.PageName.Should().BeNull();
        redirectResult.RouteValues.Should().ContainKey("KeyWords").WhoseValue.Should().Be(keywords);
        redirectResult.RouteValues.Should().ContainKey("LaCode").WhoseValue.Should().Be(laCode);
    }

    [Fact]
    public async Task OnGetAsync_should_redirect_to_details_if_given_la_code()
    {
        _sut.LaCode = "383";

        var result = await _sut.OnGetAsync();

        var redirectResult = result.Should().BeOfType<RedirectToPageResult>().Subject;
        redirectResult.PageName.Should().Be("/LocalAuthorities/Details");
        redirectResult.RouteValues.Should().ContainKey("LaCode").WhoseValue.Should().Be("383");
        await _mockGetLocalAuthorities.DidNotReceiveWithAnyArgs().SearchLocalAuthorities(default, default);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task IfPageNumber_IsLessThanOrEqualTo0_Should404(int pageNumber)
    {
        _sut.PageNumber = pageNumber;
        _sut.KeyWords = "Leeds";

        var result = await _sut.OnGetAsync();

        result.Should().BeOfType<NotFoundResult>();
        await _mockGetLocalAuthorities.DidNotReceiveWithAnyArgs().SearchLocalAuthorities(default, default);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task OnGetAsync_should_return_no_results_page_if_no_keyword_given(string? keywords)
    {
        _sut.KeyWords = keywords;

        var result = await _sut.OnGetAsync();

        result.Should().BeOfType<PageResult>();
        _sut.SearchResults.Should().BeEmpty();
        await _mockGetLocalAuthorities.DidNotReceiveWithAnyArgs().SearchLocalAuthorities(default, default);
    }

    [Fact]
    public async Task OnGetAsync_should_return_no_results_page_when_no_matching_keyword()
    {
        _sut.KeyWords = "no matching authorities";
        _mockGetLocalAuthorities.SearchLocalAuthorities(_sut.KeyWords, null)
            .Returns(new List<NameAndCodeDto>());

        var result = await _sut.OnGetAsync();

        result.Should().BeOfType<PageResult>();
        _sut.SearchResults.Should().BeEmpty();
        _sut.PageStatus.Should().BeEquivalentTo(new PageStatus(1, 0, 0));
        _sut.PaginationRouteData["KeyWords"].Should().Be(_sut.KeyWords);
    }

    [Theory]
    [InlineData("Leeds", "Leeds", null)]
    [InlineData("383", null, "383")]
    public async Task OnGetAsync_should_redirect_to_details_when_one_matching_authority(
        string keywords, string? name, string? code)
    {
        _sut.KeyWords = keywords;
        _mockGetLocalAuthorities.SearchLocalAuthorities(name, code)
            .Returns(new List<NameAndCodeDto> { new() { Code = "383", Name = "Leeds" } });

        var result = await _sut.OnGetAsync();

        var redirectResult = result.Should().BeOfType<RedirectToPageResult>().Subject;
        redirectResult.PageName.Should().Be("/LocalAuthorities/Details");
        redirectResult.RouteValues.Should().ContainKey("LaCode").WhoseValue.Should().Be("383");
        await _mockGetLocalAuthorities.Received(1).SearchLocalAuthorities(name, code);
    }

    [Theory]
    [InlineData(2, 1)]
    [InlineData(10, 1)]
    [InlineData(11, 1)]
    [InlineData(11, 2)]
    [InlineData(20, 2)]
    [InlineData(21, 3)]
    public async Task OnGetAsync_should_return_requested_page_of_results(int totalResults, int pageNumber)
    {
        var matches = Enumerable.Range(1, totalResults)
            .Select(index => new NameAndCodeDto { Code = index.ToString(), Name = $"Local authority {index}" })
            .ToList();
        _mockGetLocalAuthorities.SearchLocalAuthorities("Local authority", null).Returns(matches);
        _sut.KeyWords = "Local authority";
        _sut.PageNumber = pageNumber;

        var result = await _sut.OnGetAsync();

        result.Should().BeOfType<PageResult>();
        _sut.SearchResults.Should().BeEquivalentTo(
            matches.Skip((pageNumber - 1) * 10).Take(10), options => options.WithStrictOrdering());
        _sut.PageStatus.Should().BeEquivalentTo(
            new PageStatus(pageNumber, (int)Math.Ceiling(totalResults / 10.0), totalResults));
        _sut.PaginationRouteData.Should().BeEquivalentTo(
            new Dictionary<string, string> { { "KeyWords", "Local authority" } });
    }

    [Fact]
    public async Task IfPageNumber_IsGreaterThanNumberOfResultsPages_Should404()
    {
        _sut.KeyWords = "Local authority";
        _sut.PageNumber = 2;
        _mockGetLocalAuthorities.SearchLocalAuthorities("Local authority", null)
            .Returns(new List<NameAndCodeDto>
            {
                new() { Code = "383", Name = "Leeds" },
                new() { Code = "380", Name = "Bradford" }
            });

        var result = await _sut.OnGetAsync();

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void Title_Should_Include_The_Keywords_When_They_Are_Set()
    {
        _sut.KeyWords = "Leeds";

        _sut.Title.Should().Be("Search - Leeds");
    }

    [Theory]
    [InlineData(2, 1, "Search - Local authority")]
    [InlineData(11, 2, "Search (page 2 of 2) - Local authority")]
    public async Task Title_Should_Include_Page_Number_When_There_Is_More_Than_One_Page(
        int totalResults, int pageNumber, string expectedTitle)
    {
        _sut.KeyWords = "Local authority";
        _sut.PageNumber = pageNumber;
        _mockGetLocalAuthorities.SearchLocalAuthorities("Local authority", null)
            .Returns(Enumerable.Range(1, totalResults)
                .Select(index => new NameAndCodeDto { Code = index.ToString(), Name = $"Local authority {index}" })
                .ToList());

        await _sut.OnGetAsync();

        _sut.Title.Should().Be(expectedTitle);
    }
}
