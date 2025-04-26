using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Ypdf.Web.Domain.Commands;
using Ypdf.Web.Domain.Infrastructure.Handlers;
using Ypdf.Web.Domain.Models.Configuration;
using Ypdf.Web.PdfOperationsHistoryAPI.Commands;
using Ypdf.Web.PdfOperationsHistoryAPI.Infrastructure.Data.Repositories;
using Ypdf.Web.PdfOperationsHistoryAPI.Infrastructure.Services;
using Ypdf.Web.PdfOperationsHistoryAPI.Models.Requests;
using Ypdf.Web.PdfOperationsHistoryAPI.Models.Responses;

namespace Ypdf.Web.PdfOperationsHistoryAPI.Infrastructure.Extensions;

public static class StartupExtensions
{
    private const string StringFormatItem = "{0}";
    private const string SwaggerRouteTemplateItem = "{documentName}";
    private const string SwaggerRouteRoot = "api/doc";
    private const string SwaggerRouteTemplate = $"{SwaggerRouteRoot}/{StringFormatItem}/swagger.json";

    private const string ApiVersion = "v1";
    private const string ApiTitle = "Ypdf API";

    public static AuthenticationBuilder AddAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        AuthenticationBuilder builder = services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        });

        string issuer = configuration["Jwt:Issuer"]
            ?? throw new ConfigurationException("Issuer for Jwt not specified");

        string audience = configuration["Jwt:Audience"]
            ?? throw new ConfigurationException("Audience for Jwt not specified");

        string key = configuration["Jwt:Key"]
            ?? throw new ConfigurationException("Key for Jwt not specified");

        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        var issuerSigningKey = new SymmetricSecurityKey(keyBytes);

        return builder.AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = issuerSigningKey
            };
        });
    }

    public static IServiceCollection AddMapper(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));

        var mappingConfig = new MapperConfiguration(configuration =>
        {
        });

        IMapper mapper = mappingConfig.CreateMapper();

        return services
            .AddAutoMapper(typeof(Startup))
            .AddSingleton(mapper);
    }

    public static IServiceCollection AddCommands(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));

        return services
            .AddScoped<IProtectedCommand<GetHistoryRequest, GetHistoryResponse>, GetHistoryCommand>();
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        return services.AddHostedService<RabbitMqConsumerService>();
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        return services.AddSingleton<IPdfOperationResultRepository, PdfOperationResultRepository>();
    }

    public static IServiceCollection AddApiVersioning(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        string defaultApiVersion = configuration["DefaultApiVersion"] ?? "1.0";
        var apiVersion = new Version(defaultApiVersion);

        _ = services.AddApiVersioning(options =>
        {
            options.UseApiBehavior = true;
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;

            options.ApiVersionReader = new HeaderApiVersionReader("api-version");
            options.DefaultApiVersion = new ApiVersion(apiVersion.Major, apiVersion.Minor);
        });

        return services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
        });
    }

    public static IServiceCollection AddSwaggerGen(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));

        /*
        string assemblyXmlFile = $"{ExecutingAssemblyName}.xml";
        string assemblyXmlPath = Path.Combine(AppContext.BaseDirectory, assemblyXmlFile);
        */

        var apiInfo = new OpenApiInfo()
        {
            Title = ApiTitle,
            Version = ApiVersion
        };

        return services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(ApiVersion, apiInfo);

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter **Bearer {TOKEN}** to access this API",
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header
            });

            var openApiSecurityScheme = new OpenApiSecurityScheme()
            {
                Reference = new OpenApiReference()
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            };

            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                { openApiSecurityScheme, Array.Empty<string>() }
            });

            // options.IncludeXmlComments(assemblyXmlPath);
        });
    }

    public static IApplicationBuilder UseExceptionHandler(
        this IApplicationBuilder application,
        ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(application, nameof(application));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));

        var exceptionHandlerOptions = new ExceptionHandlerOptions()
        {
            AllowStatusCode404Response = true,

            ExceptionHandler = async httpContext => await ApiExceptionHandler
                .HandleExceptionAsync(httpContext, logger)
                .ConfigureAwait(false)
        };

        return application.UseExceptionHandler(exceptionHandlerOptions);
    }

    public static IApplicationBuilder UseSwagger(
        this IApplicationBuilder application,
        IApiVersionDescriptionProvider apiVersionDescriptionProvider)
    {
        ArgumentNullException.ThrowIfNull(application, nameof(application));
        ArgumentNullException.ThrowIfNull(apiVersionDescriptionProvider, nameof(apiVersionDescriptionProvider));

        // For caching
        const string swaggerRouteTemplate = SwaggerRouteTemplate;

        string routeTemplate = swaggerRouteTemplate.Replace(
            StringFormatItem,
            SwaggerRouteTemplateItem,
            StringComparison.InvariantCulture);

        _ = application.UseSwagger(options =>
        {
            options.RouteTemplate = routeTemplate;
        });

        IEnumerable<ApiVersionDescription> apiVersionDescriptions = apiVersionDescriptionProvider
            .ApiVersionDescriptions
            .OrderByDescending(e => e.ApiVersion.MajorVersion)
            .ThenByDescending(e => e.ApiVersion.MinorVersion);

        return application.UseSwaggerUI(options =>
        {
            foreach (ApiVersionDescription description in apiVersionDescriptions)
            {
                string groupName = description.GroupName.ToUpperInvariant();

                string url = string.Format(
                    CultureInfo.InvariantCulture,
                    swaggerRouteTemplate,
                    description.GroupName);

                options.SwaggerEndpoint($"/{url}", groupName);
            }

            options.DefaultModelsExpandDepth(-1);

            // options.SupportedSubmitMethods(SubmitMethod.Get);
            options.RoutePrefix = SwaggerRouteRoot;
        });
    }

    public static IServiceCollection ConfigureEndpoints(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));

        return services
            .AddMemoryCache()
            .AddRouting(options => options.LowercaseUrls = true);
    }
}
