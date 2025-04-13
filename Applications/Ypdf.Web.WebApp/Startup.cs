using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ypdf.Web.WebApp.Infrastructure.Extensions;

namespace Ypdf.Web.WebApp;

public class Startup
{
    public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    {
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
        ArgumentNullException.ThrowIfNull(webHostEnvironment, nameof(webHostEnvironment));

        Configuration = configuration;
        WebHostEnvironment = webHostEnvironment;
    }

    public IConfiguration Configuration { get; }
    public IWebHostEnvironment WebHostEnvironment { get; }

    public void Configure(
        IApplicationBuilder application,
        IApiVersionDescriptionProvider apiVersionDescriptionProvider,
        ILogger<Startup> logger)
    {
        ArgumentNullException.ThrowIfNull(application, nameof(application));
        ArgumentNullException.ThrowIfNull(apiVersionDescriptionProvider, nameof(apiVersionDescriptionProvider));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));

        _ = application
            .UseLocalization(Configuration);
    }

    public void ConfigureServices(IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
    }
}
