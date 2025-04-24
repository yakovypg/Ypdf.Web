using System.Threading.Tasks;
using Ypdf.Web.AccoutAPI.Models;

namespace Ypdf.Web.AccoutAPI.Data.Repositories;

public interface IUserRepository
{
    Task<User> AddAsync(string email, string password, string userName);
    Task DeleteByEmailAsync(string email);

    Task<User?> GetByEmailWithDependenciesAsync(string email);
    Task<User?> GetByEmailAsync(string email);
    Task<bool> ExistsAsync(string email);
}
