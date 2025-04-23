using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ypdf.Web.AccoutAPI.Data.Repositories;
using Ypdf.Web.AccoutAPI.Infrastructure.Data;
using Ypdf.Web.AccoutAPI.Models;
using Ypdf.Web.Domain.Models.Api.Exceptions;

namespace Ypdf.Web.AccoutAPI.Infrastructure.Services;

public class UserSubscriptionService : IUserSubscriptionService
{
    private readonly AccountsDbContext _context;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ILogger<UserSubscriptionService> _logger;

    public UserSubscriptionService(
        AccountsDbContext context,
        ISubscriptionRepository subscriptionRepository,
        ILogger<UserSubscriptionService> logger)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));
        ArgumentNullException.ThrowIfNull(subscriptionRepository, nameof(subscriptionRepository));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));

        _context = context;
        _subscriptionRepository = subscriptionRepository;
        _logger = logger;
    }

    public async Task AddSubscriptionAsync(
        User user,
        SubscriptionType subscriptionType,
        TimeSpan period)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));
        ArgumentNullException.ThrowIfNull(subscriptionType, nameof(subscriptionType));

        _logger.LogInformation(
            "Trying to add subscription '{Type}' for user with ID {Id}",
            subscriptionType,
            user.Id);

        if (user.UserSubscription is null
            || user.UserSubscription.Subscription.SubscriptionType != subscriptionType)
        {
            await AddNewSubscriptionAsync(user, subscriptionType, period)
                .ConfigureAwait(false);
        }
        else
        {
            await RenewSubscriptionAsync(user, period)
                .ConfigureAwait(false);
        }
    }

    private async Task AddNewSubscriptionAsync(
        User user,
        SubscriptionType subscriptionType,
        TimeSpan period)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));
        ArgumentNullException.ThrowIfNull(subscriptionType, nameof(subscriptionType));

        _logger.LogInformation(
            "Trying to add new subscription '{Type}' for user with ID {Id}",
            subscriptionType,
            user.Id);

        Subscription subscription = await _subscriptionRepository
            .GetByTypeAsync(subscriptionType)
            .ConfigureAwait(false);

        DateTimeOffset expiresAt = DateTimeOffset.UtcNow.Add(period);

        var userSubscription = new UserSubscription()
        {
            User = user,
            Subscription = subscription,
            ExpiresAt = expiresAt
        };

        await _context.UserSubscriptions
            .AddAsync(userSubscription)
            .ConfigureAwait(false);

        _logger.LogInformation(
            "Subscription {@Subscription} for user with ID {Id} created",
            userSubscription,
            user.Id);

        // Should we remove previous one?
        user.UserSubscription = userSubscription;

        await _context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        _logger.LogInformation(
            "Subscription '{Type}' for user with ID {Id} added",
            subscriptionType,
            user.Id);
    }

    private async Task RenewSubscriptionAsync(User user, TimeSpan period)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));

        _logger.LogInformation("Trying to renew a subscription for user with ID {Id}", user.Id);

        if (user.UserSubscription is null)
        {
            _logger.LogWarning(
                "User {Id} doesn't have a subscription. It cannot be renewed",
                user.Id);

            throw new InternalException("User doesn't have a subscription");
        }

        user.UserSubscription.ExpiresAt = user.UserSubscription.ExpiresAt < DateTimeOffset.UtcNow
            ? DateTimeOffset.UtcNow.Add(period)
            : user.UserSubscription.ExpiresAt.Add(period);

        await _context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        _logger.LogInformation(
            "Subscription '{Type}' for user with ID {Id} renewed",
            user.UserSubscription.Subscription.SubscriptionType,
            user.Id);
    }
}
