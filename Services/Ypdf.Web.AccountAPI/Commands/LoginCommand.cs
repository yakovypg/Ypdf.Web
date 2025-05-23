using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Ypdf.Web.AccoutAPI.Infrastructure.Services.Authentication;
using Ypdf.Web.AccoutAPI.Models;
using Ypdf.Web.Domain.Commands;
using Ypdf.Web.Domain.Models.Api.Dto;
using Ypdf.Web.Domain.Models.Api.Exceptions;
using Ypdf.Web.Domain.Models.Api.Requests;
using Ypdf.Web.Domain.Models.Api.Responses;

namespace Ypdf.Web.AccoutAPI.Commands;

public class LoginCommand : BaseCommand, ICommand<LoginRequest, LoginResponse>
{
    private readonly ISignInService _signInService;
    private readonly ITokenGenerationService _tokenGenerationService;

    public LoginCommand(
        ISignInService signInService,
        ITokenGenerationService tokenGenerationService,
        IMapper mapper,
        ILogger<BaseCommand> logger)
        : base(
            mapper ?? throw new ArgumentNullException(nameof(mapper)),
            logger ?? throw new ArgumentNullException(nameof(logger)))
    {
        ArgumentNullException.ThrowIfNull(signInService, nameof(signInService));
        ArgumentNullException.ThrowIfNull(tokenGenerationService, nameof(tokenGenerationService));

        _signInService = signInService;
        _tokenGenerationService = tokenGenerationService;
    }

    public async Task<LoginResponse> ExecuteAsync(LoginRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        ValidateRequestParameters(request);

        Logger.LogInformation(
            "User with email {Email} is attempting to log in",
            request.Email);

        User user = await _signInService
            .SignInAsync(request.Email!, request.Password!)
            .ConfigureAwait(false);

        string token = await _tokenGenerationService
            .GenerateAsync(user)
            .ConfigureAwait(false);

        Logger.LogInformation(
            "User with email {Email} has logged in successfully",
            request.Email);

        UserDto userDto = Mapper.Map<UserDto>(user);

        return new LoginResponse()
        {
            User = userDto,
            Token = token
        };
    }

    private static void ValidateRequestParameters(LoginRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        if (string.IsNullOrEmpty(request.Email))
            throw new BadRequestException("Email not specified");

        if (string.IsNullOrEmpty(request.Password))
            throw new BadRequestException("Password not specified");
    }
}
