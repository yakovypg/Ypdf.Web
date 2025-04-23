using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Ypdf.Web.Domain.Models.Configuration;

namespace Ypdf.Web.AccoutAPI.Infrastructure.Services;

public class TokenGenerationService : ITokenGenerationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<TokenGenerationService> _logger;

    public TokenGenerationService(
        IConfiguration configuration,
        ILogger<TokenGenerationService> logger)
    {
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        _configuration = configuration;
        _logger = logger;
    }

    public string Generate(string userEmail)
    {
        ArgumentException.ThrowIfNullOrEmpty(userEmail, nameof(userEmail));

        _logger.LogInformation("Trying to generate token for user with email {Email}", userEmail);

        string jti = Guid.NewGuid().ToString();

        var claims = new Claim[]
        {
            new(JwtRegisteredClaimNames.Sub, userEmail),
            new(JwtRegisteredClaimNames.Jti, jti)
        };

        string issuer = ExtractIssuerFromConfiguration();
        string audience = ExtractAudienceFromConfiguration();

        double expireMinutes = ExtractExpireMinutesFromConfiguration();
        DateTime expireDate = DateTime.Now.AddMinutes(expireMinutes);

        string key = ExtractKeyFromConfiguration();
        SigningCredentials signingCredentials = CreateSigningCredentials(key);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expireDate,
            signingCredentials: signingCredentials);

        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        string token = jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);

        _logger.LogInformation("Token for user with email {Email} generated", userEmail);

        return token;
    }

    private static SigningCredentials CreateSigningCredentials(string key)
    {
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));

        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        var signingKey = new SymmetricSecurityKey(keyBytes);

        return new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
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
