using System;
using System.Threading.Tasks;
using Ypdf.Web.AccoutAPI.Models;

namespace Ypdf.Web.AccoutAPI.Infrastructure.Services;

public interface IUserSubscriptionService
{
    Task AddSubscriptionAsync(User user, SubscriptionType subscriptionType, TimeSpan period);
}
