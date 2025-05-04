using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ypdf.Web.WebApp.Infrastructure.Services;

public interface IHttpClientInteractorService
{
    Task GetAsync(string url, Action<HttpResponseMessage> successHandler);
    Task GetAsync(Uri uri, Action<HttpResponseMessage> successHandler);
    Task PostAsync(string url, object data, Action<HttpResponseMessage> successHandler);
    Task PostAsync(Uri uri, object data, Action<HttpResponseMessage> successHandler);
}
