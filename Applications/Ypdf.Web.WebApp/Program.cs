using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ypdf.Web.WebApp.Components;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

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
