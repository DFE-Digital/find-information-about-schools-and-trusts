using DfE.FindInformationAcademiesTrusts.HttpServices;
using DfE.FindInformationAcademiesTrusts.Pages.LocalAuthorities;
using GovUK.Dfe.AcademiesApi.Client.Contracts;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.LocalAuthorities;

public class LocalAuthoritySearchTests
{
    private readonly IGetLocalAuthorities _mockGetLocalAuthorities = Substitute.For<IGetLocalAuthorities>();

    [Theory]
    [InlineData("Leeds", "Leeds")]
    [InlineData(" Leeds ", "Leeds")]
    [InlineData("\tLeeds\r\n", "Leeds")]
    [InlineData("Kingston upon Hull, City of", "Kingston upon Hull, City of")]
    [InlineData("Leeds383", "Leeds383")]
    public async Task Find_should_search_by_name_when_keyword_is_not_all_digits(string keywords, string expectedName)
    {
        await _mockGetLocalAuthorities.Find(keywords);

        await _mockGetLocalAuthorities.Received(1).SearchLocalAuthorities(expectedName, null);
    }

    [Theory]
    [InlineData("383", "383")]
    [InlineData(" 383 ", "383")]
    [InlineData("\t383\r\n", "383")]
    [InlineData("001", "001")]
    public async Task Find_should_search_by_code_when_keyword_is_all_digits(string keywords, string expectedCode)
    {
        await _mockGetLocalAuthorities.Find(keywords);

        await _mockGetLocalAuthorities.Received(1).SearchLocalAuthorities(null, expectedCode);
    }

    [Fact]
    public async Task Find_should_return_results_from_local_authority_search()
    {
        var expectedResults = new List<NameAndCodeDto>
        {
            new() { Code = "383", Name = "Leeds" }
        };
        _mockGetLocalAuthorities.SearchLocalAuthorities("Leeds", null).Returns(expectedResults);

        var result = await _mockGetLocalAuthorities.Find("Leeds");

        result.Should().BeSameAs(expectedResults);
    }

    [Fact]
    public async Task Find_should_return_empty_results_when_no_matching_keyword()
    {
        _mockGetLocalAuthorities.SearchLocalAuthorities("no matching authorities", null)
            .Returns(new List<NameAndCodeDto>());

        var result = await _mockGetLocalAuthorities.Find("no matching authorities");

        result.Should().BeEmpty();
    }
}
