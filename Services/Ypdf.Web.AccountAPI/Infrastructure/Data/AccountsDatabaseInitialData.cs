using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Ypdf.Web.AccoutAPI.Models;
using Ypdf.Web.Domain.Models;
using Ypdf.Web.Domain.Models.Configuration;

namespace Ypdf.Web.AccoutAPI.Infrastructure.Data;

public static class AccountsDatabaseInitialData
{
    static AccountsDatabaseInitialData()
    {
        Subscriptions =
        [
            new() { SubscriptionType = SubscriptionType.Trial },
            new() { SubscriptionType = SubscriptionType.Standard }
        ];
    }

    public static IReadOnlyList<Subscription> Subscriptions { get; }

    public static (User User, UserRole Role, string Password) GetAdmin(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        string email = configuration["InitialUsers:Admin:Email"]
            ?? throw new ConfigurationException("Email for admin not specified");

        string userName = configuration["InitialUsers:Admin:UserName"]
            ?? throw new ConfigurationException("User name for admin not specified");

        string nickname = configuration["InitialUsers:Admin:Nickname"]
            ?? throw new ConfigurationException("Nickname for admin not specified");

        string password = configuration["InitialUsers:Admin:Password"]
            ?? throw new ConfigurationException("Password for admin not specified");

        var user = new User()
        {
            Email = email,
            UserName = userName,
            Nickname = nickname
        };

        return (user, UserRole.Admin, password);
    }

    public static (User User, UserRole Role, string Password) GetTestUser(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        string email = configuration["InitialUsers:TestUser:Email"]
            ?? throw new ConfigurationException("Email for test user not specified");

        string userName = configuration["InitialUsers:TestUser:UserName"]
            ?? throw new ConfigurationException("User name for test user not specified");

        string nickname = configuration["InitialUsers:TestUser:Nickname"]
            ?? throw new ConfigurationException("Nickname for test user not specified");

        string password = configuration["InitialUsers:TestUser:Password"]
            ?? throw new ConfigurationException("Password for test user not specified");

        var user = new User()
        {
            Email = email,
            UserName = userName,
            Nickname = nickname
        };

        return (user, UserRole.User, password);
    }
}
