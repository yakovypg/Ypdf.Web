using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;

namespace Ypdf.Web.AccoutAPI.Infrastructure.Data;

public static class AccountsDatabaseInitializer
{
    public static void InitializeDatabase(IServiceProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider, nameof(provider));

        AccountsDbContext accountsDbContext = provider.GetRequiredService<AccountsDbContext>();
        ApplyMigrations(accountsDbContext);
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
}
