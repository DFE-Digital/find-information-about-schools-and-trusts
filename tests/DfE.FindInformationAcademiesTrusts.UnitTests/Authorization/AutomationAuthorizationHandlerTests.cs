using System.Security.Claims;
using DfE.FindInformationAcademiesTrusts.Authorization;
using DfE.FindInformationAcademiesTrusts.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace DfE.FindInformationAcademiesTrusts.UnitTests.Authorization;

public class AutomationAuthorizationHandlerTests
{
    private readonly IWebHostEnvironment _mockWebHostEnvironment;
    private readonly DefaultHttpContext _httpContext;
    private readonly IOptions<TestOverrideOptions> _mockTestOverrideOptions;
    private readonly AutomationAuthorizationHandler _sut;
    private readonly IHttpContextAccessor _mockHttpAccessor;

    public AutomationAuthorizationHandlerTests()
    {
        _mockHttpAccessor = Substitute.For<IHttpContextAccessor>();
        _mockWebHostEnvironment = Substitute.For<IWebHostEnvironment>();
        _mockTestOverrideOptions = Substitute.For<IOptions<TestOverrideOptions>>();
        _httpContext = new DefaultHttpContext();

        _mockHttpAccessor.HttpContext.Returns(_httpContext);
        _mockTestOverrideOptions.Value.Returns(new TestOverrideOptions { CypressTestSecret = "123" });
        _mockWebHostEnvironment.EnvironmentName.Returns("Development");

        _sut = new AutomationAuthorizationHandler(_mockWebHostEnvironment, _mockHttpAccessor,
            _mockTestOverrideOptions);
    }

    [Theory]
    [InlineData("Development", true)]
    [InlineData("LocalDevelopment", true)]
    [InlineData("CI", true)]
    [InlineData("Test", true)]
    [InlineData("Production", false)]
    public void IsClientSecretHeaderValid_should_only_be_true_in_specified_environments(string environment,
        bool expected)
    {
        _mockWebHostEnvironment.EnvironmentName.Returns(environment);
        _httpContext.Request.Headers.Append(HeaderNames.Authorization, "Bearer 123");

        //Create sut here because constructor decides whether or not an environment is live
        var sut = new AutomationAuthorizationHandler(_mockWebHostEnvironment, _mockHttpAccessor,
            _mockTestOverrideOptions);

        var result = sut.IsClientSecretHeaderValid();

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Bearer ")]
    [InlineData("Bearer 456")]
    public void ClientSecretHeaderValid_should_return_false_if_contents_are_wrong(string headerAuthKey)
    {
        _httpContext.Request.Headers.Append(HeaderNames.Authorization, $"Bearer {headerAuthKey}");

        var result = _sut.IsClientSecretHeaderValid();

        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("", null)]
    [InlineData("Bearer ", null)]
    [InlineData("", "")]
    [InlineData("Bearer ", "")]
    public void ClientSecretHeaderValid_should_return_false_if_serverAuthKey_not_set(string headerAuthKey,
        string? serverAuthKey)
    {
        _httpContext.Request.Headers.Append(HeaderNames.Authorization, $"Bearer {headerAuthKey}");
        _mockTestOverrideOptions.Value.Returns(new TestOverrideOptions { CypressTestSecret = serverAuthKey });

        var result = _sut.IsClientSecretHeaderValid();

        result.Should().BeFalse();
    }

    [Fact]
    public void ClientSecretHeaderValid_should_return_false_if_no_auth_header_provided()
    {
        var result = _sut.IsClientSecretHeaderValid();

        result.Should().BeFalse();
    }

    [Fact]
    public void SetupAutomationUser_sets_correct_claims()
    {
        _sut.SetupAutomationUser();
        var expected = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
        {
            new("name", "Automation User - name"),
            new("preferred_username", "FastTestUser@education.gov.uk")
        }));
        var actual = _httpContext.User;
        // Exclude subject due to cyclical references
        actual.Claims.Should().BeEquivalentTo(expected.Claims, options => options.Excluding(claim => claim.Subject));
    }

    [Fact]
    public void IfTestRoleInHeaderIsSet_ShouldAddClaims()
    {
        var testRole = "TestRole";

        _httpContext.Request.Headers.Append("X-Test-Role", testRole);

        _sut.SetupAutomationUser();

        var expected = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
        {
            new("name", "Automation User - name"),
            new("preferred_username", "FastTestUser@education.gov.uk"),
            new("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", "FastTestUser@education.gov.uk"),
            new("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", testRole)
        }));
        var actual = _httpContext.User;
        // Exclude subject due to cyclical references
        actual.Claims.Should().BeEquivalentTo(expected.Claims, options => options.Excluding(claim => claim.Subject));
    }
}
