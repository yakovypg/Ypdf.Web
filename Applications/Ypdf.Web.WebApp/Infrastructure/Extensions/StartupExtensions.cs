using System;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Ypdf.Web.Domain.Models.Configuration;

namespace Ypdf.Web.WebApp.Infrastructure.Extensions;

public static class StartupExtensions
{
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
