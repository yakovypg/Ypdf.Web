using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Ypdf.Web.Domain.Models.Api;
using Ypdf.Web.WebApp.Infrastructure.Services.UI;

namespace Ypdf.Web.WebApp.Infrastructure.Services.Api;

public class ApiResponseReaderService : IApiResponseReaderService
{
    private readonly IUiMessageService _messageService;

    public ApiResponseReaderService(IUiMessageService messageService)
    {
        ArgumentNullException.ThrowIfNull(messageService, nameof(messageService));
        _messageService = messageService;
    }

    public async Task<T?> ReadAsync<T>(HttpResponseMessage responseMessage)
    {
        ArgumentNullException.ThrowIfNull(responseMessage, nameof(responseMessage));

        ApiResponse<T>? response = await responseMessage.Content
            .ReadFromJsonAsync<ApiResponse<T>>()
            .ConfigureAwait(false);

        return response is null
            ? default
            : response.Result;
    }

    public async Task<(bool Success, T? Response)> ReadWithInfoAsync<T>(
        HttpResponseMessage responseMessage)
    {
        ArgumentNullException.ThrowIfNull(responseMessage, nameof(responseMessage));

        T? response = await ReadAsync<T>(responseMessage)
            .ConfigureAwait(false);

        if (response is null)
        {
            await _messageService
                .ShowAlertAsync("Failed to load history")
                .ConfigureAwait(false);

            return (false, default);
        }

        return (true, response);
    }
}
