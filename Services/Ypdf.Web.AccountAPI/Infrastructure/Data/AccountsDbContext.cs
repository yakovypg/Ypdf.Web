using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ypdf.Web.AccoutAPI.Models;

namespace Ypdf.Web.AccoutAPI.Infrastructure.Data;

public class AccountsDbContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public AccountsDbContext(DbContextOptions<AccountsDbContext> options)
        : base(options ?? throw new ArgumentNullException(nameof(options)))
    {
    }

    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<UserSubscription> UserSubscriptions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));
        base.OnModelCreating(builder);
    }
}
