using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Ypdf.Web.WebApp.Infrastructure.Services;

public class UiMessageService : IUiMessageService
{
    private readonly IJSRuntime _runtime;

    public UiMessageService(IJSRuntime runtime)
    {
        ArgumentNullException.ThrowIfNull(runtime, nameof(runtime));
        _runtime = runtime;
    }

    public async Task ShowAlertAsync(string message)
    {
        ArgumentNullException.ThrowIfNull(message, nameof(message));

        await _runtime
            .InvokeVoidAsync("alert", message)
            .ConfigureAwait(false);
    }

    public async Task ShowErrorAsync(HttpResponseMessage responseMessage)
    {
        ArgumentNullException.ThrowIfNull(responseMessage, nameof(responseMessage));

        string errorMessage = await responseMessage.Content
            .ReadAsStringAsync()
            .ConfigureAwait(false);

        if (string.IsNullOrEmpty(errorMessage))
            errorMessage = responseMessage.ReasonPhrase ?? string.Empty;

        await ShowAlertAsync(errorMessage)
            .ConfigureAwait(false);
    }
}
