namespace Ypdf.Web.AccoutAPI.Infrastructure.Services.Verification;

public class NicknameVerifierService : INicknameVerifierService
{
    public bool IsGood(string? nickname)
    {
        return !string.IsNullOrWhiteSpace(nickname);
    }
}
