using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Ypdf.Web.AccoutAPI.Models;
using Ypdf.Web.Domain.Models.Configuration;

namespace Ypdf.Web.AccoutAPI.Infrastructure.Services.Authentication;

public class TokenGenerationService : ITokenGenerationService
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TokenGenerationService> _logger;

    public TokenGenerationService(
        UserManager<User> userManager,
        IConfiguration configuration,
        ILogger<TokenGenerationService> logger)
    {
        ArgumentNullException.ThrowIfNull(userManager, nameof(userManager));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));

        _userManager = userManager;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string> GenerateAsync(User user)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));

        _logger.LogInformation("Trying to generate token for user with email {Email}", user.Email);

        string issuer = ExtractIssuerFromConfiguration();
        string audience = ExtractAudienceFromConfiguration();

        double expireMinutes = ExtractExpireMinutesFromConfiguration();
        DateTime expireDate = DateTime.Now.AddMinutes(expireMinutes);

        string key = ExtractKeyFromConfiguration();
        SigningCredentials signingCredentials = CreateSigningCredentials(key);

        IEnumerable<Claim> claims = await CreateClaimsAsync(user)
            .ConfigureAwait(false);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expireDate,
            signingCredentials: signingCredentials);

        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        string token = jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);

        _logger.LogInformation("Token for user with email {Email} generated", user.Email);

        return token;
    }

    private static SigningCredentials CreateSigningCredentials(string key)
    {
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));

        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        var signingKey = new SymmetricSecurityKey(keyBytes);

        return new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
    }

    private static void AddSubscriptionClaim(User user, List<Claim> claims)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));
        ArgumentNullException.ThrowIfNull(claims, nameof(claims));

        if (user.UserSubscription is null)
            return;

        string subscription = user.UserSubscription.Subscription.SubscriptionType.ToString();
        var subscriptionClaim = new Claim(JwtCustomClaimNames.Subscription, subscription);

        claims.Add(subscriptionClaim);
    }

    private async Task AddRoleClaimsAsync(User user, List<Claim> claims)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));
        ArgumentNullException.ThrowIfNull(claims, nameof(claims));

        IList<string> roles = await _userManager
            .GetRolesAsync(user)
            .ConfigureAwait(false);

        foreach (string role in roles)
        {
            var claim = new Claim(ClaimTypes.Role, role);
            claims.Add(claim);
        }
    }

    private async Task<List<Claim>> CreateClaimsAsync(User user)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));

        string id = user.Id.ToString(CultureInfo.InvariantCulture);
        string email = user.Email ?? string.Empty;
        string nickname = user.Nickname ?? string.Empty;
        string jti = Guid.NewGuid().ToString();

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, id),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.Name, nickname),
            new(JwtRegisteredClaimNames.Jti, jti)
        ];

        AddSubscriptionClaim(user, claims);

        await AddRoleClaimsAsync(user, claims)
            .ConfigureAwait(false);

        _logger.LogInformation(
            "Token will contain the following claims: {Claims}",
            claims.Select(t => (t.Type, t.Value)));

        return claims;
    }

    private string ExtractIssuerFromConfiguration()
    {
        return _configuration["Jwt:Issuer"]
            ?? throw new ConfigurationException("Issuer for Jwt not specified");
    }

    private string ExtractAudienceFromConfiguration()
    {
        return _configuration["Jwt:Audience"]
            ?? throw new ConfigurationException("Audience for Jwt not specified");
    }

    private string ExtractKeyFromConfiguration()
    {
        return _configuration["Jwt:Key"]
            ?? throw new ConfigurationException("Key for Jwt not specified");
    }

    private double ExtractExpireMinutesFromConfiguration()
    {
        string expireMinutesSource = _configuration["Jwt:ExpireMinutes"]
            ?? throw new ConfigurationException("Expire minutes for Jwt not specified");

        if (!double.TryParse(expireMinutesSource, out double expireMinutes))
            throw new ConfigurationException("Expire minutes for Jwt not recognized");

        return expireMinutes;
    }
}
