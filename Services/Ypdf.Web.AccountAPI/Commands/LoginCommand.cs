using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Ypdf.Web.AccoutAPI.Models.Dto.Requests;
using Ypdf.Web.AccoutAPI.Models.Dto.Responses;
using Ypdf.Web.Domain.Commands;
using Ypdf.Web.Domain.Infrastructure.Extensions;

namespace Ypdf.Web.AccoutAPI.Commands;

public class LoginCommand : BaseCommand, ICommand<LoginRequest, LoginResponse>
{
    public LoginCommand(IMapper mapper, ILogger<BaseCommand> logger)
        : base(
            mapper ?? throw new ArgumentNullException(nameof(mapper)),
            logger ?? throw new ArgumentNullException(nameof(logger)))
    {
    }

    public async Task<LoginResponse> ExecuteAsync(LoginRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        string jsonData = await request
            .ToJsonAsync()
            .ConfigureAwait(false);

        Logger.LogInformation("Login with {JsonData}", jsonData);

        var user = new Models.Dto.UserDto()
        {
            Nickname = "some_nickname",
            SubscriptionId = 1,
            UserId = 1
        };

        return new LoginResponse(user, "some_token");
    }
}
