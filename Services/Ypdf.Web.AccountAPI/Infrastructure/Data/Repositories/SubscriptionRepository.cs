using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ypdf.Web.AccoutAPI.Infrastructure.Data;
using Ypdf.Web.AccoutAPI.Models;

namespace Ypdf.Web.AccoutAPI.Data.Repositories;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly AccountsDbContext _context;

    public SubscriptionRepository(AccountsDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));
        _context = context;
    }

    public async Task<Subscription> GetByTypeAsync(SubscriptionType type)
    {
        return await _context.Subscriptions
            .SingleAsync(t => t.SubscriptionType == type)
            .ConfigureAwait(false);
    }
}
