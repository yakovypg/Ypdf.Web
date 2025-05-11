using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Ypdf.Web.WebApp.Infrastructure.Services.Http;

public class HttpClientService : IHttpClientService, IDisposable
{
    private const string Scheme = "Bearer";

    private readonly HttpClientHandler _httpClientHandler;
    private readonly HttpClient _httpClient;

    private bool _isDisposed;

    public HttpClientService()
    {
        _httpClientHandler = new HttpClientHandler()
        {
            // Y-TODO: remove it
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        };

        _httpClient = new HttpClient(_httpClientHandler);
    }

    ~HttpClientService()
    {
        Dispose(false);
    }

    public void SetAuthorizationToken(string token)
    {
        ArgumentException.ThrowIfNullOrEmpty(token, nameof(token));

        var authenticationHeaderValue = new AuthenticationHeaderValue(Scheme, token);
        _httpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
    }

    public async Task<HttpResponseMessage> GetAsync(Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri, nameof(uri));

        return await _httpClient
            .GetAsync(uri)
            .ConfigureAwait(false);
    }

    public async Task<HttpResponseMessage> PostAsync(Uri uri, object data)
    {
        ArgumentNullException.ThrowIfNull(uri, nameof(uri));
        ArgumentNullException.ThrowIfNull(data, nameof(data));

        return await _httpClient
            .PostAsJsonAsync(uri, data)
            .ConfigureAwait(false);
    }

    public async Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent data)
    {
        ArgumentNullException.ThrowIfNull(uri, nameof(uri));
        ArgumentNullException.ThrowIfNull(data, nameof(data));

        return await _httpClient
            .PostAsync(uri, data)
            .ConfigureAwait(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                _httpClientHandler?.Dispose();
                _httpClient?.Dispose();
            }

            _isDisposed = true;
        }
    }
}
