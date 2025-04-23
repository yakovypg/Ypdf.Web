namespace Ypdf.Web.AccoutAPI.Infrastructure.Services.Verification;

public interface IUserNameVerifierService
{
    bool IsGood(string? userName);
}
