using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Ypdf.Web.WebApp.Infrastructure.Services;

public class HttpClientService : IHttpClientService, IDisposable
{
    private readonly HttpClient _httpClient;
    private bool _isDisposed;

    public HttpClientService()
    {
        _httpClient = new HttpClient();
    }

    ~HttpClientService()
    {
        Dispose(false);
    }

    public async Task<HttpResponseMessage> GetAsync(string url)
    {
        var uri = new Uri(url);

        return await GetAsync(uri)
            .ConfigureAwait(false);
    }

    public async Task<HttpResponseMessage> GetAsync(Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri, nameof(uri));

        return await _httpClient
            .GetAsync(uri)
            .ConfigureAwait(false);
    }

    public async Task<HttpResponseMessage> PostAsync(string url, object data)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(url, nameof(url));
        ArgumentNullException.ThrowIfNull(data, nameof(data));

        var uri = new Uri(url);

        return await PostAsync(uri, data)
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
                _httpClient?.Dispose();

            _isDisposed = true;
        }
    }
}
