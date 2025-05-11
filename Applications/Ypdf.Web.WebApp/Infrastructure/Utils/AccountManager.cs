using System;
using System.Net.Http;
using System.Threading.Tasks;
using Ypdf.Web.Domain.Models.Api.Responses;
using Ypdf.Web.WebApp.Infrastructure.Configuration;
using Ypdf.Web.WebApp.Infrastructure.Services.Api;
using Ypdf.Web.WebApp.Infrastructure.Services.Http;
using Ypdf.Web.WebApp.Infrastructure.Services.UI;

namespace Ypdf.Web.WebApp.Infrastructure.Utils;

public class AccountManager
{
    private readonly IUiMessageService _messageService;
    private readonly IApiResponseReaderService _apiResponseReaderService;
    private readonly IHttpClientInteractorService _httpClientInteractorService;

    public AccountManager(
        IUiMessageService messageService,
        IApiResponseReaderService apiResponseReaderService,
        IHttpClientInteractorService httpClientInteractor)
    {
        ArgumentNullException.ThrowIfNull(messageService, nameof(messageService));
        ArgumentNullException.ThrowIfNull(apiResponseReaderService, nameof(apiResponseReaderService));
        ArgumentNullException.ThrowIfNull(httpClientInteractor, nameof(httpClientInteractor));

        _messageService = messageService;
        _apiResponseReaderService = apiResponseReaderService;
        _httpClientInteractorService = httpClientInteractor;
    }

    public async Task ChangePasswordAsync()
    {
        await _messageService
            .ShowAlertAsync("Not supported yet")
            .ConfigureAwait(false);
    }

    public async Task LoginAsync(
        string email,
        string password,
        Action<LoginResponse> successHandler)
    {
        ArgumentNullException.ThrowIfNull(email, nameof(email));
        ArgumentNullException.ThrowIfNull(password, nameof(password));
        ArgumentNullException.ThrowIfNull(successHandler, nameof(successHandler));

        async void SuccessHandler(HttpResponseMessage responseMessage)
        {
            (bool success, LoginResponse? response) = await _apiResponseReaderService
                .ReadWithInfoAsync<LoginResponse>(responseMessage)
                .ConfigureAwait(false);

            if (!success || response is null)
            {
                await _messageService
                    .ShowAlertAsync("Failed to register user")
                    .ConfigureAwait(false);

                return;
            }

            successHandler.Invoke(response);
        }

        var data = new
        {
            Email = email,
            Password = password
        };

        await _httpClientInteractorService
            .PostAsync(EndpointUrls.Login, data, SuccessHandler)
            .ConfigureAwait(false);
    }

    public async Task RegisterAsync(
        string nickname,
        string email,
        string password,
        Action<RegisterUserResponse> successHandler)
    {
        ArgumentNullException.ThrowIfNull(nickname, nameof(nickname));
        ArgumentNullException.ThrowIfNull(email, nameof(email));
        ArgumentNullException.ThrowIfNull(password, nameof(password));
        ArgumentNullException.ThrowIfNull(successHandler, nameof(successHandler));

        async void SuccessHandler(HttpResponseMessage responseMessage)
        {
            (bool success, RegisterUserResponse? response) = await _apiResponseReaderService
                .ReadWithInfoAsync<RegisterUserResponse>(responseMessage)
                .ConfigureAwait(false);

            if (!success || response is null)
            {
                await _messageService
                    .ShowAlertAsync("Failed to register user")
                    .ConfigureAwait(false);

                return;
            }

            successHandler.Invoke(response);
        }

        var data = new
        {
            Nickname = nickname,
            Email = email,
            Password = password
        };

        await _httpClientInteractorService
            .PostAsync(EndpointUrls.Register, data, SuccessHandler)
            .ConfigureAwait(false);
    }
}
