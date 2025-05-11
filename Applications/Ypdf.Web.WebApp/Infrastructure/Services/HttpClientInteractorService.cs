using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ypdf.Web.WebApp.Infrastructure.Services;

public class HttpClientInteractorService : IHttpClientInteractorService
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUiMessageService _messageService;

    public HttpClientInteractorService(
        IHttpClientService httpClientService,
        IUiMessageService messageService)
    {
        ArgumentNullException.ThrowIfNull(httpClientService, nameof(httpClientService));
        ArgumentNullException.ThrowIfNull(messageService, nameof(messageService));

        _httpClientService = httpClientService;
        _messageService = messageService;
    }

    public async Task GetAsync(string url, Action<HttpResponseMessage> successHandler)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(url, nameof(url));
        ArgumentNullException.ThrowIfNull(successHandler, nameof(successHandler));

        var uri = new Uri(url);

        await GetAsync(uri, successHandler)
            .ConfigureAwait(false);
    }

    public async Task GetAsync(Uri uri, Action<HttpResponseMessage> successHandler)
    {
        ArgumentNullException.ThrowIfNull(uri, nameof(uri));
        ArgumentNullException.ThrowIfNull(successHandler, nameof(successHandler));

        HttpResponseMessage responseMessage = await _httpClientService
            .GetAsync(uri)
            .ConfigureAwait(false);

        await HandleResponseAsync(responseMessage, successHandler)
            .ConfigureAwait(false);
    }

    public async Task PostAsync(string url, object data, Action<HttpResponseMessage> successHandler)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(url, nameof(url));
        ArgumentNullException.ThrowIfNull(data, nameof(data));
        ArgumentNullException.ThrowIfNull(successHandler, nameof(successHandler));

        var uri = new Uri(url);

        await PostAsync(uri, data, successHandler)
            .ConfigureAwait(false);
    }

    public async Task PostAsync(Uri uri, object data, Action<HttpResponseMessage> successHandler)
    {
        ArgumentNullException.ThrowIfNull(uri, nameof(uri));
        ArgumentNullException.ThrowIfNull(data, nameof(data));
        ArgumentNullException.ThrowIfNull(successHandler, nameof(successHandler));

        HttpResponseMessage responseMessage = await _httpClientService
            .PostAsync(uri, data)
            .ConfigureAwait(false);

        await HandleResponseAsync(responseMessage, successHandler)
            .ConfigureAwait(false);
    }

    public async Task PostAsync(string url, HttpContent data, Action<HttpResponseMessage> successHandler)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(url, nameof(url));
        ArgumentNullException.ThrowIfNull(data, nameof(data));
        ArgumentNullException.ThrowIfNull(successHandler, nameof(successHandler));

        var uri = new Uri(url);

        await PostAsync(uri, data, successHandler)
            .ConfigureAwait(false);
    }

    public async Task PostAsync(Uri uri, HttpContent data, Action<HttpResponseMessage> successHandler)
    {
        ArgumentNullException.ThrowIfNull(uri, nameof(uri));
        ArgumentNullException.ThrowIfNull(data, nameof(data));
        ArgumentNullException.ThrowIfNull(successHandler, nameof(successHandler));

        HttpResponseMessage responseMessage = await _httpClientService
            .PostAsync(uri, data)
            .ConfigureAwait(false);

        await HandleResponseAsync(responseMessage, successHandler)
            .ConfigureAwait(false);
    }

    private async Task HandleResponseAsync(
        HttpResponseMessage responseMessage,
        Action<HttpResponseMessage> successHandler)
    {
        ArgumentNullException.ThrowIfNull(responseMessage, nameof(responseMessage));
        ArgumentNullException.ThrowIfNull(successHandler, nameof(successHandler));

        if (responseMessage.IsSuccessStatusCode)
        {
            successHandler.Invoke(responseMessage);
        }
        else
        {
            await _messageService
                .ShowErrorAsync(responseMessage)
                .ConfigureAwait(false);
        }
    }
}
