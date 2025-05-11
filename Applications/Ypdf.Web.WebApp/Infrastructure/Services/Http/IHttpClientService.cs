using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ypdf.Web.WebApp.Infrastructure.Services.Http;

public interface IHttpClientService
{
    void SetAuthorizationToken(string token);

    Task<HttpResponseMessage> GetAsync(Uri uri);
    Task<HttpResponseMessage> PostAsync(Uri uri, object data);
    Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent data);
}
