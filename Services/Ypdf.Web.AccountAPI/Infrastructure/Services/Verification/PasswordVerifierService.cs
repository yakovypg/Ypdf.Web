using System;
using System.Linq;
using Ypdf.Web.AccoutAPI.Infrastructure.Configuration;

namespace Ypdf.Web.AccoutAPI.Infrastructure.Services.Verification;

public class PasswordVerifierService : IPasswordVerifierService
{
    private readonly PasswordRequirements _passwordRequirements;

    public PasswordVerifierService(PasswordRequirements passwordRequirements)
    {
        ArgumentNullException.ThrowIfNull(passwordRequirements, nameof(passwordRequirements));
        _passwordRequirements = passwordRequirements;
    }

    public bool IsGood(string? password)
    {
        return !string.IsNullOrEmpty(password)
            && ValidateLength(password)
            && ValidateUniqueChars(password)
            && ValidateDigitRequirement(password)
            && ValidateLetterRequirement(password)
            && ValidateNonAlphanumericRequirement(password)
            && ValidateLowercaseRequirement(password)
            && ValidateUppercaseRequirement(password);
    }

    private bool ValidateLength(string password)
    {
        ArgumentNullException.ThrowIfNull(password, nameof(password));
        return password.Length >= _passwordRequirements.MinimumLength;
    }

    private bool ValidateUniqueChars(string password)
    {
        ArgumentNullException.ThrowIfNull(password, nameof(password));

        int uniqueChars = password
            .Distinct()
            .Count();

        return uniqueChars >= _passwordRequirements.RequiredUniqueChars;
    }

    private bool ValidateDigitRequirement(string password)
    {
        ArgumentNullException.ThrowIfNull(password, nameof(password));

        return !_passwordRequirements.RequireDigit
            || password.Any(char.IsDigit);
    }

    private bool ValidateLetterRequirement(string password)
    {
        ArgumentNullException.ThrowIfNull(password, nameof(password));

        return !_passwordRequirements.RequireLetter
            || password.Any(char.IsLetter);
    }

    private bool ValidateNonAlphanumericRequirement(string password)
    {
        ArgumentNullException.ThrowIfNull(password, nameof(password));

        return !_passwordRequirements.RequireNonAlphanumeric
            || password.Any(t => !char.IsLetterOrDigit(t));
    }

    private bool ValidateLowercaseRequirement(string password)
    {
        ArgumentNullException.ThrowIfNull(password, nameof(password));

        return !_passwordRequirements.RequireLowercase
            || password.Any(char.IsLower);
    }

    private bool ValidateUppercaseRequirement(string password)
    {
        ArgumentNullException.ThrowIfNull(password, nameof(password));

        return !_passwordRequirements.RequireUppercase
            || password.Any(char.IsUpper);
    }
}
