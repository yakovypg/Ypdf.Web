namespace Ypdf.Web.AccoutAPI.Infrastructure.Services;

public interface IPasswordVerifierService
{
    bool IsGood(string? password);
}
