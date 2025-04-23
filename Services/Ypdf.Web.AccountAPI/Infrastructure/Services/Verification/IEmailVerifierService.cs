namespace Ypdf.Web.AccoutAPI.Infrastructure.Services.Verification;

public interface IEmailVerifierService
{
    bool IsGood(string? email);
}
