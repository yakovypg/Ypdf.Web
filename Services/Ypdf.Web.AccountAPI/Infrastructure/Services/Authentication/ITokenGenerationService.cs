namespace Ypdf.Web.AccoutAPI.Infrastructure.Services.Authentication;

public interface ITokenGenerationService
{
    string Generate(string userEmail);
}
