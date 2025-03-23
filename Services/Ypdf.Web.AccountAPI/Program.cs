using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ypdf.Web.AccoutAPI;
using Ypdf.Web.AccoutAPI.Infrastructure.Data;

IHostBuilder builder = Host.CreateDefaultBuilder(args);

_ = builder.ConfigureWebHostDefaults(webBuilder =>
{
    webBuilder.UseStartup<Startup>();
});

IHost host = builder.Build();

using IServiceScope scope = host.Services.CreateScope();
AccountsDatabaseInitializer.InitializeDatabase(scope.ServiceProvider);

host.Run();
