using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ypdf.Web.AccoutAPI.Models;

namespace Ypdf.Web.AccoutAPI.Infrastructure.Data.Configuration;

public class UserSubscriptionConfiguration : IEntityTypeConfiguration<UserSubscription>
{
    public void Configure(EntityTypeBuilder<UserSubscription> builder)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));

        _ = builder.HasKey(us => us.Id);

        _ = builder
            .Property(us => us.Id)
            .ValueGeneratedOnAdd();

        _ = builder
            .HasOne(us => us.User)
            .WithOne(u => u.UserSubscription)
            .HasForeignKey<UserSubscription>(us => us.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
