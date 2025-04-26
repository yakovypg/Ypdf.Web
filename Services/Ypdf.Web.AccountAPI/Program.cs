using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ypdf.Web.AccoutAPI;
using Ypdf.Web.AccoutAPI.Infrastructure.Data;

IHostBuilder hostBuilder = CreateHostBuilder(args);
IHost host = hostBuilder.Build();

using IServiceScope scope = host.Services.CreateScope();
var accountsDatabaseInitializer = new AccountsDatabaseInitializer(scope.ServiceProvider);

await accountsDatabaseInitializer
    .InitializeDatabaseAsync()
    .ConfigureAwait(false);

await host
    .RunAsync()
    .ConfigureAwait(false);

static IHostBuilder CreateHostBuilder(string[] args)
{
    ArgumentNullException.ThrowIfNull(args, nameof(args));

    return Host
        .CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((context, config) =>
        {
            string basePath = Directory.GetCurrentDirectory();
            string aspNetCoreEnvironment = context.HostingEnvironment.EnvironmentName;

            string defaultAppSettings = "appsettings.json";
            string environmentAppSetting = $"appsettings.{aspNetCoreEnvironment}.json";

            _ = config
                .SetBasePath(basePath)
                .AddEnvironmentVariables()
                .AddJsonFile(defaultAppSettings, optional: true)
                .AddJsonFile(environmentAppSetting, optional: true);
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
}
