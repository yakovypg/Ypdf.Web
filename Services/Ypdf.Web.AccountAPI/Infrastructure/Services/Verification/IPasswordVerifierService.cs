namespace Ypdf.Web.AccoutAPI.Infrastructure.Services.Verification;

public interface IPasswordVerifierService
{
    bool IsGood(string? password);
}
