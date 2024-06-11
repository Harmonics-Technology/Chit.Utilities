using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using Chit.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;


namespace Chit.Utilities;

// this service uses restsharp to call the identity server to validate the token and get the user details and roles and add them to the context 
// then add them to the claims principal 

public class CustomAuthenticationHandler : AuthenticationHandler<CustomAuthenticationOptions>
{

    private readonly IConfiguration _configuration;

    public CustomAuthenticationHandler(IOptionsMonitor<CustomAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IConfiguration configuration)
        : base(options, logger, encoder, clock)
    {
        _configuration = configuration;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            return AuthenticateResult.Fail("Unauthorized");

        var client = new RestClient(_configuration["IdentityServerUrl"]);
        var request = new RestRequest("api/Identity/info", Method.Get);
        request.AddHeader("Authorization", $"Bearer {token}");
        var response = await client.ExecuteAsync<ChitStandardResponse<TokenValidationResponse>>(request);
        if (response.StatusCode != HttpStatusCode.OK)
            return AuthenticateResult.Fail("Unauthorized");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, response.Data.Data.Id.ToString()),
            new Claim(ClaimTypes.Name, response.Data.Data.FullName),
            new Claim(ClaimTypes.Email, response.Data.Data.Email),
            new Claim(ClaimTypes.MobilePhone, response.Data.Data.PhoneNumber ?? ""),
            new Claim(ClaimTypes.GivenName, response.Data.Data.FirstName),
            new Claim(ClaimTypes.Surname, response.Data.Data.LastName)   
        };

        foreach (var role in response.Data.Data.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = 401;
        var problemDetails = new ProblemDetails() { Status = 401, Detail = "Unauthorized" };

        await Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
    }

    protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = 403;
        var problemDetails = new ProblemDetails() { Status = 401, Detail = "Unauthorized" };

        await Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
    }
}


public class CustomAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "CustomAuthentication";
    public string Scheme => DefaultScheme;
    public string AuthenticationType = DefaultScheme;
}

public class TokenValidationResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public IList<string> Roles { get; set; }
}
