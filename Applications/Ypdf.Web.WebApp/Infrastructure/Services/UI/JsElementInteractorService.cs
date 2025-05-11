using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Ypdf.Web.WebApp.Infrastructure.Services.UI;

public class JsElementInteractorService : IJsElementInteractorService
{
    private readonly IJSRuntime _runtime;

    public JsElementInteractorService(IJSRuntime runtime)
    {
        ArgumentNullException.ThrowIfNull(runtime, nameof(runtime));
        _runtime = runtime;
    }

    public async Task SetDisabledAsync(string itemId, bool disabled)
    {
        ArgumentNullException.ThrowIfNull(itemId, nameof(itemId));

#pragma warning disable CA1308 // Normalize strings to uppercase
        string disabledText = disabled
            .ToString()
            .ToLowerInvariant();
#pragma warning restore CA1308 // Normalize strings to uppercase

        string js = $"document.getElementById('{itemId}').disabled = {disabledText};";

        await _runtime
            .InvokeVoidAsync("eval", js)
            .ConfigureAwait(false);
    }

    public async Task SetInnerHtmlAsync(string selector, string html)
    {
        ArgumentNullException.ThrowIfNull(selector, nameof(selector));
        ArgumentNullException.ThrowIfNull(html, nameof(html));

        await _runtime
            .InvokeVoidAsync("eval", $"document.querySelector('{selector}').innerHTML = '{html}';")
            .ConfigureAwait(false);
    }

    public async Task InsertAdjacentHtmlAsync(string selector, string html)
    {
        ArgumentNullException.ThrowIfNull(selector, nameof(selector));
        ArgumentNullException.ThrowIfNull(html, nameof(html));

        string js = $"document.querySelector('{selector}').insertAdjacentHTML('beforeend', `{html}`);";

        await _runtime
            .InvokeVoidAsync("eval", js)
            .ConfigureAwait(false);
    }
}
