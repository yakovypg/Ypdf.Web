using System.Threading.Tasks;
using Ypdf.Web.AccoutAPI.Models;

namespace Ypdf.Web.AccoutAPI.Infrastructure.Services;

public interface ISignInService
{
    Task<User> SignInAsync(string email, string password);
}
