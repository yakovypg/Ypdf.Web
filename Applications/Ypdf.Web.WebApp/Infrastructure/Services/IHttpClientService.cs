using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ypdf.Web.WebApp.Infrastructure.Services;

public interface IHttpClientService
{
    void SetAuthorizationToken(string token);

    Task<HttpResponseMessage> GetAsync(string url);
    Task<HttpResponseMessage> GetAsync(Uri uri);
    Task<HttpResponseMessage> PostAsync(string url, object data);
    Task<HttpResponseMessage> PostAsync(Uri uri, object data);
}
