using Ypdf.Web.AccoutAPI.Models;

namespace Ypdf.Web.AccoutAPI.Infrastructure.Services.Authentication;

public interface ITokenGenerationService
{
    string Generate(User user);
}
