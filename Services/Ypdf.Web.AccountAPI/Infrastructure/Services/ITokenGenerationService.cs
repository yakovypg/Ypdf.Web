namespace Ypdf.Web.AccoutAPI.Infrastructure.Services;

public interface ITokenGenerationService
{
    string Generate(string userEmail);
}
