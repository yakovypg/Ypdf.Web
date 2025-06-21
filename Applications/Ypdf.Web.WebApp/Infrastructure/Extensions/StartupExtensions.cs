using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ypdf.Web.Domain.Models.Configuration;
using Ypdf.Web.WebApp.Infrastructure.Services.Api;
using Ypdf.Web.WebApp.Infrastructure.Services.Files;
using Ypdf.Web.WebApp.Infrastructure.Services.Http;
using Ypdf.Web.WebApp.Infrastructure.Services.UI;
using Ypdf.Web.WebApp.Infrastructure.Services.Users;
using Ypdf.Web.WebApp.Infrastructure.Utils;

namespace Ypdf.Web.WebApp.Infrastructure.Extensions;

public static class StartupExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));

        _ = services
            .AddSingleton<ICurrentUserService, CurrentUserService>()
            .AddSingleton<IHttpClientService, HttpClientService>();

        return services
            .AddScoped<IApiResponseReaderService, ApiResponseReaderService>()
            .AddScoped<IBrowserFileSavingService, BrowserFileSavingService>()
            .AddScoped<IFileContentCreatingService, FileContentCreatingService>()
            .AddScoped<IHttpClientInteractorService, HttpClientInteractorService>()
            .AddScoped<IJsElementInteractorService, JsElementInteractorService>()
            .AddScoped<ISubscriptionService, SubscriptionService>()
            .AddScoped<IUiMessageService, UiMessageService>();
    }

    public static IServiceCollection AddUtils(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));

        return services
            .AddScoped<AccountManager>()
            .AddScoped<HistoryPageSwitcher>();
    }

    public static IDataProtectionBuilder AddPersistentKeyStorage(
        this IServiceCollection services,
        IWebHostEnvironment environment,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(environment, nameof(environment));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        string keysDirectoryPath = configuration["Storages:PersistentKeyStorage"]
            ?? throw new ConfigurationException("Persistent key storage not specified");

        var keysDirectory = new DirectoryInfo(keysDirectoryPath);

        return services.AddDataProtection()
            .SetApplicationName("ypdf")
            .PersistKeysToFileSystem(keysDirectory)
            .SetDefaultKeyLifetime(TimeSpan.FromDays(14));
    }

    public static IApplicationBuilder UseLocalization(
        this IApplicationBuilder application,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(application, nameof(application));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        LocalizationConfiguration? localizationConfiguration = configuration
            .GetSection(nameof(LocalizationConfiguration))
            .Get<LocalizationConfiguration>();

        if (localizationConfiguration is null)
            return application;

        return application.UseRequestLocalization(options =>
        {
            options.SupportedCultures = localizationConfiguration.SupportedCultureNames
                .Select(e => CultureInfo.GetCultureInfo(e))
                .ToList();

            options.DefaultRequestCulture = new RequestCulture(
                localizationConfiguration.CultureName,
                localizationConfiguration.UiCultureName);
        });
    }
}
