namespace Ypdf.Web.AccoutAPI.Infrastructure.Services;

public class UserNameVerifierService : IUserNameVerifierService
{
    public bool IsGood(string? userName)
    {
        return !string.IsNullOrWhiteSpace(userName);
    }
}
