namespace Ypdf.Web.AccoutAPI.Infrastructure.Services.Verification;

public class UserNameVerifierService : IUserNameVerifierService
{
    public bool IsGood(string? userName)
    {
        return !string.IsNullOrWhiteSpace(userName);
    }
}
