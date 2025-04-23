using System.Collections.Generic;
using Ypdf.Web.AccoutAPI.Models;

namespace Ypdf.Web.AccoutAPI.Infrastructure.Data;

public static class AccountsDatabaseInitialData
{
    static AccountsDatabaseInitialData()
    {
        Subscriptions =
        [
            new Subscription() { SubscriptionType = SubscriptionType.Trial },
            new Subscription() { SubscriptionType = SubscriptionType.Standard },
        ];
    }

    public static IReadOnlyList<Subscription> Subscriptions { get; }
}
