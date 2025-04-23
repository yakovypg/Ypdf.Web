using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Ypdf.Web.AccoutAPI.Models;

namespace Ypdf.Web.AccoutAPI.Infrastructure.Data;

public static class AccountsDatabaseInitializer
{
    public static void InitializeDatabase(IServiceProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider, nameof(provider));

        AccountsDbContext accountsDbContext = provider.GetRequiredService<AccountsDbContext>();

        ApplyMigrations(accountsDbContext);
        AddInitialData(accountsDbContext);
    }

    private static void ApplyMigrations(AccountsDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        context.Database.Migrate();

        IEnumerable<string> pendingMigrations = context.Database.GetPendingMigrations();
        IMigrator migrator = context.GetService<IMigrator>();

        foreach (string migration in pendingMigrations)
        {
            migrator.Migrate(migration);
        }
    }

    private static void AddInitialData(AccountsDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));
        AddInitialSubscriptions(context);
    }

    private static void AddInitialSubscriptions(AccountsDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        DbSet<Subscription> subscriptions = context.Set<Subscription>();

        if (subscriptions.Any())
            return;

        foreach (Subscription subscription in AccountsDatabaseInitialData.Subscriptions)
        {
            subscriptions.Add(subscription);
        }

        context.SaveChanges();
    }
}
