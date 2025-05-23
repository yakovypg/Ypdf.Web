using System.Threading.Tasks;
using Ypdf.Web.Domain.Models;

namespace Ypdf.Web.WebApp.Infrastructure.Services.Users;

public interface ISubscriptionService
{
    Task ActivateAsync(SubscriptionType subscriptionType);
}
