using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Ypdf.Web.AccoutAPI.Commands;
using Ypdf.Web.AccoutAPI.Data.Repositories;
using Ypdf.Web.AccoutAPI.Infrastructure.Configuration;
using Ypdf.Web.AccoutAPI.Infrastructure.Data;
using Ypdf.Web.AccoutAPI.Infrastructure.Services;
using Ypdf.Web.AccoutAPI.Infrastructure.Services.Authentication;
using Ypdf.Web.AccoutAPI.Infrastructure.Services.Verification;
using Ypdf.Web.AccoutAPI.Mappings;
using Ypdf.Web.AccoutAPI.Models;
using Ypdf.Web.Domain.Commands;
using Ypdf.Web.Domain.Infrastructure.Handlers;
using Ypdf.Web.Domain.Models.Api.Requests;
using Ypdf.Web.Domain.Models.Api.Responses;
using Ypdf.Web.Domain.Models.Configuration;

namespace Ypdf.Web.AccoutAPI.Infrastructure.Extensions;

public static class StartupExtensions
{
    private const string StringFormatItem = "{0}";
    private const string SwaggerRouteTemplateItem = "{documentName}";
    private const string SwaggerRouteRoot = "api/doc";
    private const string SwaggerRouteTemplate = $"{SwaggerRouteRoot}/{StringFormatItem}/swagger.json";

    private const string ApiVersion = "v1";
    private const string ApiTitle = "Ypdf API";

    private static readonly string? ExecutingAssemblyName = Assembly
        .GetExecutingAssembly()
        .GetName()
        .Name;

    public static PasswordRequirements AddPasswordRequirements(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        PasswordRequirements? passwordRequirements = configuration
            .GetSection(nameof(PasswordRequirements))
            .Get<PasswordRequirements>();

        if (passwordRequirements is null)
            throw new ConfigurationException("Password requirements not specified");

        _ = services.AddSingleton(passwordRequirements!);

        return passwordRequirements;
    }

    public static IServiceCollection AddMapper(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));

        var mappingConfig = new MapperConfiguration(configuration =>
        {
            configuration.AddProfile(new EntityMappingProfile());
        });

        IMapper mapper = mappingConfig.CreateMapper();

        return services
            .AddAutoMapper(typeof(Startup))
            .AddSingleton(mapper);
    }

    public static IServiceCollection AddAccountsDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        string connectionString = configuration.GetConnectionString("Accounts")
            ?? throw new ConfigurationException("Connection string to Accounts not specified");

        return services.AddDbContext<AccountsDbContext>(t =>
        {
            t.UseNpgsql(
                connectionString,
                x => x.MigrationsAssembly(ExecutingAssemblyName));
        });
    }

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

    public static IdentityBuilder AddIdentity(
        this IServiceCollection services,
        PasswordRequirements passwordRequirements)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(passwordRequirements, nameof(passwordRequirements));

        IdentityBuilder builder = services.AddIdentity<User, IdentityRole<int>>(setup =>
        {
            setup.User.RequireUniqueEmail = true;
            setup.Password.RequiredLength = passwordRequirements.MinimumLength;
            setup.Password.RequiredUniqueChars = passwordRequirements.RequiredUniqueChars;
            setup.Password.RequireDigit = passwordRequirements.RequireDigit;
            setup.Password.RequireNonAlphanumeric = passwordRequirements.RequireNonAlphanumeric;
            setup.Password.RequireLowercase = passwordRequirements.RequireLowercase;
            setup.Password.RequireUppercase = passwordRequirements.RequireUppercase;
        });

        return builder
            .AddEntityFrameworkStores<AccountsDbContext>()
            .AddDefaultTokenProviders();
    }

    public static IServiceCollection AddCommands(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));

        return services
            .AddScoped<ICommand<AddSubscriptionRequest, AddSubscriptionResponse>, AddSubscriptionCommand>()
            .AddScoped<ICommand<RegisterUserRequest, RegisterUserResponse>, RegisterUserCommand>()
            .AddScoped<ICommand<LoginRequest, LoginResponse>, LoginCommand>();
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));

        return services
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<ISubscriptionRepository, SubscriptionRepository>();
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));

        return services
            .AddScoped<IEmailVerifierService, EmailVerifierService>()
            .AddScoped<IPasswordVerifierService, PasswordVerifierService>()
            .AddScoped<INicknameVerifierService, NicknameVerifierService>()
            .AddScoped<IUserSubscriptionService, UserSubscriptionService>()
            .AddScoped<ITokenGenerationService, TokenGenerationService>()
            .AddScoped<ISignInService, SignInService>();
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
