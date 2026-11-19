using DfE.FindInformationAcademiesTrusts.HttpServices;
using DfE.FindInformationAcademiesTrusts.Pages.LocalAuthorities;
using GovUK.Dfe.AcademiesApi.Client.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Pages.LocalAuthorities;

public class DetailsModelTests
{
    private readonly IGetLocalAuthorities _mockGetLocalAuthorities = Substitute.For<IGetLocalAuthorities>();
    private readonly DetailsModel _sut;

    public DetailsModelTests()
    {
        _sut = new DetailsModel(_mockGetLocalAuthorities);
    }

    [Fact]
    public void LaCode_property_is_null_by_default()
    {
        _sut.LaCode.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task OnGetAsync_should_return_not_found_if_no_la_code_given(string? laCode)
    {
        _sut.LaCode = laCode;

        var result = await _sut.OnGetAsync();

        result.Should().BeOfType<NotFoundResult>();
        await _mockGetLocalAuthorities.DidNotReceiveWithAnyArgs().SearchLocalAuthorities(default, default);
    }

    [Fact]
    public async Task OnGetAsync_should_return_not_found_when_no_matching_authority()
    {
        _sut.LaCode = "383";
        _mockGetLocalAuthorities.SearchLocalAuthorities(null, "383")
            .Returns(new List<NameAndCodeDto>());

        var result = await _sut.OnGetAsync();

        result.Should().BeOfType<NotFoundResult>();
        await _mockGetLocalAuthorities.Received(1).SearchLocalAuthorities(null, "383");
    }

    [Fact]
    public async Task OnGetAsync_should_return_page_with_matching_authority()
    {
        var expectedAuthority = new NameAndCodeDto { Code = "383", Name = "Leeds" };
        _sut.LaCode = "383";
        _mockGetLocalAuthorities.SearchLocalAuthorities(null, "383")
            .Returns(new List<NameAndCodeDto> { expectedAuthority });

        var result = await _sut.OnGetAsync();

        result.Should().BeOfType<PageResult>();
        _sut.LocalAuthority.Should().BeSameAs(expectedAuthority);
        await _mockGetLocalAuthorities.Received(1).SearchLocalAuthorities(null, "383");
    }

    [Fact]
    public async Task OnGetAsync_should_prefer_exact_code_match_over_first_result()
    {
        var expectedAuthority = new NameAndCodeDto { Code = "383", Name = "Leeds" };
        _sut.LaCode = "383";
        _mockGetLocalAuthorities.SearchLocalAuthorities(null, "383")
            .Returns(new List<NameAndCodeDto>
            {
                new() { Code = "380", Name = "Bradford" },
                expectedAuthority
            });

        var result = await _sut.OnGetAsync();

        result.Should().BeOfType<PageResult>();
        _sut.LocalAuthority.Should().BeSameAs(expectedAuthority);
    }

    [Fact]
    public async Task OnGetAsync_should_return_first_result_when_no_exact_code_match()
    {
        var firstAuthority = new NameAndCodeDto { Code = "380", Name = "Bradford" };
        _sut.LaCode = "38";
        _mockGetLocalAuthorities.SearchLocalAuthorities(null, "38")
            .Returns(new List<NameAndCodeDto>
            {
                firstAuthority,
                new() { Code = "383", Name = "Leeds" }
            });

        var result = await _sut.OnGetAsync();

        result.Should().BeOfType<PageResult>();
        _sut.LocalAuthority.Should().BeSameAs(firstAuthority);
    }
}
