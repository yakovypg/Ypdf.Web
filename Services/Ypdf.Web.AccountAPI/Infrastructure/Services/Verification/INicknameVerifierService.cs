namespace Ypdf.Web.AccoutAPI.Infrastructure.Services.Verification;

public interface INicknameVerifierService
{
    bool IsGood(string? nickname);
}
