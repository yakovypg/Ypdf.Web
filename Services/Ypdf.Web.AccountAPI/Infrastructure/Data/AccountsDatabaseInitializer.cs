using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ypdf.Web.AccoutAPI.Models;
using Ypdf.Web.Domain.Models;

namespace Ypdf.Web.AccoutAPI.Infrastructure.Data;

public class AccountsDatabaseInitializer
{
    private readonly AccountsDbContext _accountsDbContext;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;

    private readonly IConfiguration _configuration;
    private readonly ILogger<AccountsDatabaseInitializer> _logger;

    public AccountsDatabaseInitializer(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider, nameof(serviceProvider));

        _accountsDbContext = serviceProvider.GetRequiredService<AccountsDbContext>();
        _userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        _roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

        _configuration = serviceProvider.GetRequiredService<IConfiguration>();
        _logger = serviceProvider.GetRequiredService<ILogger<AccountsDatabaseInitializer>>();
    }

    public async Task InitializeDatabaseAsync()
    {
        _logger.LogInformation("Start database initialization");

        ApplyMigrations();

        await AddInitialDataAsync()
            .ConfigureAwait(false);

        _logger.LogInformation("Database initialized");
    }

    private void ApplyMigrations()
    {
        _logger.LogInformation("Start applying migrations");

        _accountsDbContext.Database.Migrate();

        IEnumerable<string> pendingMigrations = _accountsDbContext.Database.GetPendingMigrations();
        IMigrator migrator = _accountsDbContext.GetService<IMigrator>();

        foreach (string migration in pendingMigrations)
        {
            _logger.LogInformation("Trying apply migration {Migration}", migration);
            migrator.Migrate(migration);
            _logger.LogInformation("Migration {Migration} applied", migration);
        }

        _logger.LogInformation("Migrations applied");
    }

    private async Task AddInitialDataAsync()
    {
        _logger.LogInformation("Start adding initial data");

        await AddInitialSubscriptionsAsync()
            .ConfigureAwait(false);

        await AddInitialRolesAsync()
            .ConfigureAwait(false);

        await AddInitialUsersAsync()
            .ConfigureAwait(false);

        _logger.LogInformation("Initial data added");
    }

    private async Task AddInitialSubscriptionsAsync()
    {
        _logger.LogInformation("Start adding initial subscriptions");

        DbSet<Subscription> subscriptions = _accountsDbContext.Set<Subscription>();

        if (subscriptions.Any())
        {
            _logger.LogInformation("Initial subscriptions already added");
            return;
        }

        foreach (Subscription subscription in AccountsDatabaseInitialData.Subscriptions)
        {
            AddSubsciption(subscriptions, subscription);
        }

        _logger.LogInformation("Trying to save changes");

        await _accountsDbContext
            .SaveChangesAsync()
            .ConfigureAwait(false);

        _logger.LogInformation("Changes saved");
        _logger.LogInformation("Initial subscriptions added");
    }

    private void AddSubsciption(DbSet<Subscription> subscriptions, Subscription subscription)
    {
        ArgumentNullException.ThrowIfNull(subscriptions, nameof(subscriptions));
        ArgumentNullException.ThrowIfNull(subscription, nameof(subscription));

        _logger.LogInformation(
            "Trying to add {Subscription} subscription",
            subscription.SubscriptionType);

        subscriptions.Add(subscription);

        _logger.LogInformation(
            "Subscription {Subscription} added",
            subscription.SubscriptionType);
    }

    private async Task AddInitialRolesAsync()
    {
        _logger.LogInformation("Start adding initial roles");

        string[] roles = Enum.GetNames<UserRole>();

        foreach (string role in roles)
        {
            bool roleExists = await _roleManager
                .RoleExistsAsync(role)
                .ConfigureAwait(false);

            if (roleExists)
            {
                _logger.LogInformation("Role {Role} already exists", role);
                continue;
            }

            await AddRoleAsync(role)
                .ConfigureAwait(false);
        }

        _logger.LogInformation("Initial roles added");
    }

    private async Task AddRoleAsync(string role)
    {
        ArgumentNullException.ThrowIfNull(role, nameof(role));

        _logger.LogInformation("Trying to add {Role} role", role);

        IdentityResult createRoleResult = await _roleManager
            .CreateAsync(new IdentityRole<int>(role))
            .ConfigureAwait(false);

        _logger.LogInformation(
            "Result of adding {Role} role: {@Result}",
            role,
            createRoleResult);
    }

    private async Task AddInitialUsersAsync()
    {
        _logger.LogInformation("Start adding initial users");

        await AddAdminAsync()
            .ConfigureAwait(false);

        await AddTestUserAsync()
            .ConfigureAwait(false);

        _logger.LogInformation("Initial users added");
    }

    private async Task AddAdminAsync()
    {
        (User user, UserRole role, string password) = AccountsDatabaseInitialData
            .GetAdmin(_configuration);

        await AddUserAsync(user, role, password)
            .ConfigureAwait(false);
    }

    private async Task AddTestUserAsync()
    {
        (User user, UserRole role, string password) = AccountsDatabaseInitialData
            .GetTestUser(_configuration);

        await AddUserAsync(user, role, password)
            .ConfigureAwait(false);

        await AddSubscriptionToUserAsync(user, SubscriptionType.Standard)
            .ConfigureAwait(false);
    }

    private async Task AddUserAsync(User user, UserRole role, string password)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));
        ArgumentNullException.ThrowIfNull(user.Email, nameof(user.Email));
        ArgumentNullException.ThrowIfNull(password, nameof(password));

        _logger.LogInformation("Trying to add user with email {Email}", user.Email);

        User? foundUser = await _userManager
            .FindByEmailAsync(user.Email)
            .ConfigureAwait(false);

        if (foundUser is not null)
        {
            _logger.LogInformation("User with email {Email} already exists", user.Email);
            return;
        }

        IdentityResult createUserResult = await _userManager
            .CreateAsync(user, password)
            .ConfigureAwait(false);

        _logger.LogInformation(
            "Result of adding user with email {Email}: {@Result}",
            user.Email,
            createUserResult);

        if (!createUserResult.Succeeded)
            return;

        IdentityResult addToRoleResult = await _userManager
            .AddToRoleAsync(user, role.ToString())
            .ConfigureAwait(false);

        _logger.LogInformation(
            "Result of adding role {Role} to user with email {Email}: {@Result}",
            role,
            user.Email,
            addToRoleResult);
    }

    private async Task AddSubscriptionToUserAsync(User user, SubscriptionType subscriptionType)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));
        ArgumentNullException.ThrowIfNull(user.Email, nameof(user.Email));

        _logger.LogInformation(
            "Trying to add subscription {SubscriptionType} to user with email {Email}",
            subscriptionType,
            user.Email);

        Subscription? foundSubscription = AccountsDatabaseInitialData.Subscriptions
            .FirstOrDefault(t => t.SubscriptionType == subscriptionType);

        if (foundSubscription is null)
        {
            _logger.LogInformation(
                "Subscription {SubscriptionType} not found. It cannot be added to user with email {Email}",
                subscriptionType,
                user.Email);

            return;
        }

        var userSubscription = new UserSubscription()
        {
            Subscription = foundSubscription,
            User = user,
            ExpiresAt = DateTimeOffset.MaxValue
        };

        await _accountsDbContext.UserSubscriptions
            .AddAsync(userSubscription)
            .ConfigureAwait(false);

        user.UserSubscription = userSubscription;

        await _accountsDbContext
            .SaveChangesAsync()
            .ConfigureAwait(false);

        _logger.LogInformation(
            "Subscription {SubscriptionType} added to user with email {Email}",
            subscriptionType,
            user.Email);
    }
}
