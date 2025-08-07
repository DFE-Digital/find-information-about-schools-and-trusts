using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using DfE.FindInformationAcademiesTrusts.Extensions;
using DfE.FindInformationAcademiesTrusts.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace DfE.FindInformationAcademiesTrusts.Authorization;

public class AutomationAuthorizationHandler(
    IWebHostEnvironment environment,
    IHttpContextAccessor httpContextAccessor,
    IOptions<TestOverrideOptions> testOverrideOptions)
    : AuthorizationHandler<DenyAnonymousAuthorizationRequirement>,
        IAuthorizationRequirement
{
    private readonly string? _cypressTestSecret = testOverrideOptions.Value.CypressTestSecret;
    private readonly bool _isLiveEnvironment = environment.IsLiveEnvironment();

    public bool IsClientSecretHeaderValid()
    {
        if (string.IsNullOrWhiteSpace(_cypressTestSecret) || _isLiveEnvironment)
            return false;

        var requestHeader = httpContextAccessor.HttpContext?.Request.Headers[HeaderNames.Authorization];
        if (string.IsNullOrWhiteSpace(requestHeader))
            return false;

        var authHeader = requestHeader.Value.ToString().Replace("Bearer ", string.Empty);

        return authHeader == _cypressTestSecret;
    }

    public void SetupAutomationUser()
    {
        var identity = new ClaimsIdentity(new List<Claim>
        {
            new("name", "Automation User - name"),
            new("preferred_username", "Automation User - email"),
        });

        var testRole = httpContextAccessor.HttpContext?.Request.Headers["X-test-role"];
        if (!string.IsNullOrWhiteSpace(testRole))
        {
            identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
                "FastTestUser@education.gov.uk"));
            identity.AddClaim(new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                testRole.Value.ToString()));
        }
        
        var user = new ClaimsPrincipal(identity);
        

        httpContextAccessor.HttpContext!.User = user;
    }

    [ExcludeFromCodeCoverage] // This method is difficult to test, everything that can be tested has been extracted to IsClientSecretHeaderValid
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        DenyAnonymousAuthorizationRequirement requirement)
    {
        if (IsClientSecretHeaderValid())
        {
            SetupAutomationUser();
            context.Succeed(requirement);
        }
        

        return Task.CompletedTask;
    }
}
