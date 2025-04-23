namespace Ypdf.Web.AccoutAPI.Infrastructure.Services;

public interface IUserNameVerifierService
{
    bool IsGood(string? userName);
}
