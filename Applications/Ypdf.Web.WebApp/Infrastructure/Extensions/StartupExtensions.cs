using System;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ypdf.Web.Domain.Models.Configuration;
using Ypdf.Web.WebApp.Infrastructure.Services;
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
            .AddScoped<IHttpClientInteractorService, HttpClientInteractorService>()
            .AddScoped<IUiMessageService, UiMessageService>()
            .AddScoped<IJsElementInteractorService, JsElementInteractorService>();
    }

    public static IServiceCollection AddUtils(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));

        return services
            .AddScoped<AccountManager>()
            .AddScoped<HistoryPageSwitcher>();
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
