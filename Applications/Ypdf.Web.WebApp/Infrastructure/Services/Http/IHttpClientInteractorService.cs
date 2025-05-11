using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ypdf.Web.WebApp.Infrastructure.Services.Http;

public interface IHttpClientInteractorService
{
    Task GetAsync(Uri uri, Action<HttpResponseMessage> successHandler);
    Task PostAsync(Uri uri, object data, Action<HttpResponseMessage> successHandler);
    Task PostAsync(Uri uri, HttpContent data, Action<HttpResponseMessage> successHandler);
}
