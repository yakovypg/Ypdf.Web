using System.Linq;

namespace Ypdf.Web.AccoutAPI.Infrastructure.Services.Verification;

public class PasswordVerifierService : IPasswordVerifierService
{
    private const int MinPasswordLength = 8;

    public bool IsGood(string? password)
    {
        return !string.IsNullOrWhiteSpace(password)
            && password.Length >= MinPasswordLength
            && password.Any(char.IsLower)
            && password.Any(char.IsUpper)
            && password.Any(char.IsLetter)
            && password.Any(char.IsDigit);
    }
}
