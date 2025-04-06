using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Ypdf.Web.AccoutAPI.Models.Dto.Requests;
using Ypdf.Web.AccoutAPI.Models.Dto.Responses;
using Ypdf.Web.Domain.Commands;

namespace Ypdf.Web.AccoutAPI.Commands;

public class RegisterUserCommand : BaseCommand, ICommand<RegisterUserRequest, RegisterUserResponse>
{
    public RegisterUserCommand(IMapper mapper, ILogger<BaseCommand> logger)
        : base(
            mapper ?? throw new ArgumentNullException(nameof(mapper)),
            logger ?? throw new ArgumentNullException(nameof(logger)))
    {
    }

    public async Task<RegisterUserResponse> ExecuteAsync(RegisterUserRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        using var memoryStream = new System.IO.MemoryStream();

        await System.Text.Json.JsonSerializer
            .SerializeAsync(memoryStream, request)
            .ConfigureAwait(false);

        string jsonData = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
        Logger.LogInformation("Register: {JsonData}", jsonData);

        var user = new Models.Dto.UserDto()
        {
            Nickname = request.Nickname,
            SubscriptionId = 2,
            UserId = 2
        };

        return new RegisterUserResponse(user);
    }
}
