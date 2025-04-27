using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ypdf.Web.WebApp.Components;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

_ = builder.Services.AddServerSideBlazor();

_ = builder.Services
    .AddBlazorise(options =>
    {
        options.Immediate = true;
    })
    .AddBootstrapProviders()
    .AddFontAwesomeIcons();

WebApplication webApplication = builder.Build();

if (!webApplication.Environment.IsDevelopment())
    webApplication.UseHsts();

webApplication
    .UseHttpsRedirection()
    .UseStaticFiles()
    .UseAntiforgery();

webApplication
    .MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

webApplication.Run();
