using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AccountService.Tests.IntegrationTests;

public class AuthTestHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
	public const string MyScheme   = "TestScheme";
	public const string HUserId    = "X-Test-UserId";

	public AuthTestHandler(
		IOptionsMonitor<AuthenticationSchemeOptions> options,
		ILoggerFactory logger,
		UrlEncoder encoder) : base(options, logger, encoder) { }

	protected override Task<AuthenticateResult> HandleAuthenticateAsync()
	{
		var userId = Request.Headers.TryGetValue(HUserId, out var hvId) && !string.IsNullOrWhiteSpace(hvId)
			? hvId.ToString()
			: Guid.NewGuid().ToString();

		var identity = new ClaimsIdentity(MyScheme);
		identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId));

		var principal = new ClaimsPrincipal(identity);
		var ticket = new AuthenticationTicket(principal, MyScheme);

		return Task.FromResult(AuthenticateResult.Success(ticket));
	}
}