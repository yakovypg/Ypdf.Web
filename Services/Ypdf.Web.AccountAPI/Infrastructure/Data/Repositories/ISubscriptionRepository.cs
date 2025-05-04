using System.Threading.Tasks;
using Ypdf.Web.AccoutAPI.Models;
using Ypdf.Web.Domain.Models;

namespace Ypdf.Web.AccoutAPI.Data.Repositories;

public interface ISubscriptionRepository
{
    Task<Subscription> GetByTypeAsync(SubscriptionType type);
}
