using System.Threading.Tasks;
using Ypdf.Web.AccoutAPI.Models;

namespace Ypdf.Web.AccoutAPI.Infrastructure.Services.Authentication;

public interface ITokenGenerationService
{
    Task<string> GenerateAsync(User user);
}
