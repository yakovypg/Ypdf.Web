namespace Ypdf.Web.AccoutAPI.Infrastructure.Services;

public interface IEmailVerifierService
{
    bool IsGood(string? email);
}
