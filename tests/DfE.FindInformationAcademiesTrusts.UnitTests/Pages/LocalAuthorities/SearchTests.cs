using DfE.FindInformationAcademiesTrusts.HttpServices;
using DfE.FindInformationAcademiesTrusts.Pages.LocalAuthorities;
using GovUK.Dfe.AcademiesApi.Client.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.LocalAuthorities;

public class SearchModelTests
{
    private readonly IGetLocalAuthorities _mockGetLocalAuthorities = Substitute.For<IGetLocalAuthorities>();
    private readonly SearchModel _sut;

    public SearchModelTests()
    {
        _sut = new SearchModel(_mockGetLocalAuthorities);
    }

    [Fact]
    public void KeyWords_property_is_empty_by_default()
    {
        _sut.KeyWords.Should().Be("");
    }

    [Fact]
    public void InputId_LabelText_HintText_and_PlaceholderText_should_have_fixed_values()
    {
        _sut.PageSearchFormInputId.Should().Be("la-search");
        _sut.LabelText.Should().Be("Search for a local authority ");
        _sut.HintText.Should().Be("Search by LA Code or name. Include any punctuation in names.");
        _sut.PlaceholderText.Should().Be("e.g. leeds city council");
    }

    [Fact]
    public void ShowHeaderSearch_should_be_false()
    {
        _sut.ShowHeaderSearch.Should().BeFalse();
    }

    [Fact]
    public void LaCode_property_is_null_by_default()
    {
        _sut.LaCode.Should().BeNull();
    }

    [Fact]
    public void ServiceLinks_should_contain_other_tools_and_products()
    {
        _sut.ServiceLinks.Should().NotBeEmpty();
        _sut.ServiceLinks.Should().OnlyContain(link =>
            !string.IsNullOrWhiteSpace(link.ServiceName) &&
            !string.IsNullOrWhiteSpace(link.ServiceDescription) &&
            Uri.IsWellFormedUriString(link.Url, UriKind.Absolute));
    }

    [Theory]
    [InlineData("Leeds", null)]
    [InlineData("Leeds", "383")]
    [InlineData("", null)]
    [InlineData(null, null)]
    public void OnPost_should_redirect_to_results_with_keywords_and_la_code(string? keywords, string? laCode)
    {
        _sut.KeyWords = keywords;
        _sut.LaCode = laCode;

        var result = _sut.OnPost();

        var redirectResult = result.Should().BeOfType<RedirectToPageResult>().Subject;
        redirectResult.PageName.Should().Be("/LocalAuthorities/Results");
        redirectResult.RouteValues.Should().ContainKey("KeyWords").WhoseValue.Should().Be(keywords);
        redirectResult.RouteValues.Should().ContainKey("LaCode").WhoseValue.Should().Be(laCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task OnGetPopulateAutocompleteAsync_should_return_empty_json_when_no_keyword(string? keywords)
    {
        var result = await _sut.OnGetPopulateAutocompleteAsync(keywords);

        var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
        jsonResult.Value.Should().BeAssignableTo<IEnumerable<object>>().Which.Should().BeEmpty();
        await _mockGetLocalAuthorities.DidNotReceiveWithAnyArgs().SearchLocalAuthorities(default, default);
    }

    [Theory]
    [InlineData("Leeds", "Leeds", null)]
    [InlineData(" Leeds ", "Leeds", null)]
    [InlineData("383", null, "383")]
    [InlineData(" 383 ", null, "383")]
    public async Task OnGetPopulateAutocompleteAsync_should_search_with_name_or_code(
        string keywords, string? expectedName, string? expectedCode)
    {
        _mockGetLocalAuthorities.SearchLocalAuthorities(expectedName, expectedCode)
            .Returns(new List<NameAndCodeDto> { new() { Code = "383", Name = "Leeds" } });

        var result = await _sut.OnGetPopulateAutocompleteAsync(keywords);

        var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
        jsonResult.Value.Should().BeEquivalentTo(new[] { new { code = "383", name = "Leeds" } });
        await _mockGetLocalAuthorities.Received(1).SearchLocalAuthorities(expectedName, expectedCode);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(7)]
    public async Task OnGetPopulateAutocompleteAsync_should_return_at_most_five_results(int numberOfMatches)
    {
        var matches = Enumerable.Range(1, numberOfMatches)
            .Select(index => new NameAndCodeDto { Code = index.ToString(), Name = $"Local authority {index}" })
            .ToList();
        _mockGetLocalAuthorities.SearchLocalAuthorities("Local authority", null).Returns(matches);

        var result = await _sut.OnGetPopulateAutocompleteAsync("Local authority");

        var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
        jsonResult.Value.Should().BeEquivalentTo(
            matches.Take(5).Select(match => new { code = match.Code, name = match.Name }),
            options => options.WithStrictOrdering());
    }
}
