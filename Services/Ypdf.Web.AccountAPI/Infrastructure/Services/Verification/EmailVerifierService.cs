using System.Net.Mail;

namespace Ypdf.Web.AccoutAPI.Infrastructure.Services.Verification;

public class EmailVerifierService : IEmailVerifierService
{
    public bool IsGood(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            _ = new MailAddress(email);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
